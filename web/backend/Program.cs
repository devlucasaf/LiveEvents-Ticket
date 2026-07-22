using System.Text;
using System.Threading.RateLimiting;

using LiveEventsTicket.Backend.Exception;
using LiveEventsTicket.Backend.Infra.Config;
using LiveEventsTicket.Backend.Infra.Security;
using LiveEventsTicket.Backend.Modules.Evento.Repository;
using LiveEventsTicket.Backend.Modules.Evento.Service;
using LiveEventsTicket.Backend.Modules.Ingresso.Repository;
using LiveEventsTicket.Backend.Modules.Ingresso.Service;
using LiveEventsTicket.Backend.Modules.Pagamento.Service;
using LiveEventsTicket.Backend.Modules.Pedido.Repository;
using LiveEventsTicket.Backend.Modules.Pedido.Service;
using LiveEventsTicket.Backend.Modules.Relatorio.Service;
using LiveEventsTicket.Backend.Modules.Admin.Service;
using LiveEventsTicket.Backend.Modules.Usuario.Repository;
using LiveEventsTicket.Backend.Modules.Usuario.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

QuestPDF.Settings.License = LicenseType.Community;

// --- REGISTRA OPCOES E SERVICOS DE SEGURANCA ---
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<CurrentUserService>();
builder.Services.AddHttpContextAccessor();

// --- INTERCEPTOR DE AUDITORIA  ---
builder.Services.AddSingleton<AuditSaveChangesInterceptor>();

// --- CONTEXTO PRINCIPAL DA APLICACAO ---
builder.Services.AddDbContext<AppDbContext>((sp, options) =>
    options
        .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
        .AddInterceptors(sp.GetRequiredService<AuditSaveChangesInterceptor>()));

// --- CONTEXTO DA TABELA Operadores PARA GERENCIAR FUNCIONARIOS ---
builder.Services.AddDbContext<PdvDbContext>((sp, options) =>
    options
        .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
        .AddInterceptors(sp.GetRequiredService<AuditSaveChangesInterceptor>()));

// --- REGISTRA OS REPOSITORIOS DE CADA MODULO ---
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IEventoRepository, EventoRepository>();
builder.Services.AddScoped<IIngressoRepository, IngressoRepository>();
builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();

// --- REGISTRA OS SERVICOS DE CADA MODULO ---
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<EventoService>();
builder.Services.AddScoped<IngressoService>();
builder.Services.AddScoped<PagamentoService>();
builder.Services.AddScoped<RelatorioService>();
builder.Services.AddScoped<AdminService>();
builder.Services.AddScoped<FuncionarioService>();

// --- SERVICES DO MODULO DE PEDIDO ---
builder.Services.AddScoped<PedidoCriacaoService>();
builder.Services.AddScoped<PedidoConsultaService>();
builder.Services.AddScoped<ReembolsoService>();
builder.Services.AddScoped<IngressoPdfService>();
builder.Services.AddScoped<IngressoCompartilhamentoService>();
builder.Services.AddScoped<CheckinService>();
builder.Services.AddScoped<PedidoService>();

// --- HABILITA CONTROLLERS E DOCUMENTACAO SWAGGER ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// --- CONFIGURA O SWAGGER COM SUPORTE A JWT BEARER ---
builder.Services.AddSwaggerGen(options =>
{
    // --- METADADOS DA API ---
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title       = "LiveEvents-Ticket API",
        Version     = "v1",
        Description = "API DE VENDA DE INGRESSOS PARA EVENTOS AO VIVO."
    });

    // --- DEFINE O ESQUEMA DE SEGURANCA (BOTAO Authorize DO SWAGGER UI) ---
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name         = "Authorization",
        Type         = SecuritySchemeType.Http,
        Scheme       = "bearer",
        BearerFormat = "JWT",
        In           = ParameterLocation.Header,
        Description  = "INFORME O TOKEN JWT. EXEMPLO: 'eyJhbGciOi...'"
    });

    // --- EXIGE O ESQUEMA Bearer NAS ROTAS PROTEGIDAS ---
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// --- REGISTRA VERIFICACOES DE SAUDE  ---
builder.Services
    .AddHealthChecks()
    .AddDbContextCheck<AppDbContext>(name: "app-db", tags: new[] { "ready", "db" })
    .AddDbContextCheck<PdvDbContext>(name: "pdv-db", tags: new[] { "ready", "db" });

// --- CONFIGURA O RATE LIMITER GLOBAL COM UMA POLITICA DEDICADA AO LOGIN ---
builder.Services.AddRateLimiter(options =>
{
    // --- RESPOSTA PADRAO QUANDO O LIMITE E EXCEDIDO ---
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // --- POLITICA "login": ATE 5 TENTATIVAS POR IP A CADA 60 SEGUNDOS (FIXED WINDOW) ---
    options.AddPolicy("login", httpContext =>
    {
        // --- IDENTIFICA O CLIENTE PELO IP; FALLBACK PARA "unknown" QUANDO INDISPONIVEL ---
        var chave = httpContext.Connection.RemoteIpAddress?.ToString()
                    ?? httpContext.Request.Headers.Host.ToString()
                    ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(chave, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit             = 5,
            Window                  = TimeSpan.FromSeconds(60),
            QueueProcessingOrder    = QueueProcessingOrder.OldestFirst,
            QueueLimit              = 0,
            AutoReplenishment       = true
        });
    });

    // --- CALLBACK EXECUTADO QUANDO A REQUISICAO E BLOQUEADA ---
    options.OnRejected = async (context, cancellationToken) =>
    {
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            context.HttpContext.Response.Headers.RetryAfter = ((int)retryAfter.TotalSeconds).ToString();
        }

        context.HttpContext.Response.ContentType = "application/json";
        await context.HttpContext.Response.WriteAsync(
            "{\"message\":\"Muitas tentativas. Tente novamente em instantes.\",\"statusCode\":429}",
            cancellationToken);
    };
});

// --- CARREGA AS CONFIGURACOES DO JWT E MONTA A CHAVE DE ASSINATURA ---
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret));

// --- CONFIGURA A AUTENTICACAO JWT BEARER E OS PARAMETROS DE VALIDACAO DO TOKEN ---
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = key
        };
    });

builder.Services.AddAuthorization();

// --- CONFIGURA O CORS LIBERANDO O FRONTEND NA PORTA 5173 ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

// --- EM DESENVOLVIMENTO: HABILITA SWAGGER E SOBE O FRONTEND ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // --- INICIA O SERVIDOR DE DESENVOLVIMENTO DO FRONTEND EM PROCESSO SEPARADO ---
    var frontendPath = Path.GetFullPath(Path.Combine(app.Environment.ContentRootPath, "..", "Frontend"));
    var npmProcess = new System.Diagnostics.Process
    {
        StartInfo = new System.Diagnostics.ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = "/c npm run dev",
            WorkingDirectory = frontendPath,
            UseShellExecute = false,
            CreateNoWindow = true
        }
    };
    npmProcess.Start();
    app.Lifetime.ApplicationStopping.Register(() => { try { npmProcess.Kill(entireProcessTree: true); } catch { } });
}

app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();

// --- ATIVA O RATE LIMITER (DEVE VIR APOS AUTH E ANTES DE MapControllers) ---
app.UseRateLimiter();

app.MapControllers();

// --- ENDPOINTS DE HEALTH CHECK ---
app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = _ => false
});

// --- VALIDA DEPENDENCIAS ---
app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

// --- AGREGA TODAS AS VERIFICACOES ---
app.MapHealthChecks("/health");

await app.SeedDataAsync();

const string urlBackend = "http://localhost:5000";
const string urlFrontend = "http://localhost:5173";
const string urlSwagger  = "http://localhost:5000/swagger";

// --- AO INICIAR A APLICACAO, EXIBE O BANNER E OS DADOS DO AMBIENTE ---
app.Lifetime.ApplicationStarted.Register(() =>
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine(@"
    █████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗
    ╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝

    ██╗     ██╗██╗   ██╗███████╗    ███████╗██╗   ██╗███████╗███╗   ██╗████████╗███████╗    ████████╗██╗ ██████╗██╗  ██╗███████╗████████╗
    ██║     ██║██║   ██║██╔════╝    ██╔════╝██║   ██║██╔════╝████╗  ██║╚══██╔══╝██╔════╝    ╚══██╔══╝██║██╔════╝██║ ██╔╝██╔════╝╚══██╔══╝
    ██║     ██║██║   ██║█████╗      █████╗  ██║   ██║█████╗  ██╔██╗ ██║   ██║   ███████╗       ██║   ██║██║     █████╔╝ █████╗     ██║   
    ██║     ██║╚██╗ ██╔╝██╔══╝      ██╔══╝  ╚██╗ ██╔╝██╔══╝  ██║╚██╗██║   ██║   ╚════██║       ██║   ██║██║     ██╔═██╗ ██╔══╝     ██║   
    ███████╗██║ ╚████╔╝ ███████╗    ███████╗ ╚████╔╝ ███████╗██║ ╚████║   ██║   ███████║       ██║   ██║╚██████╗██║  ██╗███████╗   ██║   
    ╚══════╝╚═╝  ╚═══╝  ╚══════╝    ╚══════╝  ╚═══╝  ╚══════╝╚═╝  ╚═══╝   ╚═╝   ╚══════╝       ╚═╝   ╚═╝ ╚═════╝╚═╝  ╚═╝╚══════╝   ╚═╝ 

    █████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗
    ╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝ 
    ");
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine($@"
        ==================================================================
        | Aplicação:     LiveEvents-Ticket                               |
        | .NET:          {Environment.Version}                           |
        | Ambiente:      {app.Environment.EnvironmentName}               |
        | Backend:       {urlBackend}                                    |
        | Frontend:      {urlFrontend}                                   |
        | Swagger:       {urlSwagger}                                    |
        | Desenvolvedor: Lucas Freitas                                   | 
        | GitHub:        https://github.com/devlucasaf/LiveEvents-Ticket |
        ==================================================================
    ");
    Console.ResetColor();
});

app.Run();

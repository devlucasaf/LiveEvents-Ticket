using System.Text;
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
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<CurrentUserService>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IEventoRepository, EventoRepository>();
builder.Services.AddScoped<IIngressoRepository, IngressoRepository>();
builder.Services.AddScoped<IPedidoRepository, PedidoRepository>();

builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<EventoService>();
builder.Services.AddScoped<IngressoService>();
builder.Services.AddScoped<PagamentoService>();
builder.Services.AddScoped<PedidoService>();
builder.Services.AddScoped<RelatorioService>();
builder.Services.AddScoped<AdminService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret));

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

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

app.MapControllers();

await app.SeedDataAsync();

app.Lifetime.ApplicationStarted.Register(() =>
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine(@"
    ██╗     ██╗██╗   ██╗███████╗    ███████╗██╗   ██╗███████╗███╗   ██╗████████╗███████╗    ████████╗██╗ ██████╗██╗  ██╗███████╗████████╗
    ██║     ██║██║   ██║██╔════╝    ██╔════╝██║   ██║██╔════╝████╗  ██║╚══██╔══╝██╔════╝    ╚══██╔══╝██║██╔════╝██║ ██╔╝██╔════╝╚══██╔══╝
    ██║     ██║██║   ██║█████╗      █████╗  ██║   ██║█████╗  ██╔██╗ ██║   ██║   ███████╗       ██║   ██║██║     █████╔╝ █████╗     ██║   
    ██║     ██║╚██╗ ██╔╝██╔══╝      ██╔══╝  ╚██╗ ██╔╝██╔══╝  ██║╚██╗██║   ██║   ╚════██║       ██║   ██║██║     ██╔═██╗ ██╔══╝     ██║   
    ███████╗██║ ╚████╔╝ ███████╗    ███████╗ ╚████╔╝ ███████╗██║ ╚████║   ██║   ███████║       ██║   ██║╚██████╗██║  ██╗███████╗   ██║   
    ╚══════╝╚═╝  ╚═══╝  ╚══════╝    ╚══════╝  ╚═══╝  ╚══════╝╚═╝  ╚═══╝   ╚═╝   ╚══════╝       ╚═╝   ╚═╝ ╚═════╝╚═╝  ╚═╝╚══════╝   ╚═╝ 
    ");
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine($@"
        Aplicação:     LiveEvents-Ticket
        .NET:          {Environment.Version}
        Ambiente:      {app.Environment.EnvironmentName}
        Backend:       http://localhost:5000
        Frontend:      http://localhost:5173
        Swagger:       http://localhost:5000/swagger
        Desenvolvedor: Lucas Freitas
        GitHub:        https://github.com/devlucasaf/LiveEvents-Ticket
        ========================================================================================
    ");
    Console.ResetColor();
});

app.Run();

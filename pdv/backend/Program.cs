using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.FileProviders;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PontoVenda.Backend.Exception;
using PontoVenda.Backend.Infra.Config;
using PontoVenda.Backend.Infra.Security;
using PontoVenda.Backend.Modules.Balcao.Service;
using PontoVenda.Backend.Modules.PontoVenda.Repository;
using PontoVenda.Backend.Modules.PontoVenda.Service;

var builder = WebApplication.CreateBuilder(args);

// --- CONFIGURAÇÃO DO JWT ---
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<CurrentUserService>();
builder.Services.AddHttpContextAccessor();

// --- ENTITY FRAMEWORK CORE ---
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- CONTEXTO COMPARTILHADO COM O WEB ---
builder.Services.AddDbContext<SharedDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- REPOSITORIES ---
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<IVendaRepository, VendaRepository>();
builder.Services.AddScoped<IOperadorRepository, OperadorRepository>();
builder.Services.AddScoped<IVendaFisicaRepository, VendaFisicaRepository>();

// --- SERVICES ---
builder.Services.AddScoped<ProdutoService>();
builder.Services.AddScoped<VendaService>();
builder.Services.AddScoped<OperadorService>();
builder.Services.AddScoped<VendaFisicaService>();
builder.Services.AddScoped<BalcaoService>();

// --- CONTROLLERS, SWAGGER E API ---
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- AUTENTICAÇÃO JWT BEARER ---
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

// --- CORS ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
    });
});

var app = builder.Build();

// --- MIDDLEWARE DE EXCEÇÕES GLOBAL ---
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Frontend");

// --- FRONTEND ---
var frontendPath = Path.GetFullPath(Path.Combine(builder.Environment.ContentRootPath, "..", "frontend"));

if (Directory.Exists(frontendPath))
{
    var frontendProvider = new PhysicalFileProvider(frontendPath);

    app.UseDefaultFiles(new DefaultFilesOptions
    {
        FileProvider  = frontendProvider,
        DefaultFileNames = new List<string> { "index.html" }
    });

    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = frontendProvider
    });
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await app.SeedDataAsync();

const string urlFrontend = "http://localhost:5100";
const string urlSwagger  = "http://localhost:5100/swagger";

// --- BANNER DE INICIALIZAÇÃO ---
app.Lifetime.ApplicationStarted.Register(() =>
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine(@"
    █████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗
    ╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝         
    
    ██╗     ██╗██╗   ██╗███████╗    ███████╗██╗   ██╗███████╗███╗   ██╗████████╗███████╗              ██████╗ ██████╗ ██╗   ██╗
    ██║     ██║██║   ██║██╔════╝    ██╔════╝██║   ██║██╔════╝████╗  ██║╚══██╔══╝██╔════╝              ██╔══██╗██╔══██╗██║   ██║
    ██║     ██║██║   ██║█████╗      █████╗  ██║   ██║█████╗  ██╔██╗ ██║   ██║   ███████╗    █████╗    ██████╔╝██║  ██║██║   ██║
    ██║     ██║╚██╗ ██╔╝██╔══╝      ██╔══╝  ╚██╗ ██╔╝██╔══╝  ██║╚██╗██║   ██║   ╚════██║    ╚════╝    ██╔═══╝ ██║  ██║╚██╗ ██╔╝
    ███████╗██║ ╚████╔╝ ███████╗    ███████╗ ╚████╔╝ ███████╗██║ ╚████║   ██║   ███████║              ██║     ██████╔╝ ╚████╔╝ 
    ╚══════╝╚═╝  ╚═══╝  ╚══════╝    ╚══════╝  ╚═══╝  ╚══════╝╚═╝  ╚═══╝   ╚═╝   ╚══════╝              ╚═╝     ╚═════╝   ╚═══╝  

    █████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗█████╗
    ╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝╚════╝ 
    ");
    Console.ForegroundColor = ConsoleColor.White;
    Console.WriteLine($@"
        ==================================================================
        | Aplicação:     LiveEvents - PDV                                |
        | .NET:          {Environment.Version}                           |   
        | Ambiente:      {app.Environment.EnvironmentName}               |               
        | Backend:       http://localhost:5000                           |   
        | Frontend:      {urlFrontend}                                   |                               
        | Swagger:       {urlSwagger}                                    |
        | Desenvolvedor: Lucas Freitas                                   |
        | GitHub:        https://github.com/devlucasaf/LiveEvents-Ticket |
        ==================================================================
    ");
    Console.ResetColor();
});

if (app.Environment.IsDevelopment())
{
    _ = Task.Run(async () =>
    {
        await Task.Delay(1200);
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName        = urlFrontend,
                UseShellExecute = true
            });
        }
        catch
        {
        }
    });
}


app.Run();

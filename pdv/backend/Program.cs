using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.FileProviders;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PontoVenda.Backend.Exception;
using PontoVenda.Backend.Infra.Config;
using PontoVenda.Backend.Infra.Security;
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

// --- CONTROLLERS, SWAGGER E API ---
builder.Services.AddControllers();
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

// --- FRONTEND ESTÁTICO (HTML/CSS/JS PURO) ---
// Caminho da pasta /pdv/frontend (um nível acima do backend)
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

// --- SEED INICIAL DE DADOS ---
await app.SeedDataAsync();

const string urlFrontend = "http://localhost:5100";
const string urlSwagger  = "http://localhost:5100/swagger";

// --- BANNER DE INICIALIZAÇÃO ---
Console.WriteLine();
Console.WriteLine("+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=");
Console.WriteLine("    PONTO DE VENDA - Aplicação completa");
Console.WriteLine("    .NET:       8.0");
Console.WriteLine("    Ambiente:   " + app.Environment.EnvironmentName);
Console.WriteLine("    Frontend:   " + urlFrontend);
Console.WriteLine("    API:        " + urlFrontend + "/api");
Console.WriteLine("    Swagger:    " + urlSwagger);
Console.WriteLine("+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=");
Console.WriteLine();

// --- ABRE O NAVEGADOR AUTOMATICAMENTE EM DEVELOPMENT ---
if (app.Environment.IsDevelopment())
{
    _ = Task.Run(async () =>
    {
        // Pequeno delay para o Kestrel terminar de subir
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
            // Silencioso: se não conseguir abrir, o link continua disponível no console
        }
    });
}

app.Run();

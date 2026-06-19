using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// --- SEED INICIAL DE DADOS ---
await app.SeedDataAsync();

// --- BANNER DE INICIALIZAÇÃO ---
Console.WriteLine();
Console.WriteLine("+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=");
Console.WriteLine("    PONTO DE VENDA - Backend");
Console.WriteLine("    .NET:       8.0");
Console.WriteLine("    Ambiente:   " + app.Environment.EnvironmentName);
Console.WriteLine("    Backend:    http://localhost:5100");
Console.WriteLine("    Swagger:    http://localhost:5100/swagger");
Console.WriteLine("+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=");
Console.WriteLine();

app.Run();

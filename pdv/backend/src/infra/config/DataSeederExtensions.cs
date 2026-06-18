using Microsoft.EntityFrameworkCore;
using PontoVenda.Backend.Modules.PontoVenda.Model;

namespace PontoVenda.Backend.Infra.Config;

public static class DataSeederExtensions
{
    // --- GARANTIR CRIAÇÃO DO BANCO E POPULAR DADOS INICIAIS ---
    public static async Task SeedDataAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await context.Database.EnsureCreatedAsync();

        // --- SEED DO OPERADOR ADMIN ---
        if (!await context.Operadores.AnyAsync())
        {
            context.Operadores.Add(new Operador
            {
                Nome = "Administrador",
                Login = "admin",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = "ADMIN"
            });
        }

        // --- SEED DE PRODUTOS DE EXEMPLO ---
        if (!await context.Produtos.AnyAsync())
        {
            context.Produtos.AddRange(
                new Produto { Nome = "Refrigerante 350ml",  CodigoBarras = "7891000000017", Preco = 6.50m,  EstoqueAtual = 120 },
                new Produto { Nome = "Cerveja Lata 473ml",  CodigoBarras = "7891000000024", Preco = 9.90m,  EstoqueAtual = 200 },
                new Produto { Nome = "Água Mineral 500ml",  CodigoBarras = "7891000000031", Preco = 4.00m,  EstoqueAtual = 300 },
                new Produto { Nome = "Pipoca Pequena",       CodigoBarras = "7891000000048", Preco = 12.00m, EstoqueAtual = 80 },
                new Produto { Nome = "Hot Dog",              CodigoBarras = "7891000000055", Preco = 15.00m, EstoqueAtual = 60 }
            );
        }

        await context.SaveChangesAsync();
    }
}

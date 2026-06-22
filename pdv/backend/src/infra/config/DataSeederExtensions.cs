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

        // --- SEED DE EVENTO ---
        if (!await context.Eventos.AnyAsync())
        {
            var evento = new Evento
            {
                Nome = "Show Internacional 2026",
                Local = "Estádio Central - São Paulo/SP",
                DataEvento = new DateTime(2026, 12, 15, 21, 0, 0, DateTimeKind.Utc),
                Descricao = "Turnê mundial com convidados especiais. Abertura dos portões às 18h.",
                Ativo = true
            };

            // --- CAMAROTE VIP ---
            for (var fileira = 'A'; fileira <= 'B'; fileira++)
            {
                for (var num = 1; num <= 10; num++)
                {
                    evento.Assentos.Add(new Assento
                    {
                        EventoId = evento.Id,
                        Setor = "CAMAROTE_VIP",
                        Fileira = fileira.ToString(),
                        Numero = num,
                        Preco = 800.00m,
                        Status = "DISPONIVEL"
                    });
                }
            }

            // --- PISTA PREMIUM ---
            for (var fileira = 'A'; fileira <= 'C'; fileira++)
            {
                for (var num = 1; num <= 15; num++)
                {
                    evento.Assentos.Add(new Assento
                    {
                        EventoId = evento.Id,
                        Setor = "PISTA_PREMIUM",
                        Fileira = fileira.ToString(),
                        Numero = num,
                        Preco = 350.00m,
                        Status = "DISPONIVEL"
                    });
                }
            }

            // --- ARQUIBANCADA ---
            for (var fileira = 'A'; fileira <= 'E'; fileira++)
            {
                for (var num = 1; num <= 20; num++)
                {
                    evento.Assentos.Add(new Assento
                    {
                        EventoId = evento.Id,
                        Setor = "ARQUIBANCADA",
                        Fileira = fileira.ToString(),
                        Numero = num,
                        Preco = 150.00m,
                        Status = "DISPONIVEL"
                    });
                }
            }

            context.Eventos.Add(evento);
        }

        await context.SaveChangesAsync();
    }
}

using LiveEventsTicket.Backend.Modules.Evento.Model;
using LiveEventsTicket.Backend.Modules.Ingresso.Model;
using LiveEventsTicket.Backend.Modules.Usuario.Model;
using Microsoft.EntityFrameworkCore;

namespace LiveEventsTicket.Backend.Infra.Config;

public static class DataSeederExtensions
{
    public static async Task SeedDataAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await context.Database.EnsureCreatedAsync();

        if (!await context.Usuarios.AnyAsync())
        {
            context.Usuarios.Add(new Usuario
            {
                Nome = "Administrador",
                Email = "admin@liveevents.com",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = "ADMIN"
            });
        }

        if (!await context.Eventos.AnyAsync())
        {
            var evento1 = new Evento
            {
                Titulo = "Festival Brasil Music",
                Categoria = "SHOW NACIONAL",
                Local = "Allianz Parque - SP",
                DataEvento = DateTime.UtcNow.AddDays(40),
                Descricao = "Grandes nomes da música nacional.",
                ImagemUrl = "https://images.unsplash.com/photo-1459749411175-04bf5292ceea?w=800"
            };

            var evento2 = new Evento
            {
                Titulo = "International Pop Night",
                Categoria = "SHOW INTERNACIONAL",
                Local = "Jeunesse Arena - RJ",
                DataEvento = DateTime.UtcNow.AddDays(60),
                Descricao = "Turnê internacional com estrutura premium.",
                ImagemUrl = "https://images.unsplash.com/photo-1470229722913-7c0e2dbbafd3?w=800"
            };

            context.Eventos.AddRange(evento1, evento2);
            await context.SaveChangesAsync();

            context.Ingressos.AddRange(
                new Ingresso { EventoId = evento1.Id, Setor = "PISTA", Preco = 180, QuantidadeDisponivel = 500 },
                new Ingresso { EventoId = evento1.Id, Setor = "VIP", Preco = 320, QuantidadeDisponivel = 200 },
                new Ingresso { EventoId = evento2.Id, Setor = "PISTA", Preco = 250, QuantidadeDisponivel = 700 },
                new Ingresso { EventoId = evento2.Id, Setor = "PREMIUM", Preco = 480, QuantidadeDisponivel = 250 }
            );
        }

        await context.SaveChangesAsync();
    }
}

using LiveEventsTicket.Backend.Modules.Evento.Model;
using LiveEventsTicket.Backend.Modules.Ingresso.Model;
using LiveEventsTicket.Backend.Modules.Usuario.Model;
using Microsoft.EntityFrameworkCore;

namespace LiveEventsTicket.Backend.Infra.Config;

public static class DataSeederExtensions
{
    // --- APLICA MIGRATIONS PENDENTES E INSERE OS DADOS DEMO NA INICIALIZACAO ---
    public static async Task SeedDataAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var logger = scope.ServiceProvider
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("DataSeeder");

        var context     = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var pdvContext  = scope.ServiceProvider.GetRequiredService<PdvDbContext>();

        logger.LogInformation("APLICANDO MIGRATIONS DO AppDbContext...");
        await context.Database.MigrateAsync();

        logger.LogInformation("APLICANDO MIGRATIONS DO PdvDbContext...");
        await pdvContext.Database.MigrateAsync();

        // --- CRIA O USUARIO ADMIN SE AINDA NAO EXISTIR ---
        if (!await context.Usuarios.AnyAsync())
        {
            context.Usuarios.Add(new Usuario
            {
                Nome      = "Administrador",
                Email     = "admin@liveevents.com",
                SenhaHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role      = "ADMIN"
            });
        }

        // --- CRIA OS EVENTOS DEMO SE AINDA NAO EXISTIREM ---
        if (!await context.Eventos.AnyAsync())
        {
            // --- EVENTO NACIONAL ---
            var evento1 = new Evento
            {
                Titulo      = "Festival Brasil Music",
                Categoria   = "SHOW NACIONAL",
                Local       = "Allianz Parque - SP",
                DataEvento  = DateTime.UtcNow.AddDays(40),
                Descricao   = "Grandes nomes da música nacional.",
                ImagemUrl   = "https://images.unsplash.com/photo-1459749411175-04bf5292ceea?w=800"
            };

            // --- EVENTO INTERNACIONAL ---
            var evento2 = new Evento
            {
                Titulo      = "International Pop Night",
                Categoria   = "SHOW INTERNACIONAL",
                Local       = "Jeunesse Arena - RJ",
                DataEvento  = DateTime.UtcNow.AddDays(60),
                Descricao   = "Turnê internacional com estrutura premium.",
                ImagemUrl   = "https://images.unsplash.com/photo-1470229722913-7c0e2dbbafd3?w=800"
            };

            context.Eventos.AddRange(evento1, evento2);
            await context.SaveChangesAsync();

            // --- CRIA OS 5 SETORES OFICIAIS PARA CADA EVENTO DEMO ---
            foreach (var eventoId in new[] { evento1.Id, evento2.Id })
            {
                foreach (var setor in CatalogoIngresso.Setores)
                {
                    context.Ingressos.Add(new Ingresso
                    {
                        EventoId                = eventoId,
                        Setor                   = setor.Nome,
                        Preco                   = setor.PrecoPadrao,
                        QuantidadeDisponivel    = 500
                    });
                }
            }
        }

        // --- PERSISTE OS DADOS DE SEED ---
        await context.SaveChangesAsync();
    }
}

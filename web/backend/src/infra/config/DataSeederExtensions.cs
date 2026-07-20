using LiveEventsTicket.Backend.Modules.Evento.Model;
using LiveEventsTicket.Backend.Modules.Ingresso.Model;
using LiveEventsTicket.Backend.Modules.Usuario.Model;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace LiveEventsTicket.Backend.Infra.Config;

public static class DataSeederExtensions
{
    public static async Task SeedDataAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await EnsureTablesCreatedAsync(context);

        await EnsureItensPedidoColunasAsync(context);

        var pdvContext = scope.ServiceProvider.GetRequiredService<PdvDbContext>();
        await EnsureOperadoresTabelaAsync(pdvContext);

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

            // --- CRIA OS 5 SETORES OFICIAIS PARA CADA EVENTO DEMO --- 
            foreach (var eventoId in new[] { evento1.Id, evento2.Id })
            {
                foreach (var setor in CatalogoIngresso.Setores)
                {
                    context.Ingressos.Add(new Ingresso
                    {
                        EventoId = eventoId,
                        Setor = setor.Nome,
                        Preco = setor.PrecoPadrao,
                        QuantidadeDisponivel = 500
                    });
                }
            }
        }

        await context.SaveChangesAsync();
    }

    // --- CRIA O BANCO E AS TABELAS DESTE MODULO ---
    private static async Task EnsureTablesCreatedAsync(AppDbContext context)
    {
        var creator = context.GetService<IRelationalDatabaseCreator>();

        if (!await creator.ExistsAsync())
        {
            await creator.CreateAsync();
        }

        try
        {
            await creator.CreateTablesAsync();
        }
        catch (SqlException)
        {
        }
    }

    // --- GARANTE AS COLUNAS Modalidade E Subtipo NA TABELA ItensPedido ---
    private static async Task EnsureItensPedidoColunasAsync(AppDbContext context)
    {
        try
        {
            await context.Database.ExecuteSqlRawAsync(
                "IF COL_LENGTH('ItensPedido', 'Modalidade') IS NULL " +
                "ALTER TABLE ItensPedido ADD Modalidade nvarchar(max) NOT NULL DEFAULT 'INTEIRA';");

            await context.Database.ExecuteSqlRawAsync(
                "IF COL_LENGTH('ItensPedido', 'Subtipo') IS NULL " +
                "ALTER TABLE ItensPedido ADD Subtipo nvarchar(max) NULL;");

            await context.Database.ExecuteSqlRawAsync(
                "IF COL_LENGTH('ItensPedido', 'DocumentosJson') IS NULL " +
                "ALTER TABLE ItensPedido ADD DocumentosJson nvarchar(max) NULL;");
        }
        catch (SqlException)
        {
        }
    }

    // --- GARANTE A TABELA Operadores E A COLUNA Ativo ---
    private static async Task EnsureOperadoresTabelaAsync(PdvDbContext context)
    {
        try
        {
            var creator = context.GetService<IRelationalDatabaseCreator>();
            if (!await creator.ExistsAsync())
            {
                await creator.CreateAsync();
            }

            await context.Database.ExecuteSqlRawAsync(
                "IF OBJECT_ID('Operadores', 'U') IS NULL " +
                "CREATE TABLE Operadores (" +
                "Id int IDENTITY(1,1) NOT NULL PRIMARY KEY, " +
                "Nome nvarchar(max) NOT NULL DEFAULT '', " +
                "Login nvarchar(max) NOT NULL DEFAULT '', " +
                "SenhaHash nvarchar(max) NOT NULL DEFAULT '', " +
                "Role nvarchar(max) NOT NULL DEFAULT 'OPERADOR', " +
                "Ativo bit NOT NULL DEFAULT 1, " +
                "CreatedAt datetime2 NOT NULL DEFAULT SYSUTCDATETIME(), " +
                "UpdatedAt datetime2 NULL);");

            await context.Database.ExecuteSqlRawAsync(
                "IF COL_LENGTH('Operadores', 'Ativo') IS NULL " +
                "ALTER TABLE Operadores ADD Ativo bit NOT NULL DEFAULT 1;");
        }
        catch (SqlException)
        {
        }
    }
}

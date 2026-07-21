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
        await EnsurePedidoCompradorColunasAsync(context);
        await EnsurePedidoReembolsoColunasAsync(context);
        await EnsureUsuarioEnderecoColunasAsync(context);
        await EnsurePedidoCheckinColunasAsync(context);
        await EnsurePedidoCompartilhamentoColunasAsync(context);
        await EnsurePedidoCheckinLogTabelaAsync(context);

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

    // --- GARANTE AS COLUNAS DE DADOS DO COMPRADOR NA TABELA Pedidos ---
    private static async Task EnsurePedidoCompradorColunasAsync(AppDbContext context)
    {
        try
        {
            await context.Database.ExecuteSqlRawAsync(
                "IF COL_LENGTH('Pedidos', 'CompradorNome') IS NULL " +
                "ALTER TABLE Pedidos ADD CompradorNome nvarchar(max) NOT NULL DEFAULT ''; "
            );

            await context.Database.ExecuteSqlRawAsync(
                "IF COL_LENGTH('Pedidos', 'CompradorCpf') IS NULL " +
                "ALTER TABLE Pedidos ADD CompradorCpf nvarchar(max) NOT NULL DEFAULT ''; "
            );

            await context.Database.ExecuteSqlRawAsync(
                "IF COL_LENGTH('Pedidos', 'CompradorEmail') IS NULL " +
                "ALTER TABLE Pedidos ADD CompradorEmail nvarchar(max) NOT NULL DEFAULT ''; "
            );

            await context.Database.ExecuteSqlRawAsync(
                "IF COL_LENGTH('Pedidos', 'CompradorTelefone') IS NULL " +
                "ALTER TABLE Pedidos ADD CompradorTelefone nvarchar(max) NOT NULL DEFAULT ''; "
            );

            await context.Database.ExecuteSqlRawAsync(
                "IF COL_LENGTH('Pedidos', 'CompradorDataNascimento') IS NULL " +
                "ALTER TABLE Pedidos ADD CompradorDataNascimento nvarchar(max) NOT NULL DEFAULT ''; "
            );

            await context.Database.ExecuteSqlRawAsync(
                "IF COL_LENGTH('Pedidos', 'EnderecoCep') IS NULL " +
                "ALTER TABLE Pedidos ADD EnderecoCep nvarchar(max) NOT NULL DEFAULT ''; "
            );

            await context.Database.ExecuteSqlRawAsync(
                "IF COL_LENGTH('Pedidos', 'EnderecoLogradouro') IS NULL " +
                "ALTER TABLE Pedidos ADD EnderecoLogradouro nvarchar(max) NOT NULL DEFAULT ''; "
            );

            await context.Database.ExecuteSqlRawAsync(
                "IF COL_LENGTH('Pedidos', 'EnderecoNumero') IS NULL " +
                "ALTER TABLE Pedidos ADD EnderecoNumero nvarchar(max) NOT NULL DEFAULT ''; "
            );

            await context.Database.ExecuteSqlRawAsync(
                "IF COL_LENGTH('Pedidos', 'EnderecoComplemento') IS NULL " +
                "ALTER TABLE Pedidos ADD EnderecoComplemento nvarchar(max) NULL; "
            );

            await context.Database.ExecuteSqlRawAsync(
                "IF COL_LENGTH('Pedidos', 'EnderecoBairro') IS NULL " +
                "ALTER TABLE Pedidos ADD EnderecoBairro nvarchar(max) NOT NULL DEFAULT ''; "
            );

            await context.Database.ExecuteSqlRawAsync(
                "IF COL_LENGTH('Pedidos', 'EnderecoCidade') IS NULL " +
                "ALTER TABLE Pedidos ADD EnderecoCidade nvarchar(max) NOT NULL DEFAULT ''; "
            );

            await context.Database.ExecuteSqlRawAsync(
                "IF COL_LENGTH('Pedidos', 'EnderecoEstado') IS NULL " +
                "ALTER TABLE Pedidos ADD EnderecoEstado nvarchar(max) NOT NULL DEFAULT ''; "
            );
        }
        catch (SqlException)
        {
        }
    }

    // --- GARANTE AS COLUNAS DE REEMBOLSO NA TABELA Pedidos ---
    private static async Task EnsurePedidoReembolsoColunasAsync(AppDbContext context)
    {
        try
        {
            await context.Database.ExecuteSqlRawAsync(
                "IF COL_LENGTH('Pedidos', 'ReembolsoSolicitadoEm') IS NULL " +
                "ALTER TABLE Pedidos ADD ReembolsoSolicitadoEm datetime2 NULL;"
            );

            await context.Database.ExecuteSqlRawAsync(
                "IF COL_LENGTH('Pedidos', 'ReembolsoAprovadoEm') IS NULL " +
                "ALTER TABLE Pedidos ADD ReembolsoAprovadoEm datetime2 NULL;"
            );

            await context.Database.ExecuteSqlRawAsync(
                "IF COL_LENGTH('Pedidos', 'ReembolsoMotivo') IS NULL " +
                "ALTER TABLE Pedidos ADD ReembolsoMotivo nvarchar(max) NULL;"
            );

            await context.Database.ExecuteSqlRawAsync(
                "IF COL_LENGTH('Pedidos', 'ReembolsoMotivoCodigo') IS NULL " +
                "ALTER TABLE Pedidos ADD ReembolsoMotivoCodigo nvarchar(100) NULL;"
            );

            await context.Database.ExecuteSqlRawAsync(
                "IF COL_LENGTH('Pedidos', 'ReembolsoRegraAplicada') IS NULL " +
                "ALTER TABLE Pedidos ADD ReembolsoRegraAplicada nvarchar(max) NULL;"
            );
        }
        catch (SqlException)
        {
        }
    }

    // --- GARANTE AS COLUNAS DE CHECKIN NA TABELA Pedidos ---
    private static async Task EnsurePedidoCheckinColunasAsync(AppDbContext context)
    {
        try
        {
            await context.Database.ExecuteSqlRawAsync(
                "IF COL_LENGTH('Pedidos', 'CheckinToken') IS NULL " +
                "ALTER TABLE Pedidos ADD CheckinToken nvarchar(max) NOT NULL DEFAULT ''; "
            );

            await context.Database.ExecuteSqlRawAsync(
                "IF COL_LENGTH('Pedidos', 'CheckinUsosRealizados') IS NULL " +
                "ALTER TABLE Pedidos ADD CheckinUsosRealizados int NOT NULL DEFAULT 0; "
            );
        }
        catch (SqlException)
        {
        }
    }

    // --- GARANTE AS COLUNAS DE COMPARTILHAMENTO NA TABELA Pedidos ---
    private static async Task EnsurePedidoCompartilhamentoColunasAsync(AppDbContext context)
    {
        try
        {
            await context.Database.ExecuteSqlRawAsync(
                "IF COL_LENGTH('Pedidos', 'CompartilhamentoToken') IS NULL " +
                "ALTER TABLE Pedidos ADD CompartilhamentoToken nvarchar(max) NULL; "
            );

            await context.Database.ExecuteSqlRawAsync(
                "IF COL_LENGTH('Pedidos', 'CompartilhamentoExpiraEm') IS NULL " +
                "ALTER TABLE Pedidos ADD CompartilhamentoExpiraEm datetime2 NULL; "
            );

            await context.Database.ExecuteSqlRawAsync(
                "IF COL_LENGTH('Pedidos', 'CompartilhamentoRevogadoEm') IS NULL " +
                "ALTER TABLE Pedidos ADD CompartilhamentoRevogadoEm datetime2 NULL; "
            );

            await context.Database.ExecuteSqlRawAsync(
                "IF COL_LENGTH('Pedidos', 'CompartilhamentoMaxAcessos') IS NULL " +
                "ALTER TABLE Pedidos ADD CompartilhamentoMaxAcessos int NOT NULL DEFAULT 0; "
            );

            await context.Database.ExecuteSqlRawAsync(
                "IF COL_LENGTH('Pedidos', 'CompartilhamentoAcessosRealizados') IS NULL " +
                "ALTER TABLE Pedidos ADD CompartilhamentoAcessosRealizados int NOT NULL DEFAULT 0; "
            );
        }
        catch (SqlException)
        {
        }
    }

    // --- GARANTE A TABELA DE LOGS DE CHECKIN ---
    private static async Task EnsurePedidoCheckinLogTabelaAsync(AppDbContext context)
    {
        try
        {
            await context.Database.ExecuteSqlRawAsync(
                "IF OBJECT_ID('PedidoCheckinLogs', 'U') IS NULL " +
                "CREATE TABLE PedidoCheckinLogs (" +
                "Id int IDENTITY(1,1) NOT NULL PRIMARY KEY, " +
                "PedidoId int NULL, " +
                "OperadorId int NOT NULL, " +
                "TokenInformado nvarchar(300) NOT NULL DEFAULT '', " +
                "Permitido bit NOT NULL DEFAULT 0, " +
                "Mensagem nvarchar(500) NOT NULL DEFAULT '', " +
                "DataCheckin datetime2 NOT NULL DEFAULT SYSUTCDATETIME());"
            );
        }
        catch (SqlException)
        {
        }
    }

    // --- GARANTE AS COLUNAS DE ENDERECO NA TABELA Usuarios ---
    private static async Task EnsureUsuarioEnderecoColunasAsync(AppDbContext context)
    {
        try
        {
            await context.Database.ExecuteSqlRawAsync(
                "IF COL_LENGTH('Usuarios', 'Cep') IS NULL " +
                "ALTER TABLE Usuarios ADD Cep nvarchar(max) NOT NULL DEFAULT ''; "
            );

            await context.Database.ExecuteSqlRawAsync(
                "IF COL_LENGTH('Usuarios', 'Logradouro') IS NULL " +
                "ALTER TABLE Usuarios ADD Logradouro nvarchar(max) NOT NULL DEFAULT ''; "
            );

            await context.Database.ExecuteSqlRawAsync(
                "IF COL_LENGTH('Usuarios', 'Numero') IS NULL " +
                "ALTER TABLE Usuarios ADD Numero nvarchar(max) NOT NULL DEFAULT ''; "
            );

            await context.Database.ExecuteSqlRawAsync(
                "IF COL_LENGTH('Usuarios', 'Complemento') IS NULL " +
                "ALTER TABLE Usuarios ADD Complemento nvarchar(max) NULL; "
            );

            await context.Database.ExecuteSqlRawAsync(
                "IF COL_LENGTH('Usuarios', 'Bairro') IS NULL " +
                "ALTER TABLE Usuarios ADD Bairro nvarchar(max) NOT NULL DEFAULT ''; "
            );

            await context.Database.ExecuteSqlRawAsync(
                "IF COL_LENGTH('Usuarios', 'Cidade') IS NULL " +
                "ALTER TABLE Usuarios ADD Cidade nvarchar(max) NOT NULL DEFAULT ''; "
            );

            await context.Database.ExecuteSqlRawAsync(
                "IF COL_LENGTH('Usuarios', 'Estado') IS NULL " +
                "ALTER TABLE Usuarios ADD Estado nvarchar(max) NOT NULL DEFAULT ''; "
            );
        }
        catch (SqlException)
        {
        }
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

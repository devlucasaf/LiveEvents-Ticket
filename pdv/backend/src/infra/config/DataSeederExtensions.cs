using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using PontoVenda.Backend.Modules.PontoVenda.Model;

namespace PontoVenda.Backend.Infra.Config;

public static class DataSeederExtensions
{
    // --- GARANTIR CRIAÇÃO DO BANCO E POPULAR DADOS INICIAIS ---
    public static async Task SeedDataAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await EnsureTablesCreatedAsync(context);
        await GarantirColunaAtivoAsync(context);
        await GarantirTabelaVendasBalcaoAsync(context);

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
        {}
    }

    // --- ADICIONA A COLUNA "Ativo" NA TABELA Operadores SE AINDA NAO EXISTIR ---
    private static async Task GarantirColunaAtivoAsync(AppDbContext context)
    {
        try
        {
            await context.Database.ExecuteSqlRawAsync(
                "IF COL_LENGTH('Operadores', 'Ativo') IS NULL " +
                "ALTER TABLE Operadores ADD Ativo bit NOT NULL DEFAULT 1;");
        }
        catch (SqlException)
        {}
    }

    // --- CRIA A TABELA VendasBalcao SE AINDA NAO EXISTIR ---
    private static async Task GarantirTabelaVendasBalcaoAsync(AppDbContext context)
    {
        await context.Database.ExecuteSqlRawAsync(@"
IF OBJECT_ID('VendasBalcao', 'U') IS NULL
BEGIN
    CREATE TABLE VendasBalcao (
        Id                    uniqueidentifier NOT NULL CONSTRAINT PK_VendasBalcao PRIMARY KEY,
        OperadorId            int              NOT NULL,
        OperadorNome          nvarchar(max)    NOT NULL,
        ClienteWebId          int              NOT NULL,
        ClienteNome           nvarchar(max)    NOT NULL,
        ClienteSobrenome      nvarchar(max)    NOT NULL,
        ClienteEmail          nvarchar(max)    NOT NULL,
        ClienteCpf            nvarchar(max)    NOT NULL,
        ClienteTelefone       nvarchar(max)    NOT NULL,
        ClienteDataNascimento datetime2        NULL,
        Cep                   nvarchar(max)    NULL,
        Logradouro            nvarchar(max)    NULL,
        Numero                nvarchar(max)    NULL,
        Complemento           nvarchar(max)    NULL,
        Bairro                nvarchar(max)    NULL,
        Cidade                nvarchar(max)    NULL,
        Estado                nvarchar(max)    NULL,
        PedidoWebId           int              NOT NULL,
        EventoId              int              NOT NULL,
        EventoTitulo          nvarchar(max)    NOT NULL,
        IngressoId            int              NOT NULL,
        Setor                 nvarchar(max)    NOT NULL,
        TipoEntrada           nvarchar(max)    NOT NULL,
        FormaPagamento        nvarchar(max)    NOT NULL CONSTRAINT DF_VendasBalcao_FormaPagamento DEFAULT 'CREDITO',
        Quantidade            int              NOT NULL,
        ValorUnitario         decimal(18,2)    NOT NULL,
        ValorTotal            decimal(18,2)    NOT NULL,
        CodigoTicket          nvarchar(450)    NOT NULL,
        DataVenda             datetime2        NOT NULL,
        CreatedAt             datetime2        NOT NULL,
        UpdatedAt             datetime2        NULL,
        AcompanhantesJson     nvarchar(max)    NULL,
        Subtipo               nvarchar(max)    NULL,
        DocumentosJson        nvarchar(max)    NULL
    );
END;

IF COL_LENGTH('VendasBalcao', 'AcompanhantesJson') IS NULL
    ALTER TABLE VendasBalcao ADD AcompanhantesJson nvarchar(max) NULL;

IF COL_LENGTH('VendasBalcao', 'FormaPagamento') IS NULL
    ALTER TABLE VendasBalcao ADD FormaPagamento nvarchar(max) NOT NULL CONSTRAINT DF_VendasBalcao_FormaPagamento DEFAULT 'CREDITO';

IF COL_LENGTH('VendasBalcao', 'Subtipo') IS NULL
    ALTER TABLE VendasBalcao ADD Subtipo nvarchar(max) NULL;

IF COL_LENGTH('VendasBalcao', 'DocumentosJson') IS NULL
    ALTER TABLE VendasBalcao ADD DocumentosJson nvarchar(max) NULL;

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_VendasBalcao_DataVenda' AND object_id = OBJECT_ID('VendasBalcao'))
    CREATE INDEX IX_VendasBalcao_DataVenda ON VendasBalcao (DataVenda);

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_VendasBalcao_CodigoTicket' AND object_id = OBJECT_ID('VendasBalcao'))
    CREATE UNIQUE INDEX IX_VendasBalcao_CodigoTicket ON VendasBalcao (CodigoTicket);
");
    }
}

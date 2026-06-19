using Microsoft.EntityFrameworkCore;
using PontoVenda.Backend.Modules.PontoVenda.Model;

namespace PontoVenda.Backend.Infra.Config;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // --- TABELAS DO PONTO DE VENDA ---
    public DbSet<Produto>       Produtos    => Set<Produto>();
    public DbSet<Venda>         Vendas      => Set<Venda>();
    public DbSet<ItemVenda>     ItensVenda  => Set<ItemVenda>();
    public DbSet<Operador>      Operadores  => Set<Operador>();

    // --- TABELAS DA BILHETERIA FÍSICA ---
    public DbSet<Evento>        Eventos         => Set<Evento>();
    public DbSet<Assento>       Assentos        => Set<Assento>();
    public DbSet<VendaFisica>   VendasFisicas   => Set<VendaFisica>();

    // --- CONFIGURAÇÃO DO MODELO ---
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // --- OPERADOR ---
        modelBuilder.Entity<Operador>()
            .HasIndex(o => o.Login)
            .IsUnique();

        // --- PRODUTO ---
        modelBuilder.Entity<Produto>()
            .HasIndex(p => p.CodigoBarras)
            .IsUnique();

        modelBuilder.Entity<Produto>()
            .Property(p => p.Preco)
            .HasPrecision(18, 2);

        // --- VENDA ---
        modelBuilder.Entity<Venda>()
            .Property(v => v.ValorTotal)
            .HasPrecision(18, 2);

        modelBuilder.Entity<ItemVenda>()
            .Property(i => i.PrecoUnitario)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Venda>()
            .HasMany(v => v.Itens)
            .WithOne()
            .HasForeignKey(i => i.VendaId)
            .OnDelete(DeleteBehavior.Cascade);

        // --- EVENTO ---
        modelBuilder.Entity<Evento>()
            .Property(e => e.Nome)
            .HasMaxLength(150)
            .IsRequired();

        modelBuilder.Entity<Evento>()
            .Property(e => e.Local)
            .HasMaxLength(200)
            .IsRequired();

        modelBuilder.Entity<Evento>()
            .HasIndex(e => e.Nome);

        modelBuilder.Entity<Evento>()
            .HasMany(e => e.Assentos)
            .WithOne()
            .HasForeignKey(a => a.EventoId)
            .OnDelete(DeleteBehavior.Cascade);

        // --- ASSENTO ---
        modelBuilder.Entity<Assento>()
            .Property(a => a.Preco)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Assento>()
            .Property(a => a.Status)
            .HasMaxLength(20)
            .IsRequired();

        modelBuilder.Entity<Assento>()
            .HasIndex(a => new { a.EventoId, a.Setor, a.Fileira, a.Numero })
            .IsUnique();

        // --- VENDA FÍSICA ---
        modelBuilder.Entity<VendaFisica>()
            .Property(v => v.Valor)
            .HasPrecision(18, 2);

        modelBuilder.Entity<VendaFisica>()
            .Property(v => v.MetodoPagamento)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        modelBuilder.Entity<VendaFisica>()
            .HasIndex(v => v.DataVenda);

        modelBuilder.Entity<VendaFisica>()
            .HasIndex(v => v.EventoId);

        // Um assento só pode ser vendido uma única vez
        modelBuilder.Entity<VendaFisica>()
            .HasIndex(v => v.AssentoId)
            .IsUnique();
    }
}

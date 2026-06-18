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
    public DbSet<Evento>            Eventos             => Set<Evento>();
    public DbSet<Assento>           Assentos            => Set<Assento>();
    public DbSet<VendaFisica>       VendasFisicas       => Set<VendaFisica>();
    public DbSet<ItemVendaFisica>   ItensVendaFisica    => Set<ItemVendaFisica>();

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
            .Property(v => v.ValorTotal)
            .HasPrecision(18, 2);

        modelBuilder.Entity<VendaFisica>()
            .Property(v => v.FormaPagamento)
            .HasMaxLength(30)
            .IsRequired();

        modelBuilder.Entity<VendaFisica>()
            .Property(v => v.Status)
            .HasMaxLength(20)
            .IsRequired();

        modelBuilder.Entity<VendaFisica>()
            .Property(v => v.NomeComprador)
            .HasMaxLength(150);

        modelBuilder.Entity<VendaFisica>()
            .Property(v => v.CpfComprador)
            .HasMaxLength(14);

        modelBuilder.Entity<VendaFisica>()
            .HasIndex(v => v.DataVenda);

        modelBuilder.Entity<VendaFisica>()
            .HasIndex(v => v.OperadorId);

        modelBuilder.Entity<VendaFisica>()
            .HasIndex(v => v.EventoId);

        modelBuilder.Entity<VendaFisica>()
            .HasMany(v => v.Itens)
            .WithOne()
            .HasForeignKey(i => i.VendaFisicaId)
            .OnDelete(DeleteBehavior.Cascade);

        // --- ITEM VENDA FÍSICA ---
        modelBuilder.Entity<ItemVendaFisica>()
            .Property(i => i.Preco)
            .HasPrecision(18, 2);

        modelBuilder.Entity<ItemVendaFisica>()
            .HasIndex(i => i.AssentoId)
            .IsUnique();
    }
}

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

    // --- CONFIGURAÇÃO DO MODELO ---
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Operador>()
            .HasIndex(o => o.Login)
            .IsUnique();

        modelBuilder.Entity<Produto>()
            .HasIndex(p => p.CodigoBarras)
            .IsUnique();

        modelBuilder.Entity<Produto>()
            .Property(p => p.Preco)
            .HasPrecision(18, 2);

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
    }
}

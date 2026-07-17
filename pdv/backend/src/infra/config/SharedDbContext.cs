using Microsoft.EntityFrameworkCore;
using PontoVenda.Backend.Modules.Balcao.Model;

namespace PontoVenda.Backend.Infra.Config;

public class SharedDbContext : DbContext
{
    public SharedDbContext(DbContextOptions<SharedDbContext> options) : base(options)
    {
    }

    public DbSet<ClienteWeb>    Clientes    => Set<ClienteWeb>();
    public DbSet<EventoWeb>     Eventos     => Set<EventoWeb>();
    public DbSet<IngressoWeb>   Ingressos   => Set<IngressoWeb>();
    public DbSet<PedidoWeb>     Pedidos     => Set<PedidoWeb>();
    public DbSet<ItemPedidoWeb> ItensPedido => Set<ItemPedidoWeb>();
    public DbSet<PagamentoWeb>  Pagamentos  => Set<PagamentoWeb>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // --- MAPEAMENTO EXATO PARA AS TABELAS DO WEB ---
        modelBuilder.Entity<ClienteWeb>().ToTable("Usuarios");
        modelBuilder.Entity<EventoWeb>().ToTable("Eventos");
        modelBuilder.Entity<IngressoWeb>().ToTable("Ingressos");
        modelBuilder.Entity<PedidoWeb>().ToTable("Pedidos");
        modelBuilder.Entity<ItemPedidoWeb>().ToTable("ItensPedido");
        modelBuilder.Entity<PagamentoWeb>().ToTable("Pagamentos");

        // --- PRECISAO DECIMAL ---
        modelBuilder.Entity<IngressoWeb>().Property(i => i.Preco).HasPrecision(18, 2);
        modelBuilder.Entity<ItemPedidoWeb>().Property(i => i.PrecoUnitario).HasPrecision(18, 2);
        modelBuilder.Entity<PedidoWeb>().Property(p => p.ValorTotal).HasPrecision(18, 2);

        modelBuilder.Entity<PedidoWeb>()
            .HasMany(p => p.Itens)
            .WithOne()
            .HasForeignKey(i => i.PedidoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

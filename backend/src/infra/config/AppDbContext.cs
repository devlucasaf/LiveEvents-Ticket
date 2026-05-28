using LiveEventsTicket.Backend.Modules.Evento.Model;
using LiveEventsTicket.Backend.Modules.Ingresso.Model;
using LiveEventsTicket.Backend.Modules.Pagamento.Model;
using LiveEventsTicket.Backend.Modules.Pedido.Model;
using LiveEventsTicket.Backend.Modules.Usuario.Model;
using Microsoft.EntityFrameworkCore;

namespace LiveEventsTicket.Backend.Infra.Config;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario>       Usuarios    => Set<Usuario>();
    public DbSet<Evento>        Eventos     => Set<Evento>();
    public DbSet<Ingresso>      Ingressos   => Set<Ingresso>();
    public DbSet<Pedido>        Pedidos     => Set<Pedido>();
    public DbSet<ItemPedido>    ItensPedido => Set<ItemPedido>();
    public DbSet<Pagamento>     Pagamentos  => Set<Pagamento>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Pedido>()
            .HasMany(p => p.Itens)
            .WithOne()
            .HasForeignKey(i => i.PedidoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

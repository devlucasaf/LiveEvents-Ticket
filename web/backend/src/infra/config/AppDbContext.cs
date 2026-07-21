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

    public DbSet<Usuario>           Usuarios    => Set<Usuario>();
    public DbSet<Evento>            Eventos     => Set<Evento>();
    public DbSet<Ingresso>          Ingressos   => Set<Ingresso>();
    public DbSet<Pedido>            Pedidos     => Set<Pedido>();
    public DbSet<ItemPedido>        ItensPedido => Set<ItemPedido>();
    public DbSet<PedidoCheckinLog>  PedidoCheckinLogs => Set<PedidoCheckinLog>();
    public DbSet<Pagamento>         Pagamentos  => Set<Pagamento>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Ingresso>()
            .Property(i => i.Preco)
            .HasPrecision(18, 2);

        modelBuilder.Entity<ItemPedido>()
            .Property(i => i.PrecoUnitario)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Pedido>()
            .Property(p => p.ValorTotal)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Pedido>()
            .HasMany(p => p.Itens)
            .WithOne()
            .HasForeignKey(i => i.PedidoId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PedidoCheckinLog>()
            .Property(c => c.TokenInformado)
            .HasMaxLength(300);

        modelBuilder.Entity<PedidoCheckinLog>()
            .Property(c => c.Mensagem)
            .HasMaxLength(500);

        modelBuilder.Entity<PedidoCheckinLog>()
            .HasOne<Pedido>()
            .WithMany()
            .HasForeignKey(c => c.PedidoId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

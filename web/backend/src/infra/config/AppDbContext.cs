using LiveEventsTicket.Backend.Modules.Evento.Model;
using LiveEventsTicket.Backend.Modules.Ingresso.Model;
using LiveEventsTicket.Backend.Modules.Pagamento.Model;
using LiveEventsTicket.Backend.Modules.Pedido.Model;
using LiveEventsTicket.Backend.Modules.Usuario.Model;
using Microsoft.EntityFrameworkCore;

namespace LiveEventsTicket.Backend.Infra.Config;

public class AppDbContext : DbContext
{
    // --- RECEBE AS OPCOES DE CONFIGURACAO E REPASSA PARA O DBCONTEXT BASE ---
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario>           Usuarios            => Set<Usuario>();
    public DbSet<Evento>            Eventos             => Set<Evento>();
    public DbSet<Ingresso>          Ingressos           => Set<Ingresso>();
    public DbSet<Pedido>            Pedidos             => Set<Pedido>();
    public DbSet<ItemPedido>        ItensPedido         => Set<ItemPedido>();
    public DbSet<PedidoCheckinLog>  PedidoCheckinLogs   => Set<PedidoCheckinLog>();
    public DbSet<Pagamento>         Pagamentos          => Set<Pagamento>();

    // --- CONFIGURA MAPEAMENTOS, INDICES E RELACIONAMENTOS DAS ENTIDADES ---
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // --- EMAIL DO USUARIO DEVE SER UNICO ---
        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // --- PRECISAO DECIMAL PARA O PRECO DO INGRESSO ---
        modelBuilder.Entity<Ingresso>()
            .Property(i => i.Preco)
            .HasPrecision(18, 2);

        // --- PRECISAO DECIMAL PARA O PRECO UNITARIO DO ITEM ---
        modelBuilder.Entity<ItemPedido>()
            .Property(i => i.PrecoUnitario)
            .HasPrecision(18, 2);

        // --- PRECISAO DECIMAL PARA O VALOR TOTAL DO PEDIDO ---
        modelBuilder.Entity<Pedido>()
            .Property(p => p.ValorTotal)
            .HasPrecision(18, 2);

        // --- ITENS (1:N) COM EXCLUSAO EM CASCATA ---
        modelBuilder.Entity<Pedido>()
            .HasMany(p => p.Itens)
            .WithOne()
            .HasForeignKey(i => i.PedidoId)
            .OnDelete(DeleteBehavior.Cascade);

        // --- LIMITE DE TAMANHO DO TOKEN INFORMADO NO LOG DE CHECKIN ---
        modelBuilder.Entity<PedidoCheckinLog>()
            .Property(c => c.TokenInformado)
            .HasMaxLength(300);

        // --- LIMITE DE TAMANHO DA MENSAGEM NO LOG DE CHECKIN ---
        modelBuilder.Entity<PedidoCheckinLog>()
            .Property(c => c.Mensagem)
            .HasMaxLength(500);

        // --- RELACAO LOG DE CHECKIN ---
        modelBuilder.Entity<PedidoCheckinLog>()
            .HasOne<Pedido>()
            .WithMany()
            .HasForeignKey(c => c.PedidoId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

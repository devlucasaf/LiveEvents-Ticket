using Microsoft.EntityFrameworkCore;
using Backend.src.Entity;

namespace Backend.src.Infra.Config
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<Evento> Eventos => Set<Evento>();
        public DbSet<SetorIngresso> Setores => Set<SetorIngresso>();
        public DbSet<Ingresso> Ingressos => Set<Ingresso>();
        public DbSet<Pedido> Pedidos => Set<Pedido>();
        public DbSet<Pagamento> Pagamentos => Set<Pagamento>();

        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<Usuario>().HasIndex(u => u.Email).IsUnique();

            mb.Entity<SetorIngresso>().Property(s => s.Preco).HasColumnType("decimal(18,2)");
            mb.Entity<Pedido>().Property(p => p.Total).HasColumnType("decimal(18,2)");

            mb.Entity<Pagamento>()
                .HasOne(p => p.Pedido)
                .WithOne(p => p.Pagamento!)
                .HasForeignKey<Pagamento>(p => p.PedidoId);

            base.OnModelCreating(mb);
        }
    }
}
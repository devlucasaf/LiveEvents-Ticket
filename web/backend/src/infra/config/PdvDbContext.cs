using LiveEventsTicket.Backend.Modules.Admin.Model;
using Microsoft.EntityFrameworkCore;

namespace LiveEventsTicket.Backend.Infra.Config;

public class PdvDbContext : DbContext
{
    // --- RECEBE AS OPCOES DE CONEXAO VIA INJECAO ---
    public PdvDbContext(DbContextOptions<PdvDbContext> options) : base(options)
    {
    }

    // --- FUNCIONARIOS ---
    public DbSet<Funcionario> Funcionarios => Set<Funcionario>();

    // --- MAPEIA A ENTIDADE Funcionario PARA A TABELA EXISTENTE "Operadores" ---
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Funcionario>().ToTable("Operadores");
    }
}

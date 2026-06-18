using LiveEventsTicket.Backend.Infra.Config;
using Microsoft.EntityFrameworkCore;
using UsuarioEntity = LiveEventsTicket.Backend.Modules.Usuario.Model.Usuario;

namespace LiveEventsTicket.Backend.Modules.Usuario.Repository;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly AppDbContext _context;

    public UsuarioRepository(AppDbContext context)
    {
        _context = context;
    }

    // --- ADICIONAR NOVO USUÁRIO E SALVAR NO BANCO ---
    public async Task AdicionarAsync(UsuarioEntity usuario, CancellationToken cancellationToken = default)
    {
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync(cancellationToken);
    }

    // --- ATUALIZAR USUÁRIO EXISTENTE E SALVAR NO BANCO ---
    public async Task AtualizarAsync(UsuarioEntity usuario, CancellationToken cancellationToken = default)
    {
        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync(cancellationToken);
    }

    // --- BUSCAR USUÁRIO POR EMAIL ---
    public Task<UsuarioEntity?> BuscarPorEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    // --- BUSCAR USUÁRIO POR ID ---
    public Task<UsuarioEntity?> BuscarPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    // --- LISTAR TODOS OS USUÁRIOS ---
    public Task<List<UsuarioEntity>> ListarTodosAsync(CancellationToken cancellationToken = default)
    {
        return _context.Usuarios.ToListAsync(cancellationToken);
    }
}

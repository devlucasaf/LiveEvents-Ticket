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

    public async Task AdicionarAsync(UsuarioEntity usuario, CancellationToken cancellationToken = default)
    {
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public Task<UsuarioEntity?> BuscarPorEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public Task<UsuarioEntity?> BuscarPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }
}

using UsuarioEntity = LiveEventsTicket.Backend.Modules.Usuario.Model.Usuario;

namespace LiveEventsTicket.Backend.Modules.Usuario.Repository;

public interface IUsuarioRepository
{
    Task AdicionarAsync(UsuarioEntity usuario, CancellationToken cancellationToken = default);
    Task<UsuarioEntity?> BuscarPorEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<UsuarioEntity?> BuscarPorIdAsync(int id, CancellationToken cancellationToken = default);
}

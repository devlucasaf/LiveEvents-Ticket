using PontoVenda.Backend.Modules.PontoVenda.Model;

namespace PontoVenda.Backend.Modules.PontoVenda.Repository;

public interface IOperadorRepository
{
    Task<Operador?> BuscarPorLoginAsync(string login, CancellationToken cancellationToken = default);
    Task<Operador?> BuscarPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task            AdicionarAsync(Operador operador, CancellationToken cancellationToken = default);
}

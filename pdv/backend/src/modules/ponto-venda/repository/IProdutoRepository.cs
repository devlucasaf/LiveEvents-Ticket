using PontoVenda.Backend.Modules.PontoVenda.Model;

namespace PontoVenda.Backend.Modules.PontoVenda.Repository;

public interface IProdutoRepository
{
    Task<List<Produto>>     ListarAsync(CancellationToken cancellationToken = default);
    Task<Produto?>          BuscarPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Produto?>          BuscarPorCodigoBarrasAsync(string codigoBarras, CancellationToken cancellationToken = default);
    Task                    AdicionarAsync(Produto produto, CancellationToken cancellationToken = default);
    Task                    AtualizarAsync(CancellationToken cancellationToken = default);
}

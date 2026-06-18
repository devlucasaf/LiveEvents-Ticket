using PontoVenda.Backend.Modules.PontoVenda.Model;

namespace PontoVenda.Backend.Modules.PontoVenda.Repository;

public interface IVendaRepository
{
    Task                AdicionarAsync(Venda venda, CancellationToken cancellationToken = default);
    Task                SalvarAlteracoesAsync(CancellationToken cancellationToken = default);
    Task<List<Venda>>   ListarAsync(CancellationToken cancellationToken = default);
    Task<Venda?>        BuscarPorIdAsync(int id, CancellationToken cancellationToken = default);
}

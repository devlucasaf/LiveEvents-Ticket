using PontoVenda.Backend.Modules.PontoVenda.Model;

namespace PontoVenda.Backend.Modules.PontoVenda.Repository;

public interface IVendaFisicaRepository
{
    Task AdicionarAsync(VendaFisica venda, CancellationToken cancellationToken = default);
    Task SalvarAlteracoesAsync(CancellationToken cancellationToken = default);
    Task<List<VendaFisica>> ListarAsync(CancellationToken cancellationToken = default);
    Task<List<VendaFisica>> ListarPorEventoAsync(Guid eventoId, CancellationToken cancellationToken = default);
    Task<List<VendaFisica>> ListarPorOperadorAsync(int operadorId, CancellationToken cancellationToken = default);
    Task<VendaFisica?> BuscarPorIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> AssentoJaVendidoAsync(Guid assentoId, CancellationToken cancellationToken = default);
}

using LiveEventsTicket.Backend.Modules.Evento.Repository;
using LiveEventsTicket.Backend.Modules.Ingresso.Repository;
using LiveEventsTicket.Backend.Modules.Pedido.Dto;
using LiveEventsTicket.Backend.Modules.Pedido.Repository;

using IngressoEntity = LiveEventsTicket.Backend.Modules.Ingresso.Model.Ingresso;

namespace LiveEventsTicket.Backend.Modules.Pedido.Service;

public class PedidoConsultaService
{
    private readonly IEventoRepository _eventoRepository;
    private readonly IIngressoRepository _ingressoRepository;
    private readonly IPedidoRepository _pedidoRepository;

    public PedidoConsultaService(
        IEventoRepository eventoRepository,
        IIngressoRepository ingressoRepository,
        IPedidoRepository pedidoRepository)
    {
        _eventoRepository = eventoRepository;
        _ingressoRepository = ingressoRepository;
        _pedidoRepository = pedidoRepository;
    }

    // --- LISTA PEDIDOS DO USUARIO ENRIQUECENDO COM DADOS DE EVENTO / REEMBOLSO / COMPARTILHAMENTO ---
    public async Task<List<PedidoRespostaDto>> ListarPorUsuarioAsync(int usuarioId, CancellationToken cancellationToken = default)
    {
        var pedidos = await _pedidoRepository.ListarPorUsuarioAsync(usuarioId, cancellationToken);
        var agora   = DateTime.UtcNow;

        // --- CARREGA OS INGRESSOS REFERENCIADOS DE UMA SO VEZ ---
        var ingressoIds = pedidos.SelectMany(p => p.Itens).Select(i => i.IngressoId).Distinct().ToList();
        var ingressos   = new Dictionary<int, IngressoEntity>();
        foreach (var id in ingressoIds)
        {
            var ing = await _ingressoRepository.BuscarPorIdAsync(id, cancellationToken);
            if (ing is not null)
            {
                ingressos[id] = ing;
            }
        }

        var resposta = new List<PedidoRespostaDto>(pedidos.Count);

        // --- PROCESSA CADA PEDIDO EM SEQUENCIA PARA EVITAR CONCORRENCIA NO DBCONTEXT ---
        foreach (var p in pedidos)
        {
            var primeiroItem = p.Itens.FirstOrDefault();
            IngressoEntity? ingresso = null;
            if (primeiroItem is not null && ingressos.TryGetValue(primeiroItem.IngressoId, out var ing))
            {
                ingresso = ing;
            }

            // --- CARREGA A DATA DO EVENTO QUANDO POSSIVEL PARA AVALIAR REEMBOLSO ---
            DateTime? dataEvento = null;
            if (ingresso is not null)
            {
                var evento = await _eventoRepository.BuscarPorIdAsync(ingresso.EventoId, cancellationToken);
                dataEvento = evento?.DataEvento;
            }

            // --- REGRAS DE REEMBOLSO AGORA CENTRALIZADAS ---
            var (reembolsoElegivel, reembolsoMensagem, regraReembolso) = ReembolsoRegras.AvaliarElegibilidade(p, dataEvento, agora);

            var pagamento = await _pedidoRepository.BuscarPagamentoPorPedidoIdAsync(p.Id, cancellationToken);

            DateTime? reembolsoEstornadoEm = pagamento?.Status == StatusPagamento.Reembolsado
                ? pagamento.DataPagamento
                : null;

            var protocoloEstorno = PedidoHelpers.GerarProtocoloEstorno(p.Id, reembolsoEstornadoEm);

            resposta.Add(new PedidoRespostaDto
            {
                Id                                = p.Id,
                ValorTotal                        = p.ValorTotal,
                Status                            = p.Status,
                QrCodeBase64                      = p.QrCodeBase64,
                PagamentoStatus                   = pagamento?.Status ?? (p.Status == StatusPedido.Pago
                                                                            ? StatusPagamento.Aprovado
                                                                            : StatusPagamento.Recusado),
                DataCriacao                       = p.DataCriacao,
                EventoId                          = ingresso?.EventoId,
                IngressoId                        = primeiroItem?.IngressoId,
                Setor                             = ingresso?.Setor,
                Quantidade                        = primeiroItem?.Quantidade ?? 0,
                CompartilhamentoToken             = p.CompartilhamentoToken,
                CompartilhamentoExpiraEm          = p.CompartilhamentoExpiraEm,
                CompartilhamentoRevogadoEm        = p.CompartilhamentoRevogadoEm,
                CompartilhamentoMaxAcessos        = p.CompartilhamentoMaxAcessos,
                CompartilhamentoAcessosRealizados = p.CompartilhamentoAcessosRealizados,
                CompartilhamentoAtivo             = PedidoHelpers.CompartilhamentoAtivo(p, agora),
                ReembolsoSolicitadoEm             = p.ReembolsoSolicitadoEm,
                ReembolsoAprovadoEm               = p.ReembolsoAprovadoEm,
                ReembolsoEstornadoEm              = reembolsoEstornadoEm,
                ReembolsoMotivo                   = p.ReembolsoMotivo,
                ReembolsoMotivoCodigo             = p.ReembolsoMotivoCodigo,
                ReembolsoRegraAplicada            = p.ReembolsoRegraAplicada,
                ReembolsoElegivel                 = reembolsoElegivel,
                ReembolsoMensagem                 = reembolsoMensagem ?? regraReembolso,
                ReembolsoProtocoloEstorno         = protocoloEstorno
            });
        }

        return resposta;
    }
}

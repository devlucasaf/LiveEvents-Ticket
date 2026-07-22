using LiveEventsTicket.Backend.Modules.Pedido.Dto;
using LiveEventsTicket.Backend.Modules.Pedido.Repository;

namespace LiveEventsTicket.Backend.Modules.Pedido.Service;

public class IngressoCompartilhamentoService
{
    private readonly IPedidoRepository _pedidoRepository;

    public IngressoCompartilhamentoService(IPedidoRepository pedidoRepository)
    {
        _pedidoRepository = pedidoRepository;
    }

    // --- GERA LINK COMPARTILHAVEL COM VALIDADE E LIMITE DE ACESSOS ---
    public async Task<CompartilhamentoIngressoRespostaDto> GerarAsync(
        int usuarioId,
        int pedidoId,
        CriarCompartilhamentoIngressoDto dto,
        CancellationToken cancellationToken = default)
    {
        var pedido = await _pedidoRepository.BuscarPorIdEUsuarioAsync(pedidoId, usuarioId, cancellationToken)
            ?? throw new KeyNotFoundException("Pedido não encontrado para este usuário.");

        // --- SO PEDIDOS PAGOS PODEM SER COMPARTILHADOS ---
        if (pedido.Status != StatusPedido.Pago)
        {
            throw new InvalidOperationException("Só é possível compartilhar ingressos de pedidos pagos.");
        }

        // --- APLICA REGRAS DE VALIDADE (LIMITADAS PELO INTERVALO CONFIGURADO) ---
        var validadeMinutos = PedidoHelpers.NormalizarIntervalo(
            dto.ValidadeMinutos ?? RegrasCompartilhamento.ValidadePadraoMinutos,
            RegrasCompartilhamento.ValidadeMinMinutos,
            RegrasCompartilhamento.ValidadeMaxMinutos);

        var maxAcessos = PedidoHelpers.NormalizarIntervalo(
            dto.MaxAcessos ?? RegrasCompartilhamento.MaxAcessosPadrao,
            RegrasCompartilhamento.MaxAcessosMin,
            RegrasCompartilhamento.MaxAcessosMax);

        // --- ATUALIZA OS CAMPOS DE COMPARTILHAMENTO DO PEDIDO ---
        pedido.CompartilhamentoToken               = PedidoHelpers.GerarTokenCompartilhamento();
        pedido.CompartilhamentoExpiraEm            = DateTime.UtcNow.AddMinutes(validadeMinutos);
        pedido.CompartilhamentoRevogadoEm          = null;
        pedido.CompartilhamentoMaxAcessos          = maxAcessos;
        pedido.CompartilhamentoAcessosRealizados   = 0;

        await _pedidoRepository.SalvarAlteracoesAsync(cancellationToken);

        return new CompartilhamentoIngressoRespostaDto
        {
            PedidoId          = pedido.Id,
            Token             = pedido.CompartilhamentoToken,
            ExpiraEm          = pedido.CompartilhamentoExpiraEm.Value,
            RevogadoEm        = pedido.CompartilhamentoRevogadoEm,
            MaxAcessos        = pedido.CompartilhamentoMaxAcessos,
            AcessosRealizados = pedido.CompartilhamentoAcessosRealizados,
            Ativo             = true,
            Mensagem          = "Link de compartilhamento gerado com sucesso."
        };
    }

    // --- REVOGA UM LINK DE COMPARTILHAMENTO EM VIGOR ---
    public async Task<CompartilhamentoIngressoRespostaDto> RevogarAsync(
        int usuarioId,
        int pedidoId,
        CancellationToken cancellationToken = default)
    {
        // --- CARREGA O PEDIDO GARANTINDO PROPRIETARIO ---
        var pedido = await _pedidoRepository.BuscarPorIdEUsuarioAsync(pedidoId, usuarioId, cancellationToken)
            ?? throw new KeyNotFoundException("Pedido não encontrado para este usuário.");

        if (string.IsNullOrWhiteSpace(pedido.CompartilhamentoToken))
        {
            throw new InvalidOperationException("Este pedido ainda não possui link de compartilhamento ativo.");
        }

        // --- MARCA A REVOGACAO NO INSTANTE ATUAL ---
        pedido.CompartilhamentoRevogadoEm = DateTime.UtcNow;
        await _pedidoRepository.SalvarAlteracoesAsync(cancellationToken);

        return new CompartilhamentoIngressoRespostaDto
        {
            PedidoId          = pedido.Id,
            Token             = pedido.CompartilhamentoToken,
            ExpiraEm          = pedido.CompartilhamentoExpiraEm ?? DateTime.UtcNow,
            RevogadoEm        = pedido.CompartilhamentoRevogadoEm,
            MaxAcessos        = pedido.CompartilhamentoMaxAcessos,
            AcessosRealizados = pedido.CompartilhamentoAcessosRealizados,
            Ativo             = false,
            Mensagem          = "Link de compartilhamento revogado com sucesso."
        };
    }
}

using LiveEventsTicket.Backend.Modules.Pedido.Dto;

namespace LiveEventsTicket.Backend.Modules.Pedido.Service;

public class PedidoService
{
    private readonly PedidoCriacaoService _criacao;
    private readonly PedidoConsultaService _consulta;
    private readonly ReembolsoService _reembolso;
    private readonly IngressoPdfService _ingressoPdf;
    private readonly IngressoCompartilhamentoService _compartilhamento;
    private readonly CheckinService _checkin;

    public PedidoService(
        PedidoCriacaoService criacao,
        PedidoConsultaService consulta,
        ReembolsoService reembolso,
        IngressoPdfService ingressoPdf,
        IngressoCompartilhamentoService compartilhamento,
        CheckinService checkin)
    {
        _criacao          = criacao;
        _consulta         = consulta;
        _reembolso        = reembolso;
        _ingressoPdf      = ingressoPdf;
        _compartilhamento = compartilhamento;
        _checkin          = checkin;
    }

    // --- CRIAR PEDIDO ---
    public Task<PedidoRespostaDto> CriarAsync(int usuarioId, CriarPedidoDto dto, CancellationToken cancellationToken = default)
        => _criacao.CriarAsync(usuarioId, dto, cancellationToken);

    // --- LISTAR PEDIDOS DO USUARIO ---
    public Task<List<PedidoRespostaDto>> ListarPorUsuarioAsync(int usuarioId, CancellationToken cancellationToken = default)
        => _consulta.ListarPorUsuarioAsync(usuarioId, cancellationToken);

    // --- SOLICITAR REEMBOLSO AUTOMATICO ---
    public Task<ReembolsoRespostaDto> SolicitarReembolsoAsync(
        int usuarioId,
        int pedidoId,
        SolicitarReembolsoDto dto,
        CancellationToken cancellationToken = default)
        => _reembolso.SolicitarAsync(usuarioId, pedidoId, dto, cancellationToken);

    // --- COMPROVANTE DE ESTORNO EM PDF ---
    public Task<(byte[] arquivo, string nomeArquivo)> GerarComprovanteEstornoPdfAsync(
        int usuarioId,
        int pedidoId,
        CancellationToken cancellationToken = default)
        => _reembolso.GerarComprovanteEstornoPdfAsync(usuarioId, pedidoId, cancellationToken);

    // --- GERAR LINK DE COMPARTILHAMENTO ---
    public Task<CompartilhamentoIngressoRespostaDto> GerarCompartilhamentoIngressoAsync(
        int usuarioId,
        int pedidoId,
        CriarCompartilhamentoIngressoDto dto,
        CancellationToken cancellationToken = default)
        => _compartilhamento.GerarAsync(usuarioId, pedidoId, dto, cancellationToken);

    // --- REVOGAR LINK DE COMPARTILHAMENTO ---
    public Task<CompartilhamentoIngressoRespostaDto> RevogarCompartilhamentoIngressoAsync(
        int usuarioId,
        int pedidoId,
        CancellationToken cancellationToken = default)
        => _compartilhamento.RevogarAsync(usuarioId, pedidoId, cancellationToken);

    // --- PDF DO INGRESSO PARA O DONO ---
    public Task<(byte[] arquivo, string nomeArquivo)> GerarIngressoPdfAsync(
        int usuarioId,
        int pedidoId,
        CancellationToken cancellationToken = default)
        => _ingressoPdf.GerarPeloDonoAsync(usuarioId, pedidoId, cancellationToken);

    // --- PDF COMPARTILHADO VIA TOKEN PUBLICO ---
    public Task<(byte[] arquivo, string nomeArquivo)> BaixarIngressoPdfCompartilhadoAsync(
        string tokenCompartilhamento,
        CancellationToken cancellationToken = default)
        => _ingressoPdf.BaixarCompartilhadoAsync(tokenCompartilhamento, cancellationToken);

    // --- VALIDACAO DE CHECKIN ---
    public Task<CheckinRespostaDto> ValidarCheckinAsync(
        int operadorId,
        ValidarCheckinDto dto,
        CancellationToken cancellationToken = default)
        => _checkin.ValidarAsync(operadorId, dto, cancellationToken);
}

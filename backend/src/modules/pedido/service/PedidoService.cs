using LiveEventsTicket.Backend.Modules.Ingresso.Repository;
using LiveEventsTicket.Backend.Modules.Pagamento.Service;
using LiveEventsTicket.Backend.Modules.Pedido.Dto;
using LiveEventsTicket.Backend.Modules.Pedido.Repository;
using QRCoder;
using ItemPedido = LiveEventsTicket.Backend.Modules.Pedido.Model.ItemPedido;
using PedidoEntity = LiveEventsTicket.Backend.Modules.Pedido.Model.Pedido;

namespace LiveEventsTicket.Backend.Modules.Pedido.Service;

public class PedidoService
{
    private readonly IIngressoRepository    _ingressoRepository;
    private readonly IPedidoRepository      _pedidoRepository;
    private readonly PagamentoService       _pagamentoService;

    public PedidoService(
        IIngressoRepository ingressoRepository,
        IPedidoRepository pedidoRepository,
        PagamentoService pagamentoService)
    {
        _ingressoRepository = ingressoRepository;
        _pedidoRepository = pedidoRepository;
        _pagamentoService = pagamentoService;
    }

    // --- CRIAR PEDIDO COM VALIDAÇÃO ---
    public async Task<PedidoRespostaDto> CriarAsync(int usuarioId, CriarPedidoDto dto, CancellationToken cancellationToken = default)
    {
        // --- VALIDAR QUE O PEDIDO TEM AO MENOS UM ITEM ---
        if (dto.Itens.Count == 0)
        {
            throw new InvalidOperationException("Pedido deve conter ao menos um ingresso.");
        }

        var pedido = new PedidoEntity { UsuarioId = usuarioId };

        // --- PROCESSAR CADA ITEM DO PEDIDO ---
        foreach (var item in dto.Itens)
        {
            var ingresso = await _ingressoRepository.BuscarPorIdAsync(item.IngressoId, cancellationToken)
                ?? throw new KeyNotFoundException($"Ingresso {item.IngressoId} não encontrado.");

            if (item.Quantidade <= 0 || ingresso.QuantidadeDisponivel < item.Quantidade)
            {
                throw new InvalidOperationException($"Quantidade indisponível para ingresso {item.IngressoId}.");
            }

            ingresso.QuantidadeDisponivel -= item.Quantidade;

            pedido.Itens.Add(new ItemPedido
            {
                IngressoId = ingresso.Id,
                Quantidade = item.Quantidade,
                PrecoUnitario = ingresso.Preco
            });
        }

        // --- CALCULAR VALOR TOTAL DO PEDIDO ---
        pedido.ValorTotal = pedido.Itens.Sum(i => i.Quantidade * i.PrecoUnitario);

        // --- SALVAR PEDIDO E ATUALIZAR ESTOQUE DE INGRESSOS ---
        await _pedidoRepository.AdicionarPedidoAsync(pedido, cancellationToken);
        await _ingressoRepository.AtualizarAsync(cancellationToken);

        // --- PROCESSAR PAGAMENTO ---
        var pagamento = _pagamentoService.ProcessarPagamento(dto, pedido.Id);
        await _pedidoRepository.AdicionarPagamentoAsync(pagamento, cancellationToken);

        // --- ATUALIZAR STATUS DO PEDIDO E GERAR QR CODE SE APROVADO ---
        pedido.Status = pagamento.Status == "APROVADO" ? "PAGO" : "PAGAMENTO_RECUSADO";
        if (pagamento.Status == "APROVADO")
        {
            pedido.QrCodeBase64 = GerarQrCodeBase64($"pedido:{pedido.Id}|usuario:{pedido.UsuarioId}|valor:{pedido.ValorTotal}");
        }

        await _pedidoRepository.SalvarAlteracoesAsync(cancellationToken);

        return new PedidoRespostaDto
        {
            Id = pedido.Id,
            ValorTotal = pedido.ValorTotal,
            Status = pedido.Status,
            QrCodeBase64 = pedido.QrCodeBase64,
            PagamentoStatus = pagamento.Status,
            CodigoPix = pagamento.CodigoPix
        };
    }

    // --- LISTAR PEDIDOS DO USUÁRIO E MAPEAR PARA DTO ---
    public async Task<List<PedidoRespostaDto>> ListarPorUsuarioAsync(int usuarioId, CancellationToken cancellationToken = default)
    {
        var pedidos = await _pedidoRepository.ListarPorUsuarioAsync(usuarioId, cancellationToken);

        return pedidos.Select(p => new PedidoRespostaDto
        {
            Id = p.Id,
            ValorTotal = p.ValorTotal,
            Status = p.Status,
            QrCodeBase64 = p.QrCodeBase64,
            PagamentoStatus = p.Status.StartsWith("PAGAMENTO") ? "RECUSADO" : "APROVADO"
        }).ToList();
    }

    // --- GERAR QR CODE EM BASE64 A PARTIR DO PAYLOAD ---
    private static string GerarQrCodeBase64(string payload)
    {
        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
        var qrCode = new PngByteQRCode(data);
        var bytes = qrCode.GetGraphic(20);
        return Convert.ToBase64String(bytes);
    }
}

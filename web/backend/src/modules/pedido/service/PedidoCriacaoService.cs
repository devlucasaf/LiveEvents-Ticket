using LiveEventsTicket.Backend.Modules.Ingresso.Model;
using LiveEventsTicket.Backend.Modules.Ingresso.Repository;
using LiveEventsTicket.Backend.Modules.Pagamento.Service;
using LiveEventsTicket.Backend.Modules.Pedido.Dto;
using LiveEventsTicket.Backend.Modules.Pedido.Repository;

using ItemPedido = LiveEventsTicket.Backend.Modules.Pedido.Model.ItemPedido;
using PedidoEntity = LiveEventsTicket.Backend.Modules.Pedido.Model.Pedido;

namespace LiveEventsTicket.Backend.Modules.Pedido.Service;

public class PedidoCriacaoService
{
    private readonly IIngressoRepository _ingressoRepository;
    private readonly IPedidoRepository _pedidoRepository;
    private readonly PagamentoService _pagamentoService;

    public PedidoCriacaoService(
        IIngressoRepository ingressoRepository,
        IPedidoRepository pedidoRepository,
        PagamentoService pagamentoService)
    {
        _ingressoRepository = ingressoRepository;
        _pedidoRepository   = pedidoRepository;
        _pagamentoService   = pagamentoService;
    }

    // --- CRIAR PEDIDO COM VALIDACAO DE ESTOQUE, MODALIDADE E PROCESSAMENTO DE PAGAMENTO ---
    public async Task<PedidoRespostaDto> CriarAsync(int usuarioId, CriarPedidoDto dto, CancellationToken cancellationToken = default)
    {
        if (dto.Itens.Count == 0)
        {
            throw new InvalidOperationException("Pedido deve conter ao menos um ingresso.");
        }

        ValidarDadosComprador(dto.Comprador);

        // --- MONTA O PEDIDO NORMALIZANDO STRINGS ---
        var pedido = new PedidoEntity
        {
            UsuarioId                   = usuarioId,
            CompradorNome               = dto.Comprador.Nome.Trim(),
            CompradorCpf                = PedidoHelpers.SomenteDigitos(dto.Comprador.Cpf),
            CompradorEmail              = dto.Comprador.Email.Trim(),
            CompradorTelefone           = PedidoHelpers.SomenteDigitos(dto.Comprador.Telefone),
            CompradorDataNascimento     = dto.Comprador.DataNascimento.Trim(),
            EnderecoCep                 = PedidoHelpers.SomenteDigitos(dto.Comprador.Cep),
            EnderecoLogradouro          = dto.Comprador.Logradouro.Trim(),
            EnderecoNumero              = dto.Comprador.Numero.Trim(),
            EnderecoComplemento         = string.IsNullOrWhiteSpace(dto.Comprador.Complemento) ? null : dto.Comprador.Complemento.Trim(),
            EnderecoBairro              = dto.Comprador.Bairro.Trim(),
            EnderecoCidade              = dto.Comprador.Cidade.Trim(),
            EnderecoEstado              = dto.Comprador.Estado.Trim().ToUpperInvariant()
        };

        // --- PROCESSA CADA ITEM DO PEDIDO ---
        foreach (var item in dto.Itens)
        {
            var ingresso = await _ingressoRepository.BuscarPorIdAsync(item.IngressoId, cancellationToken)
                ?? throw new KeyNotFoundException($"Ingresso {item.IngressoId} não encontrado.");

            if (item.Quantidade <= 0 || ingresso.QuantidadeDisponivel < item.Quantidade)
            {
                throw new InvalidOperationException($"Quantidade indisponível para ingresso {item.IngressoId}.");
            }

            if (!CatalogoIngresso.ModalidadeValida(item.Modalidade))
            {
                throw new InvalidOperationException($"Modalidade inválida para ingresso {item.IngressoId}.");
            }

            var modalidade      = item.Modalidade.Trim().ToUpperInvariant();
            string? subtipo     = null;
            string? documentosJson = null;

            if (modalidade == CatalogoIngresso.ModalidadeMeia)
            {
                if (!CatalogoIngresso.SubtipoMeiaValido(item.SubtipoMeia))
                {
                    throw new InvalidOperationException($"Subtipo de meia inválido para ingresso {item.IngressoId}.");
                }
                subtipo = item.SubtipoMeia!.Trim().ToUpperInvariant();

                var documentos = item.Documentos ?? new List<Dictionary<string, string?>>();
                if (documentos.Count != item.Quantidade)
                {
                    throw new InvalidOperationException(
                        $"Informe os documentos da meia entrada para cada ingresso do setor (esperado {item.Quantidade}).");
                }

                foreach (var doc in documentos)
                {
                    CatalogoIngresso.ValidarDocumentos(subtipo, doc);
                }

                documentosJson = System.Text.Json.JsonSerializer.Serialize(documentos);
            }

            // --- BAIXA O ESTOQUE E CALCULA O PRECO UNITARIO PELA MODALIDADE ---
            ingresso.QuantidadeDisponivel -= item.Quantidade;

            pedido.Itens.Add(new ItemPedido
            {
                IngressoId      = ingresso.Id,
                Quantidade      = item.Quantidade,
                PrecoUnitario   = CatalogoIngresso.CalcularPreco(ingresso.Preco, modalidade),
                Modalidade      = modalidade,
                Subtipo         = subtipo,
                DocumentosJson  = documentosJson
            });
        }

        // --- CALCULA O VALOR TOTAL DO PEDIDO ---
        pedido.ValorTotal = pedido.Itens.Sum(i => i.Quantidade * i.PrecoUnitario);

        // --- PERSISTE O PEDIDO E ATUALIZA O ESTOQUE ---
        await _pedidoRepository.AdicionarPedidoAsync(pedido, cancellationToken);
        await _ingressoRepository.AtualizarAsync(cancellationToken);

        // --- PROCESSA PAGAMENTO (SIMULADO PELO PagamentoService) ---
        var pagamento = _pagamentoService.ProcessarPagamento(dto, pedido.Id);
        await _pedidoRepository.AdicionarPagamentoAsync(pagamento, cancellationToken);

        // --- DEFINE STATUS FINAL DO PEDIDO E GERA QR CODE QUANDO APROVADO ---
        pedido.Status = pagamento.Status == StatusPagamento.Aprovado
            ? StatusPedido.Pago
            : StatusPedido.PagamentoRecusado;

        if (pagamento.Status == StatusPagamento.Aprovado)
        {
            pedido.CheckinToken  = PedidoHelpers.GerarTokenCheckin();
            pedido.QrCodeBase64  = PedidoHelpers.GerarQrCodeBase64(pedido.CheckinToken);
        }

        await _pedidoRepository.SalvarAlteracoesAsync(cancellationToken);

        return new PedidoRespostaDto
        {
            Id              = pedido.Id,
            ValorTotal      = pedido.ValorTotal,
            Status          = pedido.Status,
            QrCodeBase64    = pedido.QrCodeBase64,
            PagamentoStatus = pagamento.Status,
            CodigoPix       = pagamento.CodigoPix
        };
    }

    private static void ValidarDadosComprador(DadosCompradorDto comprador)
    {
        if (
            string.IsNullOrWhiteSpace(comprador.Nome) ||
            string.IsNullOrWhiteSpace(comprador.Cpf) ||
            string.IsNullOrWhiteSpace(comprador.Email) ||
            string.IsNullOrWhiteSpace(comprador.Telefone) ||
            string.IsNullOrWhiteSpace(comprador.DataNascimento) ||
            string.IsNullOrWhiteSpace(comprador.Cep) ||
            string.IsNullOrWhiteSpace(comprador.Logradouro) ||
            string.IsNullOrWhiteSpace(comprador.Numero) ||
            string.IsNullOrWhiteSpace(comprador.Bairro) ||
            string.IsNullOrWhiteSpace(comprador.Cidade) ||
            string.IsNullOrWhiteSpace(comprador.Estado)
        )
        {
            throw new InvalidOperationException("Preencha os dados obrigatórios do comprador para finalizar a compra.");
        }

        // --- VALIDACOES DE FORMATO ---
        if (PedidoHelpers.SomenteDigitos(comprador.Cpf).Length != 11)
        {
            throw new InvalidOperationException("CPF do comprador inválido.");
        }

        if (PedidoHelpers.SomenteDigitos(comprador.Telefone).Length < 10)
        {
            throw new InvalidOperationException("Telefone do comprador inválido.");
        }

        if (PedidoHelpers.SomenteDigitos(comprador.Cep).Length != 8)
        {
            throw new InvalidOperationException("CEP do comprador inválido.");
        }

        if (!comprador.Email.Contains('@') || !comprador.Email.Contains('.'))
        {
            throw new InvalidOperationException("E-mail do comprador inválido.");
        }

        if (comprador.Estado.Trim().Length != 2)
        {
            throw new InvalidOperationException("UF do comprador deve conter 2 letras.");
        }

        if (!DateTime.TryParse(comprador.DataNascimento, out var dataNascimento))
        {
            throw new InvalidOperationException("Data de nascimento do comprador inválida.");
        }

        if (dataNascimento.Date > DateTime.UtcNow.Date)
        {
            throw new InvalidOperationException("Data de nascimento do comprador não pode ser no futuro.");
        }
    }
}

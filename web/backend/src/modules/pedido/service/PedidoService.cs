using LiveEventsTicket.Backend.Modules.Evento.Repository;
using LiveEventsTicket.Backend.Modules.Ingresso.Model;
using LiveEventsTicket.Backend.Modules.Ingresso.Repository;
using LiveEventsTicket.Backend.Modules.Pagamento.Service;
using LiveEventsTicket.Backend.Modules.Pedido.Dto;
using LiveEventsTicket.Backend.Modules.Pedido.Repository;

using QRCoder;

using IngressoEntity = LiveEventsTicket.Backend.Modules.Ingresso.Model.Ingresso;
using ItemPedido = LiveEventsTicket.Backend.Modules.Pedido.Model.ItemPedido;
using PedidoCheckinLogEntity = LiveEventsTicket.Backend.Modules.Pedido.Model.PedidoCheckinLog;
using PedidoEntity = LiveEventsTicket.Backend.Modules.Pedido.Model.Pedido;

namespace LiveEventsTicket.Backend.Modules.Pedido.Service;

public class PedidoService
{
    private const int JanelaArrependimentoDias = 7;
    private const int MinimoHorasAntesEvento = 48;
    private const string MotivoReembolsoArrependimento = "ARREPENDIMENTO_7_DIAS";
    private const string MotivoReembolsoImpedimento = "IMPEDIMENTO_PESSOAL";
    private const string MotivoReembolsoAlteracaoPlanos = "ALTERACAO_DE_PLANOS";
    private const string MotivoReembolsoErroCompra = "ERRO_NA_COMPRA";
    private const string MotivoReembolsoOutro = "OUTRO";
    private const int CompartilhamentoValidadePadraoMinutos = 60;
    private const int CompartilhamentoValidadeMinMinutos = 5;
    private const int CompartilhamentoValidadeMaxMinutos = 1440;
    private const int CompartilhamentoMaxAcessosPadrao = 3;
    private const int CompartilhamentoMaxAcessosMin = 1;
    private const int CompartilhamentoMaxAcessosMax = 50;

    private static readonly Dictionary<string, string> MotivosReembolsoPadrao = new(StringComparer.OrdinalIgnoreCase)
    {
        { MotivoReembolsoArrependimento, "Arrependimento dentro do prazo legal de 7 dias." },
        { MotivoReembolsoImpedimento, "Impedimento pessoal para comparecer ao evento." },
        { MotivoReembolsoAlteracaoPlanos, "Mudança de planos do comprador." },
        { MotivoReembolsoErroCompra, "Erro na compra (setor, quantidade ou dados)." },
        { MotivoReembolsoOutro, "Outro motivo informado pelo comprador." }
    };

    private readonly IEventoRepository _eventoRepository;
    private readonly IIngressoRepository _ingressoRepository;
    private readonly IPedidoRepository _pedidoRepository;
    private readonly PagamentoService _pagamentoService;

    public PedidoService(
        IEventoRepository eventoRepository,
        IIngressoRepository ingressoRepository,
        IPedidoRepository pedidoRepository,
        PagamentoService pagamentoService)
    {
        _eventoRepository = eventoRepository;
        _ingressoRepository = ingressoRepository;
        _pedidoRepository = pedidoRepository;
        _pagamentoService = pagamentoService;
    }

    // --- CRIAR PEDIDO COM VALIDAÇÃO ---
    public async Task<PedidoRespostaDto> CriarAsync(int usuarioId, CriarPedidoDto dto, CancellationToken cancellationToken = default)
    {
        if (dto.Itens.Count == 0)
        {
            throw new InvalidOperationException("Pedido deve conter ao menos um ingresso.");
        }

        ValidarDadosComprador(dto.Comprador);

        var pedido = new PedidoEntity
        {
            UsuarioId = usuarioId,
            CompradorNome = dto.Comprador.Nome.Trim(),
            CompradorCpf = SomenteDigitos(dto.Comprador.Cpf),
            CompradorEmail = dto.Comprador.Email.Trim(),
            CompradorTelefone = SomenteDigitos(dto.Comprador.Telefone),
            CompradorDataNascimento = dto.Comprador.DataNascimento.Trim(),
            EnderecoCep = SomenteDigitos(dto.Comprador.Cep),
            EnderecoLogradouro = dto.Comprador.Logradouro.Trim(),
            EnderecoNumero = dto.Comprador.Numero.Trim(),
            EnderecoComplemento = string.IsNullOrWhiteSpace(dto.Comprador.Complemento) ? null : dto.Comprador.Complemento.Trim(),
            EnderecoBairro = dto.Comprador.Bairro.Trim(),
            EnderecoCidade = dto.Comprador.Cidade.Trim(),
            EnderecoEstado = dto.Comprador.Estado.Trim().ToUpperInvariant()
        };

        // --- PROCESSAR CADA ITEM DO PEDIDO ---
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

            var modalidade = item.Modalidade.Trim().ToUpperInvariant();
            string? subtipo = null;
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
                IngressoId = ingresso.Id,
                Quantidade = item.Quantidade,
                PrecoUnitario = CatalogoIngresso.CalcularPreco(ingresso.Preco, modalidade),
                Modalidade = modalidade,
                Subtipo = subtipo,
                DocumentosJson = documentosJson
            });
        }

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
            pedido.CheckinToken = GerarTokenCheckin();
            pedido.QrCodeBase64 = GerarQrCodeBase64(pedido.CheckinToken);
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
        var agora = DateTime.UtcNow;

        var ingressoIds = pedidos.SelectMany(p => p.Itens).Select(i => i.IngressoId).Distinct().ToList();
        var ingressos = new Dictionary<int, IngressoEntity>();
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

            DateTime? dataEvento = null;
            if (ingresso is not null)
            {
                var evento = await _eventoRepository.BuscarPorIdAsync(ingresso.EventoId, cancellationToken);
                dataEvento = evento?.DataEvento;
            }

            var (reembolsoElegivel, reembolsoMensagem, regraReembolso) = AvaliarElegibilidadeReembolso(
                p,
                dataEvento,
                agora);

            var pagamento = await _pedidoRepository.BuscarPagamentoPorPedidoIdAsync(p.Id, cancellationToken);
            DateTime? reembolsoEstornadoEm = pagamento?.Status == "REEMBOLSADO" ? pagamento.DataPagamento : null;
            var protocoloEstorno = GerarProtocoloEstorno(p.Id, reembolsoEstornadoEm);

            resposta.Add(new PedidoRespostaDto
            {
                Id = p.Id,
                ValorTotal = p.ValorTotal,
                Status = p.Status,
                QrCodeBase64 = p.QrCodeBase64,
                PagamentoStatus = pagamento?.Status ?? (p.Status == "PAGO" ? "APROVADO" : "RECUSADO"),
                DataCriacao = p.DataCriacao,
                EventoId = ingresso?.EventoId,
                IngressoId = primeiroItem?.IngressoId,
                Setor = ingresso?.Setor,
                Quantidade = primeiroItem?.Quantidade ?? 0,
                CompartilhamentoToken = p.CompartilhamentoToken,
                CompartilhamentoExpiraEm = p.CompartilhamentoExpiraEm,
                CompartilhamentoRevogadoEm = p.CompartilhamentoRevogadoEm,
                CompartilhamentoMaxAcessos = p.CompartilhamentoMaxAcessos,
                CompartilhamentoAcessosRealizados = p.CompartilhamentoAcessosRealizados,
                CompartilhamentoAtivo = CompartilhamentoAtivo(p, agora),
                ReembolsoSolicitadoEm = p.ReembolsoSolicitadoEm,
                ReembolsoAprovadoEm = p.ReembolsoAprovadoEm,
                ReembolsoEstornadoEm = reembolsoEstornadoEm,
                ReembolsoMotivo = p.ReembolsoMotivo,
                ReembolsoMotivoCodigo = p.ReembolsoMotivoCodigo,
                ReembolsoRegraAplicada = p.ReembolsoRegraAplicada,
                ReembolsoElegivel = reembolsoElegivel,
                ReembolsoMensagem = reembolsoMensagem ?? regraReembolso,
                ReembolsoProtocoloEstorno = protocoloEstorno
            });
        }

        return resposta;
    }

    // --- SOLICITA REEMBOLSO COM DECISAO AUTOMATICA SEM APROVACAO MANUAL ---
    public async Task<ReembolsoRespostaDto> SolicitarReembolsoAsync(
        int usuarioId,
        int pedidoId,
        SolicitarReembolsoDto dto,
        CancellationToken cancellationToken = default)
    {
        var pedido = await _pedidoRepository.BuscarPorIdEUsuarioAsync(pedidoId, usuarioId, cancellationToken)
            ?? throw new KeyNotFoundException("Pedido não encontrado para este usuário.");

        var pagamento = await _pedidoRepository.BuscarPagamentoPorPedidoIdAsync(pedidoId, cancellationToken)
            ?? throw new InvalidOperationException("Pagamento do pedido não encontrado.");

        var ingressoIds = pedido.Itens.Select(i => i.IngressoId).Distinct().ToList();
        var ingressos = new Dictionary<int, IngressoEntity>();
        foreach (var ingressoId in ingressoIds)
        {
            var ingresso = await _ingressoRepository.BuscarPorIdAsync(ingressoId, cancellationToken)
                ?? throw new KeyNotFoundException($"Ingresso {ingressoId} não encontrado.");

            ingressos[ingressoId] = ingresso;
        }

        DateTime? dataEvento = null;
        var primeiroIngresso = pedido.Itens.FirstOrDefault();
        if (primeiroIngresso is not null && ingressos.TryGetValue(primeiroIngresso.IngressoId, out var primeiro))
        {
            var evento = await _eventoRepository.BuscarPorIdAsync(primeiro.EventoId, cancellationToken);
            dataEvento = evento?.DataEvento;
        }

        var agora = DateTime.UtcNow;
        var (elegivel, mensagem, regra) = AvaliarElegibilidadeReembolso(pedido, dataEvento, agora);
        if (!elegivel)
        {
            throw new InvalidOperationException(mensagem ?? "Pedido não elegível para reembolso automático.");
        }

        var motivo = NormalizarMotivoReembolso(dto);

        // --- DECISAO AUTOMATICA APROVADA: REVERTE ESTOQUE E ESTORNA INTERNAMENTE ---
        foreach (var item in pedido.Itens)
        {
            if (ingressos.TryGetValue(item.IngressoId, out var ingresso))
            {
                ingresso.QuantidadeDisponivel += item.Quantidade;
            }
        }

        pagamento.Status = "REEMBOLSADO";
        pagamento.DataPagamento = agora;

        pedido.Status = "REEMBOLSADO";
        pedido.ReembolsoSolicitadoEm = agora;
        pedido.ReembolsoAprovadoEm = agora;
        pedido.ReembolsoMotivoCodigo = motivo.Codigo;
        pedido.ReembolsoMotivo = motivo.MotivoPersistido;
        pedido.ReembolsoRegraAplicada = regra;

        await _ingressoRepository.AtualizarAsync(cancellationToken);
        await _pedidoRepository.SalvarAlteracoesAsync(cancellationToken);

        var protocoloEstorno = GerarProtocoloEstorno(pedido.Id, pagamento.DataPagamento);

        return new ReembolsoRespostaDto
        {
            PedidoId = pedido.Id,
            Status = pedido.Status,
            Mensagem = "Reembolso aprovado automaticamente e processado com sucesso.",
            RegraAplicada = regra,
            MotivoCodigo = motivo.Codigo,
            MotivoDescricao = motivo.Descricao,
            MotivoDetalhe = motivo.Detalhe,
            SolicitadoEm = pedido.ReembolsoSolicitadoEm,
            AprovadoEm = pedido.ReembolsoAprovadoEm,
            EstornadoEm = pagamento.DataPagamento,
            ProtocoloEstorno = protocoloEstorno,
            ComprovanteDisponivel = true
        };
    }

    // --- GERA O COMPROVANTE DE ESTORNO PARA PEDIDOS REEMBOLSADOS ---
    public async Task<(byte[] arquivo, string nomeArquivo)> GerarComprovanteEstornoPdfAsync(
        int usuarioId,
        int pedidoId,
        CancellationToken cancellationToken = default)
    {
        var pedido = await _pedidoRepository.BuscarPorIdEUsuarioAsync(pedidoId, usuarioId, cancellationToken)
            ?? throw new KeyNotFoundException("Pedido não encontrado para este usuário.");

        var pagamento = await _pedidoRepository.BuscarPagamentoPorPedidoIdAsync(pedidoId, cancellationToken)
            ?? throw new InvalidOperationException("Pagamento do pedido não encontrado.");

        if (pedido.Status != "REEMBOLSADO" || pagamento.Status != "REEMBOLSADO")
        {
            throw new InvalidOperationException("Comprovante disponível apenas para pedidos estornados.");
        }

        var primeiroItem = pedido.Itens.FirstOrDefault();
        string eventoTitulo = "Evento não identificado";
        string? setor = null;
        DateTime? dataEvento = null;

        if (primeiroItem is not null)
        {
            var ingresso = await _ingressoRepository.BuscarPorIdAsync(primeiroItem.IngressoId, cancellationToken);
            if (ingresso is not null)
            {
                setor = ingresso.Setor;
                var evento = await _eventoRepository.BuscarPorIdAsync(ingresso.EventoId, cancellationToken);
                if (evento is not null)
                {
                    eventoTitulo = evento.Titulo;
                    dataEvento = evento.DataEvento;
                }
            }
        }

        var estornadoEm = pagamento.DataPagamento;
        var dados = new EstornoComprovantePdfDados
        {
            PedidoId = pedido.Id,
            ProtocoloEstorno = GerarProtocoloEstorno(pedido.Id, estornadoEm),
            NomeComprador = pedido.CompradorNome,
            DocumentoComprador = pedido.CompradorCpf,
            EmailComprador = pedido.CompradorEmail,
            MotivoCodigo = pedido.ReembolsoMotivoCodigo ?? MotivoReembolsoOutro,
            MotivoDescricao = DescricaoMotivoReembolso(pedido.ReembolsoMotivoCodigo),
            MotivoInformado = pedido.ReembolsoMotivo,
            RegraAplicada = pedido.ReembolsoRegraAplicada,
            ValorEstornadoFormatado = FormatarMoeda(pedido.ValorTotal),
            DataSolicitacao = pedido.ReembolsoSolicitadoEm,
            DataAprovacao = pedido.ReembolsoAprovadoEm,
            DataEstorno = estornadoEm,
            EventoTitulo = eventoTitulo,
            EventoData = dataEvento,
            Setor = setor,
            QuantidadeIngressos = pedido.Itens.Sum(i => i.Quantidade)
        };

        var pdf = EstornoComprovantePdfBuilder.Gerar(dados);
        var nomeArquivo = $"comprovante-estorno-pedido-{pedido.Id}.pdf";
        return (pdf, nomeArquivo);
    }

    // --- GERA UM LINK TEMPORARIO DE COMPARTILHAMENTO DO PDF ---
    public async Task<CompartilhamentoIngressoRespostaDto> GerarCompartilhamentoIngressoAsync(
        int usuarioId,
        int pedidoId,
        CriarCompartilhamentoIngressoDto dto,
        CancellationToken cancellationToken = default)
    {
        var pedido = await _pedidoRepository.BuscarPorIdEUsuarioAsync(pedidoId, usuarioId, cancellationToken)
            ?? throw new KeyNotFoundException("Pedido não encontrado para este usuário.");

        if (pedido.Status != "PAGO")
        {
            throw new InvalidOperationException("Só é possível compartilhar ingressos de pedidos pagos.");
        }

        var validadeMinutos = NormalizarIntervalo(
            dto.ValidadeMinutos ?? CompartilhamentoValidadePadraoMinutos,
            CompartilhamentoValidadeMinMinutos,
            CompartilhamentoValidadeMaxMinutos);

        var maxAcessos = NormalizarIntervalo(
            dto.MaxAcessos ?? CompartilhamentoMaxAcessosPadrao,
            CompartilhamentoMaxAcessosMin,
            CompartilhamentoMaxAcessosMax);

        pedido.CompartilhamentoToken = GerarTokenCompartilhamento();
        pedido.CompartilhamentoExpiraEm = DateTime.UtcNow.AddMinutes(validadeMinutos);
        pedido.CompartilhamentoRevogadoEm = null;
        pedido.CompartilhamentoMaxAcessos = maxAcessos;
        pedido.CompartilhamentoAcessosRealizados = 0;

        await _pedidoRepository.SalvarAlteracoesAsync(cancellationToken);

        return new CompartilhamentoIngressoRespostaDto
        {
            PedidoId = pedido.Id,
            Token = pedido.CompartilhamentoToken,
            ExpiraEm = pedido.CompartilhamentoExpiraEm.Value,
            RevogadoEm = pedido.CompartilhamentoRevogadoEm,
            MaxAcessos = pedido.CompartilhamentoMaxAcessos,
            AcessosRealizados = pedido.CompartilhamentoAcessosRealizados,
            Ativo = true,
            Mensagem = "Link de compartilhamento gerado com sucesso."
        };
    }

    // --- REVOGA O LINK DE COMPARTILHAMENTO GERADO PELO USUARIO ---
    public async Task<CompartilhamentoIngressoRespostaDto> RevogarCompartilhamentoIngressoAsync(
        int usuarioId,
        int pedidoId,
        CancellationToken cancellationToken = default)
    {
        var pedido = await _pedidoRepository.BuscarPorIdEUsuarioAsync(pedidoId, usuarioId, cancellationToken)
            ?? throw new KeyNotFoundException("Pedido não encontrado para este usuário.");

        if (string.IsNullOrWhiteSpace(pedido.CompartilhamentoToken))
        {
            throw new InvalidOperationException("Este pedido ainda não possui link de compartilhamento ativo.");
        }

        pedido.CompartilhamentoRevogadoEm = DateTime.UtcNow;
        await _pedidoRepository.SalvarAlteracoesAsync(cancellationToken);

        return new CompartilhamentoIngressoRespostaDto
        {
            PedidoId = pedido.Id,
            Token = pedido.CompartilhamentoToken,
            ExpiraEm = pedido.CompartilhamentoExpiraEm ?? DateTime.UtcNow,
            RevogadoEm = pedido.CompartilhamentoRevogadoEm,
            MaxAcessos = pedido.CompartilhamentoMaxAcessos,
            AcessosRealizados = pedido.CompartilhamentoAcessosRealizados,
            Ativo = false,
            Mensagem = "Link de compartilhamento revogado com sucesso."
        };
    }

    // --- GERA PDF DO INGRESSO PARA O USUARIO DONO DO PEDIDO ---
    public async Task<(byte[] arquivo, string nomeArquivo)> GerarIngressoPdfAsync(
        int usuarioId,
        int pedidoId,
        CancellationToken cancellationToken = default)
    {
        var pedido = await _pedidoRepository.BuscarPorIdEUsuarioAsync(pedidoId, usuarioId, cancellationToken)
            ?? throw new KeyNotFoundException("Pedido não encontrado para este usuário.");

        if (pedido.Status != "PAGO")
        {
            throw new InvalidOperationException("O PDF do ingresso está disponível apenas para pedidos pagos.");
        }

        return await MontarIngressoPdfDoPedidoAsync(pedido, cancellationToken);
    }

    // --- BAIXA PDF A PARTIR DE TOKEN PUBLICO TEMPORARIO E CONTROLADO ---
    public async Task<(byte[] arquivo, string nomeArquivo)> BaixarIngressoPdfCompartilhadoAsync(
        string tokenCompartilhamento,
        CancellationToken cancellationToken = default)
    {
        var token = (tokenCompartilhamento ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new InvalidOperationException("Token de compartilhamento inválido.");
        }

        var pedido = await _pedidoRepository.BuscarPorCompartilhamentoTokenAsync(token, cancellationToken)
            ?? throw new KeyNotFoundException("Link de compartilhamento não encontrado.");

        if (pedido.Status != "PAGO")
        {
            throw new InvalidOperationException("Este ingresso não está disponível para compartilhamento.");
        }

        if (!CompartilhamentoAtivo(pedido, DateTime.UtcNow))
        {
            throw new InvalidOperationException("Link expirado, revogado ou limite de acessos atingido.");
        }

        var resultado = await MontarIngressoPdfDoPedidoAsync(pedido, cancellationToken);

        pedido.CompartilhamentoAcessosRealizados += 1;
        await _pedidoRepository.SalvarAlteracoesAsync(cancellationToken);

        return resultado;
    }

    // --- MONTA O PDF DO INGRESSO COM DADOS COMPLETOS DO PEDIDO ---
    private async Task<(byte[] arquivo, string nomeArquivo)> MontarIngressoPdfDoPedidoAsync(
        PedidoEntity pedido,
        CancellationToken cancellationToken)
    {
        var linhas = new List<IngressoPdfLinha>();

        foreach (var item in pedido.Itens)
        {
            var ingresso = await _ingressoRepository.BuscarPorIdAsync(item.IngressoId, cancellationToken)
                ?? throw new KeyNotFoundException($"Ingresso {item.IngressoId} não encontrado.");

            var evento = await _eventoRepository.BuscarPorIdAsync(ingresso.EventoId, cancellationToken);

            linhas.Add(new IngressoPdfLinha
            {
                EventoTitulo = evento?.Titulo ?? "Evento não identificado",
                EventoData = evento?.DataEvento.ToLocalTime().ToString("dd/MM/yyyy HH:mm") ?? "Data não informada",
                EventoLocal = evento?.Local ?? "Local não informado",
                Setor = ingresso.Setor,
                Quantidade = item.Quantidade,
                ValorUnitarioFormatado = FormatarMoeda(item.PrecoUnitario),
                SubtotalFormatado = FormatarMoeda(item.PrecoUnitario * item.Quantidade)
            });
        }

        var endereco = MontarEnderecoComprador(pedido);

        if (string.IsNullOrWhiteSpace(pedido.CheckinToken))
        {
            pedido.CheckinToken = GerarTokenCheckin();
            await _pedidoRepository.SalvarAlteracoesAsync(cancellationToken);
        }

        var qrCode = string.IsNullOrWhiteSpace(pedido.QrCodeBase64)
            ? GerarQrCodeBase64(pedido.CheckinToken)
            : pedido.QrCodeBase64;

        var dados = new IngressoPdfDados
        {
            PedidoId = pedido.Id,
            NomeComprador = pedido.CompradorNome,
            DocumentoComprador = pedido.CompradorCpf,
            EmailComprador = pedido.CompradorEmail,
            EnderecoComprador = endereco,
            DataCompra = pedido.DataCriacao.ToLocalTime().ToString("dd/MM/yyyy HH:mm"),
            ValorTotalFormatado = FormatarMoeda(pedido.ValorTotal),
            QrCodeBase64 = qrCode,
            Itens = linhas
        };

        var pdf = IngressoPdfBuilder.Gerar(dados);
        var nomeArquivo = $"ingresso-pedido-{pedido.Id}.pdf";

        return (pdf, nomeArquivo);
    }

    // --- VALIDA O CHECKIN DO INGRESSO E BLOQUEIA REUTILIZACAO ALEM DO LIMITE ---
    public async Task<CheckinRespostaDto> ValidarCheckinAsync(
        int operadorId,
        ValidarCheckinDto dto,
        CancellationToken cancellationToken = default)
    {
        var token = (dto.Token ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(token))
        {
            return await RegistrarTentativaCheckinAsync(
                pedido: null,
                operadorId: operadorId,
                tokenInformado: token,
                permitido: false,
                mensagem: "Token do ingresso não informado.",
                cancellationToken: cancellationToken);
        }

        var pedido = await _pedidoRepository.BuscarPorCheckinTokenAsync(token, cancellationToken);
        if (pedido is null)
        {
            return await RegistrarTentativaCheckinAsync(
                pedido: null,
                operadorId: operadorId,
                tokenInformado: token,
                permitido: false,
                mensagem: "Ingresso inválido ou não encontrado.",
                cancellationToken: cancellationToken);
        }

        var quantidadeTotal = pedido.Itens.Sum(i => i.Quantidade);
        if (quantidadeTotal <= 0)
        {
            return await RegistrarTentativaCheckinAsync(
                pedido,
                operadorId,
                token,
                false,
                "Pedido sem itens válidos para check-in.",
                cancellationToken,
                quantidadeTotal,
                pedido.CheckinUsosRealizados);
        }

        if (pedido.Status != "PAGO")
        {
            return await RegistrarTentativaCheckinAsync(
                pedido,
                operadorId,
                token,
                false,
                "Check-in disponível apenas para pedidos pagos.",
                cancellationToken,
                quantidadeTotal,
                pedido.CheckinUsosRealizados);
        }

        if (pedido.CheckinUsosRealizados >= quantidadeTotal)
        {
            return await RegistrarTentativaCheckinAsync(
                pedido,
                operadorId,
                token,
                false,
                "Todos os ingressos deste pedido já foram utilizados.",
                cancellationToken,
                quantidadeTotal,
                pedido.CheckinUsosRealizados);
        }

        pedido.CheckinUsosRealizados += 1;
        await _pedidoRepository.SalvarAlteracoesAsync(cancellationToken);

        return await RegistrarTentativaCheckinAsync(
            pedido,
            operadorId,
            token,
            true,
            "Check-in validado com sucesso.",
            cancellationToken,
            quantidadeTotal,
            pedido.CheckinUsosRealizados);
    }

    // --- VALIDA DADOS OBRIGATÓRIOS DO COMPRADOR ---
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

        var cpf = SomenteDigitos(comprador.Cpf);
        if (cpf.Length != 11)
        {
            throw new InvalidOperationException("CPF do comprador inválido.");
        }

        var telefone = SomenteDigitos(comprador.Telefone);
        if (telefone.Length < 10)
        {
            throw new InvalidOperationException("Telefone do comprador inválido.");
        }

        var cep = SomenteDigitos(comprador.Cep);
        if (cep.Length != 8)
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

    // --- MANTEM APENAS DIGITOS DE UMA STRING ---
    private static string SomenteDigitos(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            return string.Empty;
        }

        return new string(valor.Where(char.IsDigit).ToArray());
    }

    // --- FORMATA DECIMAL NO PADRAO DE MOEDA BRL ---
    private static string FormatarMoeda(decimal valor)
    {
        return valor.ToString("C", new System.Globalization.CultureInfo("pt-BR"));
    }

    // --- GERA TOKEN UNICO DE CHECKIN PARA LEITURA NO QR CODE ---
    private static string GerarTokenCheckin()
    {
        return $"CHK-{Guid.NewGuid():N}";
    }

    // --- GERA TOKEN UNICO PARA COMPARTILHAMENTO PUBLICO DE PDF ---
    private static string GerarTokenCompartilhamento()
    {
        return $"SHR-{Guid.NewGuid():N}";
    }

    // --- NORMALIZA UM VALOR NUMERICO PARA UM INTERVALO PERMITIDO ---
    private static int NormalizarIntervalo(int valor, int minimo, int maximo)
    {
        return Math.Clamp(valor, minimo, maximo);
    }

    // --- GERA UM CODIGO DE PROTOCOLO DE ESTORNO COM BASE NO PEDIDO E NA DATA ---
    private static string GerarProtocoloEstorno(int pedidoId, DateTime? dataEstorno)
    {
        if (!dataEstorno.HasValue)
        {
            return string.Empty;
        }

        return $"EST-{pedidoId:D6}-{dataEstorno.Value:yyyyMMddHHmmss}";
    }

    // --- RETORNA DESCRICAO PADRAO DO CODIGO DE MOTIVO DE REEMBOLSO ---
    private static string DescricaoMotivoReembolso(string? codigo)
    {
        var codigoNormalizado = NormalizarCodigoMotivo(codigo);
        if (!string.IsNullOrWhiteSpace(codigoNormalizado) && MotivosReembolsoPadrao.TryGetValue(codigoNormalizado, out var descricao))
        {
            return descricao;
        }

        return MotivosReembolsoPadrao[MotivoReembolsoOutro];
    }

    // --- NORMALIZA E VALIDA O MOTIVO INFORMADO NO PEDIDO DE REEMBOLSO ---
    private static MotivoReembolsoResolvido NormalizarMotivoReembolso(SolicitarReembolsoDto dto)
    {
        var codigo = NormalizarCodigoMotivo(dto.MotivoCodigo);
        var detalhe = string.IsNullOrWhiteSpace(dto.MotivoDetalhe) ? null : dto.MotivoDetalhe.Trim();
        var motivoLivre = string.IsNullOrWhiteSpace(dto.Motivo) ? null : dto.Motivo.Trim();

        if (string.IsNullOrWhiteSpace(codigo))
        {
            if (string.IsNullOrWhiteSpace(motivoLivre))
            {
                codigo = MotivoReembolsoArrependimento;
            }
            else
            {
                codigo = MotivoReembolsoOutro;
                detalhe = motivoLivre;
            }
        }

        if (!MotivosReembolsoPadrao.TryGetValue(codigo, out var descricao))
        {
            var codigosValidos = string.Join(", ", MotivosReembolsoPadrao.Keys.OrderBy(x => x));
            throw new InvalidOperationException($"Motivo de reembolso inválido. Use um dos códigos: {codigosValidos}.");
        }

        if (codigo == MotivoReembolsoOutro)
        {
            var detalheFinal = detalhe ?? motivoLivre;
            if (string.IsNullOrWhiteSpace(detalheFinal))
            {
                throw new InvalidOperationException("Para o motivo OUTRO, detalhe o motivo do estorno.");
            }

            detalhe = detalheFinal.Trim();
        }

        if (!string.IsNullOrWhiteSpace(detalhe) && detalhe.Length > 300)
        {
            throw new InvalidOperationException("O detalhe do motivo deve ter no máximo 300 caracteres.");
        }

        var motivoPersistido = string.IsNullOrWhiteSpace(detalhe)
            ? descricao
            : $"{descricao} Detalhe: {detalhe}";

        return new MotivoReembolsoResolvido(codigo, descricao, detalhe, motivoPersistido);
    }

    // --- NORMALIZA CODIGO DE MOTIVO PARA O PADRAO ESPERADO ---
    private static string NormalizarCodigoMotivo(string? codigo)
    {
        if (string.IsNullOrWhiteSpace(codigo))
        {
            return string.Empty;
        }

        return codigo.Trim().ToUpperInvariant().Replace('-', '_').Replace(' ', '_');
    }

    // --- AVALIA SE O LINK COMPARTILHADO CONTINUA ATIVO ---
    private static bool CompartilhamentoAtivo(PedidoEntity pedido, DateTime agora)
    {
        if (string.IsNullOrWhiteSpace(pedido.CompartilhamentoToken))
        {
            return false;
        }

        if (pedido.CompartilhamentoRevogadoEm.HasValue)
        {
            return false;
        }

        if (!pedido.CompartilhamentoExpiraEm.HasValue || pedido.CompartilhamentoExpiraEm.Value <= agora)
        {
            return false;
        }

        if (pedido.CompartilhamentoMaxAcessos <= 0)
        {
            return false;
        }

        return pedido.CompartilhamentoAcessosRealizados < pedido.CompartilhamentoMaxAcessos;
    }

    // --- MONTA ENDERECO COMPLETO DO COMPRADOR EM UMA LINHA ---
    private static string MontarEnderecoComprador(PedidoEntity pedido)
    {
        var complemento = string.IsNullOrWhiteSpace(pedido.EnderecoComplemento)
            ? string.Empty
            : $" ({pedido.EnderecoComplemento})";

        return $"{pedido.EnderecoLogradouro}, {pedido.EnderecoNumero}{complemento} - {pedido.EnderecoBairro}, {pedido.EnderecoCidade}/{pedido.EnderecoEstado}, CEP {pedido.EnderecoCep}";
    }

    // --- REGISTRA A TENTATIVA DE CHECKIN E RETORNA RESPOSTA PADRONIZADA ---
    private async Task<CheckinRespostaDto> RegistrarTentativaCheckinAsync(
        PedidoEntity? pedido,
        int operadorId,
        string tokenInformado,
        bool permitido,
        string mensagem,
        CancellationToken cancellationToken,
        int quantidadeTotal = 0,
        int usosRealizados = 0)
    {
        var log = new PedidoCheckinLogEntity
        {
            PedidoId = pedido?.Id,
            OperadorId = operadorId,
            TokenInformado = tokenInformado,
            Permitido = permitido,
            Mensagem = mensagem,
            DataCheckin = DateTime.UtcNow
        };

        await _pedidoRepository.AdicionarCheckinLogAsync(log, cancellationToken);

        string? eventoTitulo = null;
        string? setor = null;
        
        if (pedido is not null && pedido.Itens.Count > 0)
        {
            var primeiroItem = pedido.Itens[0];
            var ingresso = await _ingressoRepository.BuscarPorIdAsync(primeiroItem.IngressoId, cancellationToken);
            if (ingresso is not null)
            {
                setor = ingresso.Setor;
                var evento = await _eventoRepository.BuscarPorIdAsync(ingresso.EventoId, cancellationToken);
                eventoTitulo = evento?.Titulo;
            }
        }

        return new CheckinRespostaDto
        {
            Permitido = permitido,
            Mensagem = mensagem,
            PedidoId = pedido?.Id,
            EventoTitulo = eventoTitulo,
            Setor = setor,
            QuantidadeTotal = quantidadeTotal,
            UsosRealizados = usosRealizados,
            UsosRestantes = Math.Max(0, quantidadeTotal - usosRealizados),
            DataCheckin = log.DataCheckin
        };
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

    // --- AVALIA ELEGIBILIDADE DE REEMBOLSO SEM INTERVENCAO MANUAL ---
    private static (bool elegivel, string? mensagem, string regra) AvaliarElegibilidadeReembolso(
        PedidoEntity pedido,
        DateTime? dataEvento,
        DateTime agora)
    {
        const string regra = "ATE_7_DIAS_DA_COMPRA_E_MIN_48H_ANTES_DO_EVENTO";

        if (pedido.Status == "REEMBOLSADO")
        {
            return (false, "Este pedido já foi reembolsado.", regra);
        }

        if (pedido.Status != "PAGO")
        {
            return (false, "Apenas pedidos com pagamento aprovado podem ser reembolsados.", regra);
        }

        var limiteArrependimento = pedido.DataCriacao.AddDays(JanelaArrependimentoDias);
        if (agora > limiteArrependimento)
        {
            return (false, "Prazo de arrependimento expirado para este pedido.", regra);
        }

        if (!dataEvento.HasValue)
        {
            return (false, "Não foi possível validar a data do evento para este pedido.", regra);
        }

        if (dataEvento.Value <= agora.AddHours(MinimoHorasAntesEvento))
        {
            return (false, "Reembolso indisponível para eventos com menos de 48 horas para início.", regra);
        }

        return (true, "Elegível para reembolso automático.", regra);
    }
}

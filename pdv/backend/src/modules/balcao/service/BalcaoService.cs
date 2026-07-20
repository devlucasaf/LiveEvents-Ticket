using Microsoft.EntityFrameworkCore;
using PontoVenda.Backend.Infra.Config;
using PontoVenda.Backend.Modules.Balcao.Dto;
using PontoVenda.Backend.Modules.Balcao.Model;
using QRCoder;

namespace PontoVenda.Backend.Modules.Balcao.Service;

public class BalcaoService
{
    private readonly SharedDbContext _web;
    private readonly AppDbContext    _pdv;

    // --- SENHA INICIAL PADRAO PARA NOVAS CONTAS DE CLIENTE ---
    private const string SenhaInicialPadrao = "Cliente@123";

    public BalcaoService(SharedDbContext web, AppDbContext pdv)
    {
        _web = web;
        _pdv = pdv;
    }

    // --- LISTAR EVENTOS DISPONIVEIS ---
    public async Task<List<EventoBalcaoDto>> ListarEventosAsync(CancellationToken cancellationToken = default)
    {
        return await _web.Eventos
            .OrderBy(e => e.DataEvento)
            .Select(e => new EventoBalcaoDto
            {
                Id = e.Id,
                Titulo = e.Titulo,
                Categoria = e.Categoria,
                Local = e.Local,
                DataEvento = e.DataEvento,
                ImagemUrl = e.ImagemUrl
            })
            .ToListAsync(cancellationToken);
    }

    // --- LISTAR TIPOS DE INGRESSO DE UM EVENTO ---
    public async Task<List<IngressoBalcaoDto>> ListarIngressosAsync(int eventoId, CancellationToken cancellationToken = default)
    {
        return await _web.Ingressos
            .Where(i => i.EventoId == eventoId)
            .OrderBy(i => i.Preco)
            .Select(i => new IngressoBalcaoDto
            {
                Id = i.Id,
                Setor = i.Setor,
                Preco = i.Preco,
                QuantidadeDisponivel = i.QuantidadeDisponivel
            })
            .ToListAsync(cancellationToken);
    }

    // --- CRIA/REUSA CLIENTE, GERA PEDIDO PAGO E TICKET ---
    public async Task<VendaBalcaoRespostaDto> RegistrarVendaAsync(
        CriarVendaBalcaoDto dto,
        int operadorId,
        string operadorNome,
        CancellationToken cancellationToken = default)
    {
        var tipoEntrada = (dto.TipoEntrada ?? "INTEIRA").Trim().ToUpperInvariant();
        if (tipoEntrada != "INTEIRA" && tipoEntrada != "MEIA" && tipoEntrada != "SOCIAL")
        {
            throw new InvalidOperationException("Tipo de entrada inválido. Use INTEIRA, MEIA ou SOCIAL.");
        }

        var subtipo = (dto.Subtipo ?? string.Empty).Trim().ToUpperInvariant();
        if (tipoEntrada == "MEIA")
        {
            if (!CatalogoMeia.SubtipoValido(subtipo))
            {
                throw new InvalidOperationException("Selecione o tipo de meia entrada.");
            }
        }
        else
        {
            subtipo = string.Empty;
        }

        var formaPagamento = (dto.FormaPagamento ?? "CREDITO").Trim().ToUpperInvariant();
        if (formaPagamento != "CREDITO" && formaPagamento != "DEBITO" && formaPagamento != "PIX")
        {
            throw new InvalidOperationException("Forma de pagamento inválida. Use CREDITO, DEBITO ou PIX.");
        }

        var evento = await _web.Eventos.FirstOrDefaultAsync(e => e.Id == dto.EventoId, cancellationToken)
            ?? throw new KeyNotFoundException("Evento não encontrado.");

        var ingresso = await _web.Ingressos.FirstOrDefaultAsync(i => i.Id == dto.IngressoId, cancellationToken)
            ?? throw new KeyNotFoundException("Tipo de ingresso não encontrado.");

        if (ingresso.EventoId != evento.Id)
        {
            throw new InvalidOperationException("O ingresso selecionado não pertence ao evento.");
        }

        if (dto.Quantidade <= 0)
        {
            throw new InvalidOperationException("Quantidade inválida.");
        }

        var acompanhantes = dto.Acompanhantes ?? new List<ClienteBalcaoDto>();
        if (acompanhantes.Count != dto.Quantidade - 1)
        {
            throw new InvalidOperationException("Informe os dados de todos os ingressos (um cliente por ingresso).");
        }

        if (tipoEntrada == "MEIA")
        {
            CatalogoMeia.ValidarPessoa(subtipo, dto.Cliente.Documentos, dto.Cliente.DataNascimento, "Comprador");
            for (var i = 0; i < acompanhantes.Count; i++)
            {
                var acomp = acompanhantes[i];
                CatalogoMeia.ValidarPessoa(subtipo, acomp.Documentos, acomp.DataNascimento, $"Acompanhante {i + 1}");
            }
        }

        if (ingresso.QuantidadeDisponivel < dto.Quantidade)
        {
            throw new InvalidOperationException($"Estoque insuficiente para o setor {ingresso.Setor}. Disponível: {ingresso.QuantidadeDisponivel}.");
        }

        var emailNormalizado = dto.Cliente.Email.Trim().ToLowerInvariant();
        var cliente = await _web.Clientes.FirstOrDefaultAsync(c => c.Email == emailNormalizado, cancellationToken);

        var contaCriada = false;
        if (cliente is null)
        {
            cliente = new ClienteWeb
            {
                Nome = dto.Cliente.Nome.Trim(),
                Sobrenome = dto.Cliente.Sobrenome.Trim(),
                Email = emailNormalizado,
                Cpf = dto.Cliente.Cpf.Trim(),
                Telefone = dto.Cliente.Telefone.Trim(),
                SenhaHash = BCrypt.Net.BCrypt.HashPassword(SenhaInicialPadrao),
                Role = "CLIENTE",
                DataCadastro = DateTime.UtcNow,
                DataNascimento = dto.Cliente.DataNascimento ?? DateTime.UtcNow
            };
            _web.Clientes.Add(cliente);
            await _web.SaveChangesAsync(cancellationToken);
            contaCriada = true;
        }

        var precoMeia = decimal.Round(ingresso.Preco / 2m, 2);
        var precoUnitario = tipoEntrada switch
        {
            "MEIA"   => precoMeia,
            "SOCIAL" => decimal.Round(precoMeia + CatalogoMeia.SocialAcrescimo, 2),
            _        => ingresso.Preco
        };
        var valorTotal = precoUnitario * dto.Quantidade;

        ingresso.QuantidadeDisponivel -= dto.Quantidade;

        var pedido = new PedidoWeb
        {
            UsuarioId = cliente.Id,
            ValorTotal = valorTotal,
            Status = "PAGO",
            DataCriacao = DateTime.UtcNow,
            Itens =
            {
                new ItemPedidoWeb
                {
                    IngressoId = ingresso.Id,
                    Quantidade = dto.Quantidade,
                    PrecoUnitario = precoUnitario
                }
            }
        };
        _web.Pedidos.Add(pedido);
        await _web.SaveChangesAsync(cancellationToken);

        _web.Pagamentos.Add(new PagamentoWeb
        {
            PedidoId = pedido.Id,
            Tipo = formaPagamento,
            Status = "APROVADO",
            DataPagamento = DateTime.UtcNow
        });

        pedido.QrCodeBase64 = GerarQrCodeBase64($"pedido:{pedido.Id}|usuario:{pedido.UsuarioId}|valor:{pedido.ValorTotal}");
        await _web.SaveChangesAsync(cancellationToken);

        var codigoTicket = "PDV-" + Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();

        _pdv.VendasBalcao.Add(new VendaBalcao
        {
            OperadorId = operadorId,
            OperadorNome = operadorNome,
            ClienteWebId = cliente.Id,
            ClienteNome = cliente.Nome,
            ClienteSobrenome = cliente.Sobrenome,
            ClienteEmail = cliente.Email,
            ClienteCpf = cliente.Cpf,
            ClienteTelefone = cliente.Telefone,
            ClienteDataNascimento = dto.Cliente.DataNascimento,
            Cep = dto.Cliente.Cep,
            Logradouro = dto.Cliente.Logradouro,
            Numero = dto.Cliente.Numero,
            Complemento = dto.Cliente.Complemento,
            Bairro = dto.Cliente.Bairro,
            Cidade = dto.Cliente.Cidade,
            Estado = dto.Cliente.Estado,
            PedidoWebId = pedido.Id,
            EventoId = evento.Id,
            EventoTitulo = evento.Titulo,
            IngressoId = ingresso.Id,
            Setor = ingresso.Setor,
            TipoEntrada = tipoEntrada,
            FormaPagamento = formaPagamento,
            Quantidade = dto.Quantidade,
            ValorUnitario = precoUnitario,
            ValorTotal = valorTotal,
            CodigoTicket = codigoTicket,
            DataVenda = DateTime.UtcNow,
            AcompanhantesJson = acompanhantes.Count > 0
                ? System.Text.Json.JsonSerializer.Serialize(acompanhantes)
                : null,
            Subtipo = string.IsNullOrEmpty(subtipo) ? null : subtipo,
            DocumentosJson = (tipoEntrada == "MEIA" && dto.Cliente.Documentos is { Count: > 0 })
                ? System.Text.Json.JsonSerializer.Serialize(dto.Cliente.Documentos)
                : null
        });
        await _pdv.SaveChangesAsync(cancellationToken);

        // --- MONTA A RESPOSTA ---
        return new VendaBalcaoRespostaDto
        {
            CodigoTicket = codigoTicket,
            PedidoId = pedido.Id,
            ClienteNome = $"{cliente.Nome} {cliente.Sobrenome}".Trim(),
            ClienteEmail = cliente.Email,
            EventoTitulo = evento.Titulo,
            EventoLocal = evento.Local,
            EventoData = evento.DataEvento,
            Setor = ingresso.Setor,
            TipoEntrada = tipoEntrada,
            Quantidade = dto.Quantidade,
            ValorTotal = valorTotal,
            DataVenda = DateTime.UtcNow,
            QrCodeBase64 = pedido.QrCodeBase64,
            ContaCriada = contaCriada,
            SenhaInicial = contaCriada ? SenhaInicialPadrao : null,
            FormaPagamento = formaPagamento
        };
    }

    // --- LISTAR TODAS AS VENDAS DE BALCAO ---
    public async Task<List<VendaRelatorioBalcaoDto>> ListarVendasAsync(CancellationToken cancellationToken = default)
    {
        return await _pdv.VendasBalcao
            .OrderByDescending(v => v.DataVenda)
            .Select(v => new VendaRelatorioBalcaoDto
            {
                Id = v.Id,
                CodigoTicket = v.CodigoTicket,
                DataVenda = v.DataVenda,
                EventoNome = v.EventoTitulo,
                Setor = v.Setor,
                TipoEntrada = v.TipoEntrada,
                Quantidade = v.Quantidade,
                OperadorNome = v.OperadorNome,
                ClienteNome = (v.ClienteNome + " " + v.ClienteSobrenome).Trim(),
                MetodoPagamento = v.FormaPagamento,
                Valor = v.ValorTotal
            })
            .ToListAsync(cancellationToken);
    }

    // --- RESUMO AGREGADO POR EVENTO ---
    public async Task<List<RelatorioEventoBalcaoDto>> ResumoPorEventoAsync(CancellationToken cancellationToken = default)
    {
        var vendas = await _pdv.VendasBalcao.ToListAsync(cancellationToken);
        if (vendas.Count == 0)
        {
            return new List<RelatorioEventoBalcaoDto>();
        }

        var eventoIds = vendas.Select(v => v.EventoId).Distinct().ToList();
        var eventos = await _web.Eventos
            .Where(e => eventoIds.Contains(e.Id))
            .ToDictionaryAsync(e => e.Id, cancellationToken);

        return vendas
            .GroupBy(v => v.EventoId)
            .Select(g =>
            {
                var quantidade = g.Count();
                var faturamento = g.Sum(v => v.ValorTotal);
                eventos.TryGetValue(g.Key, out var ev);

                return new RelatorioEventoBalcaoDto
                {
                    EventoId = g.Key,
                    EventoNome = ev?.Titulo ?? g.First().EventoTitulo,
                    EventoLocal = ev?.Local ?? string.Empty,
                    EventoData = ev?.DataEvento ?? default,
                    QuantidadeVendas = quantidade,
                    FaturamentoTotal = faturamento,
                    TicketMedio = quantidade > 0 ? faturamento / quantidade : 0m
                };
            })
            .OrderByDescending(r => r.FaturamentoTotal)
            .ToList();
    }

    // --- RESUMO AGREGADO POR ATENDENTE ---
    public async Task<List<RelatorioAtendenteBalcaoDto>> ResumoPorAtendenteAsync(CancellationToken cancellationToken = default)
    {
        var vendas = await _pdv.VendasBalcao.ToListAsync(cancellationToken);
        if (vendas.Count == 0)
        {
            return new List<RelatorioAtendenteBalcaoDto>();
        }

        var operadorIds = vendas.Select(v => v.OperadorId).Distinct().ToList();
        var operadores = await _pdv.Operadores
            .Where(o => operadorIds.Contains(o.Id))
            .ToDictionaryAsync(o => o.Id, cancellationToken);

        return vendas
            .GroupBy(v => v.OperadorId)
            .Select(g =>
            {
                var quantidade = g.Count();
                var faturamento = g.Sum(v => v.ValorTotal);
                operadores.TryGetValue(g.Key, out var op);

                return new RelatorioAtendenteBalcaoDto
                {
                    OperadorId = g.Key,
                    OperadorNome = op?.Nome ?? g.First().OperadorNome,
                    OperadorLogin = op?.Login ?? string.Empty,
                    QuantidadeVendas = quantidade,
                    FaturamentoTotal = faturamento,
                    TicketMedio = quantidade > 0 ? faturamento / quantidade : 0m
                };
            })
            .OrderByDescending(r => r.FaturamentoTotal)
            .ToList();
    }

    // --- GERA O QR CODE EM BASE64 ---
    private static string GerarQrCodeBase64(string payload)
    {
        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
        var qrCode = new PngByteQRCode(data);
        var bytes = qrCode.GetGraphic(20);
        return Convert.ToBase64String(bytes);
    }
}

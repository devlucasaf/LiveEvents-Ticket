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
        // --- VALIDA TIPO DE ENTRADA ---
        var tipoEntrada = (dto.TipoEntrada ?? "INTEIRA").Trim().ToUpperInvariant();
        if (tipoEntrada != "INTEIRA" && tipoEntrada != "MEIA")
        {
            throw new InvalidOperationException("Tipo de entrada inválido. Use INTEIRA ou MEIA.");
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

        if (ingresso.QuantidadeDisponivel < dto.Quantidade)
        {
            throw new InvalidOperationException($"Estoque insuficiente para o setor {ingresso.Setor}. Disponível: {ingresso.QuantidadeDisponivel}.");
        }

        // --- ENCONTRA OU CRIA O CLIENTE ---
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

        var precoUnitario = tipoEntrada == "MEIA"
            ? decimal.Round(ingresso.Preco / 2m, 2)
            : ingresso.Preco;
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

        // --- REGISTRA O PAGAMENTO E GERA O QR CODE ---
        _web.Pagamentos.Add(new PagamentoWeb
        {
            PedidoId = pedido.Id,
            Tipo = "BALCAO",
            Status = "APROVADO",
            DataPagamento = DateTime.UtcNow
        });

        pedido.QrCodeBase64 = GerarQrCodeBase64($"pedido:{pedido.Id}|usuario:{pedido.UsuarioId}|valor:{pedido.ValorTotal}");
        await _web.SaveChangesAsync(cancellationToken);

        // --- GERA O CODIGO DO TICKET E GRAVA O REGISTRO LOCAL ---
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
            Quantidade = dto.Quantidade,
            ValorUnitario = precoUnitario,
            ValorTotal = valorTotal,
            CodigoTicket = codigoTicket,
            DataVenda = DateTime.UtcNow
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
            SenhaInicial = contaCriada ? SenhaInicialPadrao : null
        };
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

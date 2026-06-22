using Microsoft.EntityFrameworkCore;
using PontoVenda.Backend.Infra.Config;
using PontoVenda.Backend.Infra.Security;
using PontoVenda.Backend.Modules.PontoVenda.Dto;
using PontoVenda.Backend.Modules.PontoVenda.Model;
using PontoVenda.Backend.Modules.PontoVenda.Repository;

namespace PontoVenda.Backend.Modules.PontoVenda.Service;

public class VendaFisicaService
{
    private readonly IVendaFisicaRepository _repository;
    private readonly AppDbContext _context;
    private readonly CurrentUserService _currentUser;

    // --- INJEÇÃO DE DEPENDÊNCIAS ---
    public VendaFisicaService(
        IVendaFisicaRepository repository,
        AppDbContext context,
        CurrentUserService currentUser)
    {
        _repository  = repository;
        _context     = context;
        _currentUser = currentUser;
    }

    // --- REGISTRAR UMA NOVA VENDA FÍSICA ---
    public async Task<VendaFisicaRespostaDto> RegistrarAsync(CriarVendaFisicaDto dto, CancellationToken cancellationToken)
    {
        // --- VALIDA EVENTO ATIVO ---
        var evento = await _context.Eventos
            .FirstOrDefaultAsync(e => e.Id == dto.EventoId, cancellationToken)
            ?? throw new KeyNotFoundException("Evento não encontrado.");

        if (!evento.Ativo)
        {
            throw new InvalidOperationException("Evento não está ativo para vendas.");
        }

        // --- VALIDA ASSENTO PERTENCE AO EVENTO ---
        var assento = await _context.Assentos
            .FirstOrDefaultAsync(a => a.Id == dto.AssentoId, cancellationToken)
            ?? throw new KeyNotFoundException("Assento não encontrado.");

        if (assento.EventoId != dto.EventoId)
        {
            throw new InvalidOperationException("O assento informado não pertence ao evento.");
        }

        // --- VALIDA DISPONIBILIDADE ---
        if (assento.Status != "DISPONIVEL")
        {
            throw new InvalidOperationException($"Assento indisponível (status atual: {assento.Status}).");
        }

        if (await _repository.AssentoJaVendidoAsync(dto.AssentoId, cancellationToken))
        {
            throw new InvalidOperationException("Assento já foi vendido.");
        }

        // --- VALIDA QUE O VALOR ENVIADO CONFERE COM O PREÇO DO ASSENTO ---
        if (dto.Valor != assento.Preco)
        {
            throw new InvalidOperationException(
                $"Valor informado (R$ {dto.Valor:F2}) não confere com o preço do assento (R$ {assento.Preco:F2}).");
        }

        // --- REGISTRA A VENDA FÍSICA ---
        var venda = new VendaFisica
        {
            EventoId        = dto.EventoId,
            AssentoId       = dto.AssentoId,
            OperadorId      = _currentUser.GetUserId(),
            MetodoPagamento = dto.MetodoPagamento,
            Valor           = dto.Valor,
            DataVenda       = DateTime.UtcNow
        };

        // --- BAIXA NO ASSENTO ---
        assento.Status      = "VENDIDO";
        assento.UpdatedAt   = DateTime.UtcNow;

        await _repository.AdicionarAsync(venda, cancellationToken);

        var operador = await _context.Operadores
            .FirstOrDefaultAsync(o => o.Id == venda.OperadorId, cancellationToken);

        return Map(venda, evento, assento, operador);
    }

    // --- LISTAR TODAS AS VENDAS FÍSICAS ---
    public async Task<List<VendaFisicaRespostaDto>> ListarAsync(CancellationToken cancellationToken)
    {
        var vendas = await _repository.ListarAsync(cancellationToken);
        return await EnriquecerAsync(vendas, cancellationToken);
    }

    // --- LISTAR VENDAS DE UM EVENTO ---
    public async Task<List<VendaFisicaRespostaDto>> ListarPorEventoAsync(Guid eventoId, CancellationToken cancellationToken)
    {
        var vendas = await _repository.ListarPorEventoAsync(eventoId, cancellationToken);
        return await EnriquecerAsync(vendas, cancellationToken);
    }

    // --- LISTAR VENDAS DE UM ATENDENTE ---
    public async Task<List<VendaFisicaRespostaDto>> ListarPorAtendenteAsync(int operadorId, CancellationToken cancellationToken)
    {
        var vendas = await _repository.ListarPorOperadorAsync(operadorId, cancellationToken);
        return await EnriquecerAsync(vendas, cancellationToken);
    }

    // --- RESUMO AGREGADO POR EVENTO ---
    public async Task<List<RelatorioPorEventoDto>> ResumoPorEventoAsync(CancellationToken cancellationToken)
    {
        var vendas = await _repository.ListarAsync(cancellationToken);
        if (vendas.Count == 0)
        {
            return new List<RelatorioPorEventoDto>();
        }

        var eventoIds = vendas.Select(v => v.EventoId).Distinct().ToList();
        var eventos = await _context.Eventos
            .Where(e => eventoIds.Contains(e.Id))
            .ToDictionaryAsync(e => e.Id, cancellationToken);

        return vendas
            .GroupBy(v => v.EventoId)
            .Select(g =>
            {
                var qtd        = g.Count();
                var faturamento = g.Sum(v => v.Valor);
                eventos.TryGetValue(g.Key, out var ev);

                return new RelatorioPorEventoDto
                {
                    EventoId            = g.Key,
                    EventoNome          = ev?.Nome      ?? string.Empty,
                    EventoLocal         = ev?.Local     ?? string.Empty,
                    EventoData          = ev?.DataEvento ?? default,
                    QuantidadeVendas    = qtd,
                    FaturamentoTotal    = faturamento,
                    TicketMedio         = qtd > 0 ? faturamento / qtd : 0m
                };
            })
            .OrderByDescending(r => r.FaturamentoTotal)
            .ToList();
    }

    // --- RESUMO AGREGADO POR ATENDENTE ---
    public async Task<List<RelatorioPorAtendenteDto>> ResumoPorAtendenteAsync(CancellationToken cancellationToken)
    {
        var vendas = await _repository.ListarAsync(cancellationToken);
        if (vendas.Count == 0)
        {
            return new List<RelatorioPorAtendenteDto>();
        }

        var operadorIds = vendas.Select(v => v.OperadorId).Distinct().ToList();
        var operadores = await _context.Operadores
            .Where(o => operadorIds.Contains(o.Id))
            .ToDictionaryAsync(o => o.Id, cancellationToken);

        return vendas
            .GroupBy(v => v.OperadorId)
            .Select(g =>
            {
                var qtd        = g.Count();
                var faturamento = g.Sum(v => v.Valor);
                operadores.TryGetValue(g.Key, out var op);

                return new RelatorioPorAtendenteDto
                {
                    OperadorId          = g.Key,
                    OperadorNome        = op?.Nome  ?? "(desconhecido)",
                    OperadorLogin       = op?.Login ?? string.Empty,
                    QuantidadeVendas    = qtd,
                    FaturamentoTotal    = faturamento,
                    TicketMedio         = qtd > 0 ? faturamento / qtd : 0m
                };
            })
            .OrderByDescending(r => r.FaturamentoTotal)
            .ToList();
    }

    // --- ENRIQUECE LISTA DE VENDAS COM EVENTO ---
    private async Task<List<VendaFisicaRespostaDto>> EnriquecerAsync(
        List<VendaFisica> vendas,
        CancellationToken cancellationToken)
    {
        if (vendas.Count == 0)
        {
            return new List<VendaFisicaRespostaDto>();
        }

        var eventoIds = vendas.Select(v => v.EventoId).Distinct().ToList();
        var assentoIds = vendas.Select(v => v.AssentoId).Distinct().ToList();
        var operadorIds = vendas.Select(v => v.OperadorId).Distinct().ToList();

        var eventos = await _context.Eventos
            .Where(e => eventoIds.Contains(e.Id))
            .ToDictionaryAsync(e => e.Id, cancellationToken);

        var assentos = await _context.Assentos
            .Where(a => assentoIds.Contains(a.Id))
            .ToDictionaryAsync(a => a.Id, cancellationToken);

        var operadores = await _context.Operadores
            .Where(o => operadorIds.Contains(o.Id))
            .ToDictionaryAsync(o => o.Id, cancellationToken);

        return vendas
            .Select(v => Map(
                v,
                eventos.TryGetValue(v.EventoId, out var e)    ? e : null,
                assentos.TryGetValue(v.AssentoId, out var a)  ? a : null,
                operadores.TryGetValue(v.OperadorId, out var op) ? op : null))
            .ToList();
    }

    // --- BUSCAR UMA VENDA FÍSICA PELO ID ---
    public async Task<VendaFisicaRespostaDto?> BuscarPorIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var venda = await _repository.BuscarPorIdAsync(id, cancellationToken);
        if (venda is null)
        {
            return null;
        }

        var evento = await _context.Eventos.FirstOrDefaultAsync(e => e.Id == venda.EventoId, cancellationToken);
        var assento = await _context.Assentos.FirstOrDefaultAsync(a => a.Id == venda.AssentoId, cancellationToken);
        var operador = await _context.Operadores.FirstOrDefaultAsync(o => o.Id == venda.OperadorId, cancellationToken);

        return Map(venda, evento, assento, operador);
    }

    // --- MAPEAR ENTIDADE PARA DTO DE RESPOSTA ---
    private static VendaFisicaRespostaDto Map(VendaFisica venda, Evento? evento, Assento? assento, Operador? operador)
    {
        return new VendaFisicaRespostaDto
        {
            Id = venda.Id,
            CodigoTicket = GerarCodigoTicket(venda.Id),
            EventoId = venda.EventoId,
            EventoNome = evento?.Nome ?? string.Empty,
            EventoLocal = evento?.Local ?? string.Empty,
            EventoData = evento?.DataEvento ?? default,
            AssentoId = venda.AssentoId,
            AssentoSetor = assento?.Setor ?? string.Empty,
            AssentoFileira = assento?.Fileira ?? string.Empty,
            AssentoNumero = assento?.Numero ?? 0,
            OperadorId = venda.OperadorId,
            OperadorNome = operador?.Nome ?? string.Empty,
            MetodoPagamento = venda.MetodoPagamento.ToString(),
            Valor = venda.Valor,
            DataVenda = venda.DataVenda
        };
    }

    // --- GERA UM CÓDIGO LEGÍVEL A PARTIR DO ID DA VENDA ---
    private static string GerarCodigoTicket(Guid id)
    {
        return "TKT-" + id.ToString("N").Substring(0, 12).ToUpperInvariant();
    }
}

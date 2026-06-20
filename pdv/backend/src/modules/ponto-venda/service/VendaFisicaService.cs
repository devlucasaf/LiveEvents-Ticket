using Microsoft.EntityFrameworkCore;
using PontoVenda.Backend.Infra.Config;
using PontoVenda.Backend.Modules.PontoVenda.Dto;
using PontoVenda.Backend.Modules.PontoVenda.Model;
using PontoVenda.Backend.Modules.PontoVenda.Repository;

namespace PontoVenda.Backend.Modules.PontoVenda.Service;

public class VendaFisicaService
{
    private readonly IVendaFisicaRepository _repository;
    private readonly AppDbContext _context;

    // --- INJEÇÃO DE DEPENDÊNCIAS ---
    public VendaFisicaService(IVendaFisicaRepository repository, AppDbContext context)
    {
        _repository = repository;
        _context = context;
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

        // --- REGISTRA A VENDA FÍSICA ---
        var venda = new VendaFisica
        {
            EventoId        = dto.EventoId,
            AssentoId       = dto.AssentoId,
            MetodoPagamento = dto.MetodoPagamento,
            Valor           = assento.Preco,
            DataVenda       = DateTime.UtcNow
        };

        // --- BAIXA NO ASSENTO ---
        assento.Status      = "VENDIDO";
        assento.UpdatedAt   = DateTime.UtcNow;

        await _repository.AdicionarAsync(venda, cancellationToken);

        return Map(venda);
    }

    // --- LISTAR TODAS AS VENDAS FÍSICAS ---
    public async Task<List<VendaFisicaRespostaDto>> ListarAsync(CancellationToken cancellationToken)
    {
        var vendas = await _repository.ListarAsync(cancellationToken);
        return vendas.Select(Map).ToList();
    }

    // --- MAPEAR ENTIDADE PARA DTO DE RESPOSTA ---
    private static VendaFisicaRespostaDto Map(VendaFisica venda) => new()
    {
        Id = venda.Id,
        EventoId = venda.EventoId,
        AssentoId = venda.AssentoId,
        MetodoPagamento = venda.MetodoPagamento.ToString(),
        Valor = venda.Valor,
        DataVenda = venda.DataVenda
    };
}

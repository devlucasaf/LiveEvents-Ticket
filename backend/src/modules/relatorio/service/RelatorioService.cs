using LiveEventsTicket.Backend.Infra.Config;
using LiveEventsTicket.Backend.Modules.Relatorio.Dto;
using Microsoft.EntityFrameworkCore;

namespace LiveEventsTicket.Backend.Modules.Relatorio.Service;

public class RelatorioService
{
    private readonly AppDbContext _context;

    public RelatorioService(AppDbContext context)
    {
        _context = context;
    }

    // --- GERAR RELATÓRIO DE VENDAS ---
    public async Task<RelatorioVendasDto> GerarVendasAsync(CancellationToken cancellationToken = default)
    {
        // --- BUSCAR TODOS OS PEDIDOS COM STATUS PAGO ---
        var pedidosPagos = await _context.Pedidos
            .Include(p => p.Itens)
            .Where(p => p.Status == "PAGO")
            .ToListAsync(cancellationToken);

        var ingressos = await _context.Ingressos.ToListAsync(cancellationToken);
        var eventos = await _context.Eventos.ToListAsync(cancellationToken);

        // --- AGRUPAR VENDAS POR EVENTO ---
        var porEvento = pedidosPagos
            .SelectMany(p => p.Itens)
            .Join(ingressos,
                item => item.IngressoId,
                ingresso => ingresso.Id,
                (item, ingresso) => new { item, ingresso })
            .Join(eventos,
                itemIngresso => itemIngresso.ingresso.EventoId,
                evento => evento.Id,
                (itemIngresso, evento) => new { itemIngresso.item, evento })
            .GroupBy(x => x.evento.Titulo)
            .Select(g => new ResumoEventoDto
            {
                Evento = g.Key,
                QuantidadeIngressosVendidos = g.Sum(x => x.item.Quantidade),
                Receita = g.Sum(x => x.item.Quantidade * x.item.PrecoUnitario)
            })
            .OrderByDescending(r => r.Receita)
            .ToList();

        // --- RETORNAR RELATÓRIO COM TOTAIS E DETALHAMENTO POR EVENTO ---
        return new RelatorioVendasDto
        {
            TotalPedidos = pedidosPagos.Count,
            ReceitaTotal = pedidosPagos.Sum(p => p.ValorTotal),
            Eventos = porEvento
        };
    }
}

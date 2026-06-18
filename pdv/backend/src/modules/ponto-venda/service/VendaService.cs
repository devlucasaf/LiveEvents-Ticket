using PontoVenda.Backend.Modules.PontoVenda.Dto;
using PontoVenda.Backend.Modules.PontoVenda.Model;
using PontoVenda.Backend.Modules.PontoVenda.Repository;

namespace PontoVenda.Backend.Modules.PontoVenda.Service;

public class VendaService
{
    private readonly IVendaRepository   _vendaRepository;
    private readonly IProdutoRepository _produtoRepository;

    // --- INJEÇÃO DOS REPOSITORIES ---
    public VendaService(IVendaRepository vendaRepository, IProdutoRepository produtoRepository)
    {
        _vendaRepository = vendaRepository;
        _produtoRepository = produtoRepository;
    }

    // --- REGISTRAR NOVA VENDA, BAIXAR ESTOQUE E CALCULAR TOTAL ---
    public async Task<VendaRespostaDto> RegistrarAsync(int operadorId, CriarVendaDto dto, CancellationToken cancellationToken = default)
    {
        if (dto.Itens.Count == 0)
        {
            throw new InvalidOperationException("A venda deve conter ao menos um item.");
        }

        var venda = new Venda
        {
            OperadorId = operadorId,
            FormaPagamento = dto.FormaPagamento
        };

        // --- VALIDAR E REGISTRAR CADA ITEM ---
        foreach (var item in dto.Itens)
        {
            var produto = await _produtoRepository.BuscarPorIdAsync(item.ProdutoId, cancellationToken)
                ?? throw new KeyNotFoundException($"Produto {item.ProdutoId} não encontrado.");

            if (item.Quantidade <= 0 || produto.EstoqueAtual < item.Quantidade)
            {
                throw new InvalidOperationException($"Estoque insuficiente para o produto {produto.Nome}.");
            }

            produto.EstoqueAtual -= item.Quantidade;

            venda.Itens.Add(new ItemVenda
            {
                ProdutoId = produto.Id,
                Quantidade = item.Quantidade,
                PrecoUnitario = produto.Preco
            });
        }

        // --- CALCULAR VALOR TOTAL DA VENDA ---
        venda.ValorTotal = venda.Itens.Sum(i => i.Quantidade * i.PrecoUnitario);

        await _vendaRepository.AdicionarAsync(venda, cancellationToken);
        await _produtoRepository.AtualizarAsync(cancellationToken);

        return new VendaRespostaDto
        {
            Id = venda.Id,
            ValorTotal = venda.ValorTotal,
            FormaPagamento = venda.FormaPagamento,
            Status = venda.Status,
            DataVenda = venda.DataVenda,
            QuantidadeItens = venda.Itens.Sum(i => i.Quantidade)
        };
    }

    // --- LISTAR TODAS AS VENDAS ---
    public async Task<List<VendaRespostaDto>> ListarAsync(CancellationToken cancellationToken = default)
    {
        var vendas = await _vendaRepository.ListarAsync(cancellationToken);
        return vendas.Select(v => new VendaRespostaDto
        {
            Id = v.Id,
            ValorTotal = v.ValorTotal,
            FormaPagamento = v.FormaPagamento,
            Status = v.Status,
            DataVenda = v.DataVenda,
            QuantidadeItens = v.Itens.Sum(i => i.Quantidade)
        }).ToList();
    }
}

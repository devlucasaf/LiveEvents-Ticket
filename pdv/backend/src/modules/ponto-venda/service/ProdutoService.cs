using PontoVenda.Backend.Modules.PontoVenda.Dto;
using PontoVenda.Backend.Modules.PontoVenda.Model;
using PontoVenda.Backend.Modules.PontoVenda.Repository;

namespace PontoVenda.Backend.Modules.PontoVenda.Service;

public class ProdutoService
{
    private readonly IProdutoRepository _repository;

    // --- INJEÇÃO DO REPOSITORY DE PRODUTOS ---
    public ProdutoService(IProdutoRepository repository)
    {
        _repository = repository;
    }

    // --- LISTAR PRODUTOS ATIVOS ---
    public async Task<List<ProdutoDto>> ListarAsync(CancellationToken cancellationToken = default)
    {
        var produtos = await _repository.ListarAsync(cancellationToken);
        return produtos.Select(Map).ToList();
    }

    // --- BUSCAR PRODUTO POR CÓDIGO DE BARRAS ---
    public async Task<ProdutoDto> BuscarPorCodigoBarrasAsync(string codigoBarras, CancellationToken cancellationToken = default)
    {
        var produto = await _repository.BuscarPorCodigoBarrasAsync(codigoBarras, cancellationToken)
            ?? throw new KeyNotFoundException("Produto não encontrado.");
        return Map(produto);
    }

    // --- CRIAR NOVO PRODUTO ---
    public async Task<ProdutoDto> CriarAsync(ProdutoDto dto, CancellationToken cancellationToken = default)
    {
        var existente = await _repository.BuscarPorCodigoBarrasAsync(dto.CodigoBarras, cancellationToken);
        if (existente is not null)
        {
            throw new InvalidOperationException("Já existe um produto com este código de barras.");
        }

        var produto = new Produto
        {
            Nome = dto.Nome,
            CodigoBarras = dto.CodigoBarras,
            Preco = dto.Preco,
            EstoqueAtual = dto.EstoqueAtual,
            Ativo = dto.Ativo
        };

        await _repository.AdicionarAsync(produto, cancellationToken);
        return Map(produto);
    }

    // --- MAPEAR ENTIDADE PARA DTO ---
    private static ProdutoDto Map(Produto produto) => new()
    {
        Id = produto.Id,
        Nome = produto.Nome,
        CodigoBarras = produto.CodigoBarras,
        Preco = produto.Preco,
        EstoqueAtual = produto.EstoqueAtual,
        Ativo = produto.Ativo
    };
}

using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Pagination;

namespace APICatalogo.Repositories;

// Herda de GenericRepository e implementa ICategoriaRepository
public class ProdutoRepository : GenericRepository<Produto>, IProdutoRepository
{
    //IMPLEMENTAÇÃO DIFERENTE DA DE CATEGORIAS, PARA PODER VER MODOS DIFERENTE DE FAZER

    public ProdutoRepository(AppDbContext context) : base(context)
    {
    }

    public IEnumerable<Produto> GetProdutos(ProdutosParameters produtosParams)
    {
        return GetAll()
            .OrderBy(p => p.Nome)
            .Skip((produtosParams.PageNumber - 1) * produtosParams.PageSize) // Pular os da pagina anterior
            .Take(produtosParams.PageSize).ToList();
    }

    public IEnumerable<Produto> GetProdutosPorCategoria(int id)
    {
        return GetAll().Where(c => c.CategoriaId == id);
    }
}

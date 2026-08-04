using APICatalogo.Context;
using APICatalogo.Models;

namespace APICatalogo.Repositories;

// Herda de GenericRepository e implementa ICategoriaRepository
public class ProdutoRepository : GenericRepository<Produto>, IProdutoRepository
{
    //IMPLEMENTAÇÃO DIFERENTE DA DE CATEGORIAS, PARA PODER VER MODOS DIFERENTE DE FAZER

    public ProdutoRepository(AppDbContext context) : base(context)
    {
    }

    public IEnumerable<Produto> GetProdutosPorCategoria(int id)
    {
        return GetAll().Where(c => c.CategoriaId == id);
    }
}

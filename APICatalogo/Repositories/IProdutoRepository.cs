using APICatalogo.Models;

namespace APICatalogo.Repositories;

public interface IProdutoRepository : IGenericRepository<Produto>
{
    IEnumerable<Produto> GetProdutosPorCategoria (int id);
}

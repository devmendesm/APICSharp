using APICatalogo.Context;

namespace APICatalogo.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private IProdutoRepository? _produtoRepo;

    private ICategoriaRepository? _categoriaRepo;
    public AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    /* Lazy Loading
     * Adia a obtenção dos objetos até eles realmente serem necessários
     * Com essa abordagem elimina as multiplas instâncias, evitando a concorrência
     */
    public IProdutoRepository ProdutoRepository
    {
        get { return _produtoRepo = _produtoRepo ?? new ProdutoRepository(_context); }
    }

    public ICategoriaRepository CategoriaRepository
    {
        get { return _categoriaRepo = _categoriaRepo ?? new CategoriaRepository(_context); }
    }

    public async Task CommitAsync()
    {
       await _context.SaveChangesAsync();
    }

    // Libera recursos alocados do DbContext
    public void Dispose()
    {
        _context.Dispose();
    }
}

using APICatalogo.Context;
using APICatalogo.Models;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Repositories;

// Herda de GenericRepository e implementa ICategoriaRepository
public class CategoriaRepository : GenericRepository<Categoria>, ICategoriaRepository
{
    
    public CategoriaRepository(AppDbContext context) : base(context)
    {
    }

}

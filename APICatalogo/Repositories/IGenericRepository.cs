using System.Linq.Expressions;

namespace APICatalogo.Repositories;

// Interface que permitira uma implementação mais genérica para a aplicação
public interface IGenericRepository<T>
{
    // As funções get utilizam acesso ao DB, por isso são asyncs
    Task<IEnumerable<T>> GetAllAsync();

    /* Expression -> permite a função entender o lambda
     * Func -> Um delegate, uma função que pode ser passada como argumento, expressão lambda que recebe objeto do tipo T,
     * e vai retornar um boolean
     * EX: _repo.Get(c => c.CategoriaId == id);
     */
    Task<T?> GetAsync(Expression<Func<T, bool>> predicate);

    // As seguintes funções utilizam o contexto do EF, o que faz o acesso ao DB é o saveChanges da UnitOfWork, por isso não vão ser async
    T Create(T entity);
    T Update(T entity);
    T Delete(T entity);
}

using System.Linq.Expressions;

namespace APICatalogo.Repositories;

public interface IGenericRepository<T>
{
    // Interface que permitira uma implementação mais genérica para a aplicação
    IEnumerable<T> GetAll();

    /* Expression -> permite a função entender o lambda
     * Func -> Um delegate, uma função que pode ser passada como argumento, expressão lambda que recebe objeto do tipo T,
     * e vai retornar um boolean
     * EX: _repo.Get(c => c.CategoriaId == id);
     */
    T? Get(Expression<Func<T, bool>> predicate);
    T Create(T entity);
    T Update(T entity);
    T Delete(T entity);
}

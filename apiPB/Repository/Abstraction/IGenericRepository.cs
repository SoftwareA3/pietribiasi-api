using System.Linq.Expressions;

namespace apiPB.Repository.Abstraction
{
    public interface IGenericRepository<T>
    {
        // Classe che definisce un repository generico per le operazioni CRUD
        // e altre operazioni comuni su un'entità di tipo T.

        // Performa una query "Where" su un'entità di tipo T e restituisce una lista di entità di tipo C.
        IEnumerable<T> GetFiltered(Expression<Func<T, bool>> predicate, bool distinct = false);
    }
}
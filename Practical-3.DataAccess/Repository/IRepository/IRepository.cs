using System.Linq.Expressions;

namespace Practical_3.DataAccess.Repository.IRepository
{ 
    public interface IRepository<T> where T : class
    {
        void Add(T entity);

        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);

        T GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties = null);
    }
}

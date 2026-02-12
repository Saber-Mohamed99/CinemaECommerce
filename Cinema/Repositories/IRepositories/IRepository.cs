using System.Linq.Expressions;

namespace CinemaECommerce.Repositories.IRepositories
{
    public interface IRepository<T> where T : class
    {
        Task CreateAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<List<T>> GetAsync(Expression<Func<T, bool>>? expression = null,
          bool tracked = true, Expression<Func<T, object>>[]? includes = null);

        Task<T> GetOneAsync(Expression<Func<T, bool>>? expression = null,
           bool tracked = true, Expression<Func<T, object>>[]? include = null);

        Task<int> CommitAsync();

    }
}

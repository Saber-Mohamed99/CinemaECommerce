using CinemaECommerce.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CinemaECommerce.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private DbSet<T> _dbSet;
        public Repository(ApplicationDbContext context)
        { 
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<int> CommitAsync()
        {
            try
            {

                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error{ex.Message}");
                return 0;
            }
        }

        public async Task CreateAsync(T entity)
        {
           await _dbSet.AddAsync(entity);
        }
        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<List<T>> GetAsync(System.Linq.Expressions.Expression<Func<T, bool>>? expression = null,
            bool tracked = true, System.Linq.Expressions.Expression<Func<T, object>>[]? includes = null)
        {
          var objects =_dbSet.AsQueryable();
            if(expression is not null)
                objects= objects.Where(expression);
            if (!tracked)
                objects = objects.AsNoTracking();
            if(includes != null)
                foreach( var include in includes)
                  objects = objects.Include(include);
            return await objects.ToListAsync();
        }

        public async Task<T> GetOneAsync(Expression<Func<T, bool>>? expression = null, 
            bool tracked = true,Expression<Func<T, object>>[]? includes = null)
        {
            return (await GetAsync(expression, tracked,includes)).FirstOrDefault();
        }

    }
}

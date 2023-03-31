using RPU.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace RPU.DataAccess.Repository;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly AppDbContext _db;
    internal DbSet<T> dbSet;
    public Repository(AppDbContext db)
    {
        _db = db;
        //_db.ShoppingCarts.AsNoTracking().FirstOrDefault(); // AsNoTracking - EntityFramework nie śledzi zmiennej
        this.dbSet = _db.Set<T>();
    }

    public void Add(T entity)
    {
        //_db.Set<T>.Add(entity); np. _db.Categories.Add(entity);
        dbSet.Add(entity);
    }

    // includeProperties - "Category,CoverType"
    public IEnumerable<T> GetAll(Expression<Func<T, bool>> filter = null, string? includeProperties = null)
    {
        IQueryable<T> query = dbSet;

        if (filter is not null)
        {
            query = query.Where(filter);
        }

        if (includeProperties != null)
        {
            foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProp);
            }
        }
        return query.ToList();
    }

    public T GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = true)
    {
        IQueryable<T> query;

        if (tracked)
        {
            query = dbSet;
        }
        else
        {
            query = dbSet.AsNoTracking();  // AsNoTracking - EntityFramework nie śledzi zmiennej
        }

        query = query.Where(filter);
        if (includeProperties != null)
        {
            foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProp);
            }
        }
        return query.FirstOrDefault();
    }

    public void Remove(T entity)
    {
        dbSet.Remove(entity);
    }

    public void RemoveRange(IEnumerable<T> entities)
    {
        dbSet.RemoveRange(entities);
    }
}

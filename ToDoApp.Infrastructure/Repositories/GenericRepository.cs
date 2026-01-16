using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using ToDoApp.Application.IRepo;
using ToDoApp.Infrastructure.Data;

namespace ToDoApp.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T>
    where T : class
{
    protected readonly AppDbContext _context;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<T?> GetByIdAsync(object id)
        => await _context.Set<T>().FindAsync(id);

    public async Task<IReadOnlyList<T>> GetAllAsync()
        => await _context.Set<T>().ToListAsync();

    public async Task AddAsync(T entity)
        => await _context.Set<T>().AddAsync(entity);

    public void Update(T entity)
        => _context.Set<T>().Update(entity);

    public void Delete(T entity)
        => _context.Set<T>().Remove(entity);

    public async Task<bool> ExistsAsync(
        System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        => await _context.Set<T>().AnyAsync(predicate);
}


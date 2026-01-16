using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Linq.Expressions;

namespace ToDoApp.Application.IRepo;

public interface IGenericRepository<T> where T : class
{
        Task<T?> GetByIdAsync(object id);
        Task<IReadOnlyList<T>> GetAllAsync();
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);

        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    }


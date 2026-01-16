using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Domain.Entities;


namespace ToDoApp.Application.IRepo;
public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetByIdAsync(string id);
    Task CreateAsync(User user, string password);
    Task UpdateAsync(User user);
    Task DeleteAsync(User user);
}

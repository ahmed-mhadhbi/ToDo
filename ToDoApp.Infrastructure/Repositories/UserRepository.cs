using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using ToDoApp.Application.IRepo;

using TodoApp.Domain.Entities;

namespace ToDoApp.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserManager<User> _userManager;

    public UserRepository(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IEnumerable<User>> GetAllAsync()
        => _userManager.Users.ToList();

    public async Task<User?> GetByIdAsync(string id)
        => await _userManager.FindByIdAsync(id);

    public async Task CreateAsync(User user, string password)
        => await _userManager.CreateAsync(user, password);

    public async Task UpdateAsync(User user)
        => await _userManager.UpdateAsync(user);

    public async Task DeleteAsync(User user)
        => await _userManager.DeleteAsync(user);
}

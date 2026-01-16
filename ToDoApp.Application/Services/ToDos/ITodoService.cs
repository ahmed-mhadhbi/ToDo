using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TodoApp.Domain.Entities;

namespace ToDoApp.Application.Services.ToDos;

public interface ITodoService
{
    Task CreateAsync(string userId, string title);
    Task<List<Todo>> GetByUserIdAsync(string userId);
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApp.Domain.Entities;
using ToDoApp.Domain;

namespace ToDoApp.Application.IRepo;

public interface ITodoRepository
{
    Task AddAsync(Todo todo);
    Task<List<Todo>> GetByUserIdAsync(Guid userId);
}
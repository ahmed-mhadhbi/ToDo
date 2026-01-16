using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using ToDoApp.Application.IRepo;
using ToDoApp.Domain;
using TodoApp.Domain.Entities;
using ToDoApp.Infrastructure.Data;

namespace ToDoApp.Infrastructure.Repositories;

public class TodoRepository : ITodoRepository
{
    private readonly AppDbContext _context;

    public TodoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Todo todo)
    {
        await _context.Todos.AddAsync(todo);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Todo>> GetByUserIdAsync(Guid userId)
    {
        return await _context.Todos
            .Where(t => t.UserId == userId.ToString())
            .ToListAsync();
    }
}

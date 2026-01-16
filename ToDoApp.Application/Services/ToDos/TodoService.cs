using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Entities;
using ToDoApp.Application.IRepo;


namespace ToDoApp.Application.Services.ToDos;


public class TodoService : ITodoService
{
    private readonly ITodoRepository _todoRepository;

    public TodoService(ITodoRepository todoRepository)
    {
        _todoRepository = todoRepository;
    }

    public async Task CreateAsync(string userId, string title)
    {
        if (!Guid.TryParse(userId, out var userGuid))
            throw new Exception("Invalid user ID format");

        var todo = new Todo
        {
            UserId = userGuid.ToString(), //  convert string -> Guid
            Title = title
        };

        await _todoRepository.AddAsync(todo);
    }



    public async Task<List<Todo>> GetByUserIdAsync(string userId)
    {
        if (!Guid.TryParse(userId, out var userGuid))
            throw new Exception("Invalid user ID format");

        return await _todoRepository.GetByUserIdAsync(userGuid);
    }
}
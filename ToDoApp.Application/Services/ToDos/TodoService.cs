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
        _todoRepository = todoRepository
            ?? throw new ArgumentNullException(nameof(todoRepository));
    }

    public async Task CreateAsync(string userId, string title)
    {
        try
        {
            // Validation
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID is required", nameof(userId));

            if (!Guid.TryParse(userId, out var userGuid))
                throw new ArgumentException("Invalid user ID format", nameof(userId));

            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title is required", nameof(title));

            var todo = new Todo
            {
                UserId = userGuid.ToString(),
                Title = title
            };

            await _todoRepository.AddAsync(todo);
        }
        catch (ArgumentException)
        {
            // Known validation errors → just bubble up
            throw;
        }
        catch (Exception ex)
        {
            // Infrastructure / unexpected errors
            throw new ApplicationException(
                "An error occurred while creating the todo item.",
                ex
            );
        }
    }

    public async Task<List<Todo>> GetByUserIdAsync(string userId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID is required", nameof(userId));

            if (!Guid.TryParse(userId, out var userGuid))
                throw new ArgumentException("Invalid user ID format", nameof(userId));

            return await _todoRepository.GetByUserIdAsync(userGuid);
        }
        catch (ArgumentException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ApplicationException(
                "An error occurred while retrieving todo items.",
                ex
            );
        }
    }
    
}

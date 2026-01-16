using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToDoApp.Application.DTOs;
using ToDoApp.Application.Services.ToDos;
using ToDoApp.Application.Common;

namespace ToDoApp.api.Controllers;

[ApiController]
[Route("api/todos")]
public class TodoController : ControllerBase
{
    private readonly ITodoService _todoService;

    public TodoController(ITodoService todoService)
    {
        _todoService = todoService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTodoDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized(ApiResponse<string>.Fail("User not logged in"));

        await _todoService.CreateAsync(userId, dto.Title);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetMyTodos()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var todos = await _todoService.GetByUserIdAsync(userId);
        return Ok(todos);
    }
}

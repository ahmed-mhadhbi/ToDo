using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToDoApp.Application.Common;
using ToDoApp.Application.DTOs;
using ToDoApp.Application.Services.ToDos;
using ToDoApp.Application.Services.Users;

namespace ToDoApp.api.Controllers;


[ApiController]
[Route("api/todos")]
public class TodoController : ControllerBase
{
    private readonly ITodoService _todoService;
    private readonly IUserService _userService ;
    public TodoController(ITodoService todoService, IUserService userService)
    {
        _todoService = todoService;
        _userService = userService;
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
        if (string.IsNullOrEmpty(userId))
            return Unauthorized("User not logged in");

        var todos = await _todoService.GetByUserIdAsync(userId);
        return Ok(todos);
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized("Invalid token");

        await _userService.ChangePasswordAsync(
            userId,
            dto.CurrentPassword,
            dto.NewPassword
        );
        return Ok(new { message = "Password updated successfully" });
    }

}

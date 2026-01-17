using Microsoft.AspNetCore.Identity;
using TodoApp.Domain.Entities;
using ToDoApp.Application.DTOs;

namespace ToDoApp.Application.Services.Users;
using ToDoApp.Application.Services.Users;

public interface IUserService
{
    Task<IdentityResult> RegisterAsync(RegisterUserDto dto);
    Task<List<User>> GetAllAsync();
    Task<User?> GetByIdAsync(string id);
    Task DeleteAsync(string id);
    
    Task<(string AccessToken, string RefreshToken)> LoginAsync(LoginUserDto dto);
    Task<(string AccessToken, string RefreshToken)> RefreshTokenAsync(string refreshToken);




}


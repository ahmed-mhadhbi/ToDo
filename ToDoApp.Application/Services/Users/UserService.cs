using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TodoApp.Domain.Entities;
using ToDoApp.Application.DTOs;
using ToDoApp.Application.IRepo;

namespace ToDoApp.Application.Services.Users;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<UserService> _logger;

    public UserService(
        UserManager<User> userManager,
        ILogger<UserService> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }
    //Task<RegisterUserDto>
    public async Task AddToRoleAsync(User user, string roleName)
    {
        await _userManager.AddToRoleAsync(user, roleName);
    }

    public async Task<IdentityResult> RegisterAsync(RegisterUserDto dto)
    {
        try
        {
            _logger.LogInformation("Starting user registration for username: {Username}", dto.Username);

            var user = new User
            {
                UserName = dto.Username,
                Email = dto.Email,
                FullName = dto.FullName
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            await _userManager.AddToRoleAsync(user, "User");


            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("User registration failed for {Username}. Errors: {Errors}", dto.Username, errors);
                throw new Exception(errors);
            }
            _logger.LogInformation("User registered successfully: {Username}", dto.Username);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while registering user: {Username}", dto.Username);
            throw;
            
        }
        
    }

    public async Task<List<User>> GetAllAsync()
    {
        try
        {
            _logger.LogInformation("Fetching all users");

            var users = await _userManager.Users.ToListAsync();

            _logger.LogInformation("Fetched {Count} users", users.Count);
            return users;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching all users");
            throw;
        }
    }

    public async Task<User?> GetByIdAsync(string id)
    {
        try
        {
            _logger.LogInformation("Fetching user by ID: {UserId}", id);

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                _logger.LogWarning("User not found with ID: {UserId}", id);

            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching user by ID: {UserId}", id);
            throw;
        }
    }

    public async Task DeleteAsync(string id)
    {
        try
        {
            _logger.LogInformation("Attempting to delete user with ID: {UserId}", id);

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                _logger.LogWarning("Delete failed. User not found with ID: {UserId}", id);
                return;
            }

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("Failed to delete user {UserId}. Errors: {Errors}", id, errors);
                throw new Exception(errors);
            }

            _logger.LogInformation("User deleted successfully with ID: {UserId}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while deleting user with ID: {UserId}", id);
            throw;
        }
    }

    public async Task<User?> GetByUsernameAndPasswordAsync(string username, string password)
    {
        try
        {
            _logger.LogInformation("Authenticating user: {Username}", username);

            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
            {
                _logger.LogWarning("Authentication failed. User not found: {Username}", username);
                return null;
            }

            var isValid = await _userManager.CheckPasswordAsync(user, password);
            if (!isValid)
            {
                _logger.LogWarning("Authentication failed. Invalid password for user: {Username}", username);
                return null;
            }

            _logger.LogInformation("User authenticated successfully: {Username}", username);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during authentication for user: {Username}", username);
            throw;
        }
    }



   



}

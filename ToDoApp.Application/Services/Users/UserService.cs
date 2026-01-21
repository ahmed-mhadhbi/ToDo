using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using TodoApp.Domain.Entities;
using ToDoApp.Application.DTOs;
using ToDoApp.Application.IRepo;
using ToDoApp.Application.Services.Auth;
using ToDoApp.Domain.entities;
using ToDoApp.Application.Common.Resources;

namespace ToDoApp.Application.Services.Users;

public class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly IRefreshTokenRepository _refreshTokenRepo;
    private readonly IJwtService _jwtService;
    private readonly ILogger<UserService> _logger;


    public UserService(
        UserManager<User> userManager,
        IRefreshTokenRepository refreshTokenRepo,
        IJwtService jwtService,
        ILogger<UserService> logger)
    {
        _userManager = userManager;
        _refreshTokenRepo = refreshTokenRepo;
        _jwtService = jwtService;
        _logger = logger;
    }

    // ---------- REGISTER ----------
    public async Task<IdentityResult> RegisterAsync(RegisterUserDto dto)
    {
        try
        {
            _logger.LogInformation("Starting user registration for username: {Username}", dto.Username);

            var user = new User
            {
                UserName = dto.Username,
                Email = dto.Email
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

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

    // ---------- LOGIN ----------
    public async Task<(string AccessToken, string RefreshToken)> LoginAsync(LoginUserDto dto)
    {
        try
        {
            // Validation
            if (dto is null)
                throw new ArgumentNullException(nameof(dto));

            if (string.IsNullOrWhiteSpace(dto.Username))
                throw new ArgumentException("Username is required", nameof(dto.Username));

            if (string.IsNullOrWhiteSpace(dto.Password))
                throw new ArgumentException("Password is required", nameof(dto.Password));

            var user = await _userManager.FindByNameAsync(dto.Username);

            if (user is null)
                throw new UnauthorizedAccessException("Invalid credentials");

            var passwordValid = await _userManager.CheckPasswordAsync(user, dto.Password);

            if (!passwordValid)
                throw new UnauthorizedAccessException("Invalid credentials");

            return await GenerateTokensAsync(user);
        }
        catch (ArgumentException)
        {
            // Validation errors → bubble up
            throw;
        }
        catch (UnauthorizedAccessException)
        {
            // Auth errors → bubble up (important for 401 responses)
            throw;
        }
        catch (Exception ex)
        {
            // Unexpected or infrastructure-related errors
            throw new ApplicationException(
                "An error occurred while logging in the user.",
                ex
            );
        }
    }


    // ---------- REFRESH TOKEN ----------
    public async Task<(string AccessToken, string RefreshToken)> RefreshTokenAsync(string refreshToken)
    {
        var storedToken = await _refreshTokenRepo.GetValidTokenAsync(refreshToken)
            ?? throw new UnauthorizedAccessException("Invalid refresh token");

        await _refreshTokenRepo.RevokeAsync(storedToken);

        return await GenerateTokensAsync(storedToken.User);
    }

    // ---------- USERS ----------
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

    // ---------- HELPERS ----------
    private async Task<(string, string)> GenerateTokensAsync(User user)
    {
        var accessToken = await _jwtService.GenerateAccessTokenAsync(user);
        var refreshToken = _jwtService.GenerateRefreshToken();

        await _refreshTokenRepo.SaveAsync(new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        });

        return (accessToken, refreshToken);
    }


    public async Task ChangePasswordAsync(string userId,string currentPassword,string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            throw new UnauthorizedAccessException("User not found");

        var result = await _userManager.ChangePasswordAsync(
            user,
            currentPassword,
            newPassword
        );

        if (!result.Succeeded)
        {
            var errors = string.Join(
                ", ",
                result.Errors.Select(e => e.Description)
            );
            throw new Exception(errors);
        }
    }


}

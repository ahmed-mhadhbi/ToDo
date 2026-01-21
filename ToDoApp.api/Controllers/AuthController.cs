using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TodoApp.Domain.Entities;
using ToDoApp.Application.Common;
using ToDoApp.Application.DTOs;
using ToDoApp.Application.DTOs.Auth;
using ToDoApp.Application.Services.Email;
using ToDoApp.Application.Services.Users;

namespace ToDoApp.Api.Controllers;

[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IEmailService _emailService;
    private readonly UserManager<User> _userManager;

    public AuthController(IUserService userService, IEmailService emailService , UserManager<User> userManager)
    {
        _userService = userService;
        _emailService = emailService;
        _userManager = userManager;

    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserDto dto)
    {
        await _userService.RegisterAsync(dto);
        return Ok(ApiResponse<string>.Ok(null, "User registered successfully"));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUserDto dto)
    {
        var result = await _userService.LoginAsync(dto);

        return Ok(ApiResponse<object>.Ok(new
        {
            accessToken = result.AccessToken,
            refreshToken = result.RefreshToken
        }));
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(
    [FromBody] ForgotPasswordDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            return Ok(); // security: don’t reveal user existence

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);

        var encodedToken = WebUtility.UrlEncode(token);

        var resetLink =
            $"http://localhost:4200/reset-password?email={dto.Email}&token={encodedToken}";

        await _emailService.SendAsync(
            dto.Email,
            "Reset your password",
            $"Click here to reset your password:\n{resetLink}"
        );

        return Ok(new { message = "Reset link sent to email" });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(
    [FromBody] ResetPasswordDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            return BadRequest("Invalid request");

        var result = await _userManager.ResetPasswordAsync(
            user,
            dto.Token,
            dto.NewPassword
        );

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok(new { message = "Password reset successful" });
    }


    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenRequestDto dto)
    {
        var result = await _userService.RefreshTokenAsync(dto.RefreshToken);

        return Ok(ApiResponse<object>.Ok(new
        {
            accessToken = result.AccessToken,
            refreshToken = result.RefreshToken
        }));
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using TodoApp.Domain.Entities;
using ToDoApp.Application.Common;
using ToDoApp.Application.DTOs;
using ToDoApp.Application.DTOs.Auth;
using ToDoApp.Application.Services.Email;
using ToDoApp.Application.Services.OTP;
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
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;
    private readonly IHashOtp _hashOtp;

    public AuthController(IUserService userService, IHashOtp hashOtp, IEmailService emailService , UserManager<User> userManager, IConfiguration configuration , ILogger<AuthController> logger )
    {
        _userService = userService;
        _emailService = emailService;
        _userManager = userManager;
        _configuration = configuration;
        _logger = logger;
        _hashOtp = hashOtp;

    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserDto dto)
    {
        await _userService.RegisterAsync(dto);
        return Ok(ApiResponse<string>.Ok(null, "User registered successfully check your Email"));
    }

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyOtpDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Otp))
            return BadRequest("Invalid data");

        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
            return BadRequest("Invalid email");

        // 🔒 NULL SAFETY (THIS PREVENTS 500)
        if (user.EmailOtpHash == null || user.EmailOtpExpiresAt == null)
            return BadRequest("OTP not found");

        if (user.EmailOtpExpiresAt < DateTime.UtcNow)
            return BadRequest("OTP expired");

        var hashedOtp = _hashOtp.Hashotp(dto.Otp);

        if (hashedOtp != user.EmailOtpHash)
            return BadRequest("Invalid OTP");

        user.EmailConfirmed = true;
        user.EmailOtpHash = null;
        user.EmailOtpExpiresAt = default;

        await _userManager.UpdateAsync(user);

        return Ok(new { message = "Email verified successfully" });
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

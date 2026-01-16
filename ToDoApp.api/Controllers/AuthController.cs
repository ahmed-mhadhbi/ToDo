using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TodoApp.Domain.Entities;
using ToDoApp.Application.DTOs;
using ToDoApp.Application.Services.Users;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;

    public AuthController(
        IUserService userService,
        IConfiguration configuration)
    {
        _userService = userService;
        _configuration = configuration;
    }

    // ---------------- Register ----------------
    [HttpPost("register")]
    
    public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await _userService.RegisterAsync(dto);

        return Ok(new { message = "User registered successfully" });
    }

    // ---------------- Login ----------------
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userService
            .GetByUsernameAndPasswordAsync(dto.Username, dto.Password);

        if (user == null)
            return Unauthorized(new { message = "Invalid username or password" });

        var token = GenerateJwtToken(user);

        return Ok(new
        {
            accessToken = token,
            tokenType = "Bearer",
            expiresIn = 3600
        });
    }

    // ---------------- JWT Helper ----------------
    private string GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim("FullName", user.FullName ?? string.Empty)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)
        );

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(60),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

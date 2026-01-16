using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TodoApp.Domain.Entities;
using ToDoApp.Application.Common;
using ToDoApp.Application.DTOs;
using ToDoApp.Application.Services.Users;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;
    private readonly UserManager<User> _userManager;

    public AuthController(
        IUserService userService,
        IConfiguration configuration,
    UserManager<User> userManager)
    {
        _userService = userService;
        _configuration = configuration;
        _userManager = userManager;
    }

    // ---------------- Register ----------------
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<string>.Fail("Invalid input"));

        var user = await _userService.RegisterAsync(dto); // returns User
        if (user == null)
            return BadRequest(ApiResponse<string>.Fail("User could not be created"));

       

        return Ok(ApiResponse<string>.Ok(null, "User registered successfully"));
    }


    // ---------------- Login ----------------
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<string>.Fail("Invalid input"));

        var user = await _userService.GetByUsernameAndPasswordAsync(dto.Username, dto.Password);

        if (user == null)
            return Unauthorized(ApiResponse<string>.Fail("Invalid username or password"));

        var token = await GenerateJwtToken(user);

        return Ok(ApiResponse<object>.Ok(new
        {
            accessToken = token,
            tokenType = "Bearer",
            expiresIn = 3600
        }));
    }

    // ---------------- JWT Helper ----------------
    private async Task<string> GenerateJwtToken(User user)
    {
        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
        new Claim("FullName", user.FullName ?? string.Empty)
    };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

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
            expires: DateTime.UtcNow.AddMinutes(60),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }







}

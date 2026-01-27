using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Security.Claims;
using System.Threading.Tasks;
using TodoApp.Domain.Entities;
using ToDoApp.Application.Services.Auth;
using Xunit;
namespace ToDoApp.UnitTest.Services;

public class JwtServiceTests
{
    private readonly Mock<IConfiguration> _configMock;
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly JwtService _jwtService;

    public JwtServiceTests()
    {
        // Mock IConfiguration
        _configMock = new Mock<IConfiguration>();
        _configMock.Setup(c => c["Jwt:Key"]).Returns("VeryStrongSecretKey123!");
        _configMock.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
        _configMock.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");
        _configMock.Setup(c => c["Jwt:DurationInMinutes"]).Returns("60");

        // Mock UserManager<User>
        var store = new Mock<IUserStore<User>>();
        _userManagerMock = new Mock<UserManager<User>>(
            store.Object, null, null, null, null, null, null, null, null
        );

        _jwtService = new JwtService(_configMock.Object, _userManagerMock.Object);
    }

    [Fact]
    public void GenerateRefreshToken_ShouldReturn64ByteBase64String()
    {
        // Act
        var token = _jwtService.GenerateRefreshToken();

        // Assert
        token.Should().NotBeNullOrEmpty();
        var bytes = Convert.FromBase64String(token);
        bytes.Length.Should().Be(64);
    }

    [Fact]
    public async Task GenerateAccessTokenAsync_ShouldReturnJwtToken()
    {
        // Arrange
        var user = new User { Id = "1", UserName = "Ahmed", FullName = "Ahmed Mhadhbi" };
        _userManagerMock.Setup(u => u.GetRolesAsync(user))
                        .ReturnsAsync(new List<string> { "Admin" });

        // Act
        var token = await _jwtService.GenerateAccessTokenAsync(user);

        // Assert
        token.Should().NotBeNullOrEmpty();
        token.Should().Contain(".");
    }

    [Fact]
    public async Task ResetPasswordAsync_ShouldReturnTrue_WhenSuccess()
    {
        var user = new User { Id = "1", UserName = "Ahmed", Email = "a@test.com" };
        _userManagerMock.Setup(u => u.FindByEmailAsync(user.Email))
                        .ReturnsAsync(user);
        _userManagerMock.Setup(u => u.ResetPasswordAsync(user, "token", "newPass123"))
                        .ReturnsAsync(IdentityResult.Success);

        var result = await _jwtService.ResetPasswordAsync(user.Email, "token", "newPass123");

        result.Should().BeTrue();
    }

    [Fact]
    public async Task ResetPasswordAsync_ShouldReturnFalse_WhenUserNotFound()
    {
        _userManagerMock.Setup(u => u.FindByEmailAsync("missing@test.com"))
                        .ReturnsAsync((User)null);

        var result = await _jwtService.ResetPasswordAsync("missing@test.com", "token", "newPass123");

        result.Should().BeFalse();
    }
}


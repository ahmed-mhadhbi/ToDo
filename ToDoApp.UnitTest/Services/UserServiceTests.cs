using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoApp.Domain.Entities;
using ToDoApp.Application.DTOs;
using ToDoApp.Application.IRepo;
using ToDoApp.Application.Services.Auth;
using ToDoApp.Application.Services.Email;
using ToDoApp.Application.Services.OTP;
using ToDoApp.Application.Services.Users;
using ToDoApp.Domain.entities;
using Xunit;

namespace ToDoApp.UnitTest.Services
{
    public class UserServiceTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<IRefreshTokenRepository> _refreshTokenRepoMock;
        private readonly Mock<IJwtService> _jwtServiceMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<IHashOtp> _hashOtpMock;
        private readonly Mock<ILogger<UserService>> _loggerMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            var storeMock = new Mock<IUserStore<User>>();
            _userManagerMock = new Mock<UserManager<User>>(
                storeMock.Object, null, null, null, null, null, null, null, null
            );

            _refreshTokenRepoMock = new Mock<IRefreshTokenRepository>();
            _jwtServiceMock = new Mock<IJwtService>();
            _emailServiceMock = new Mock<IEmailService>();
            _hashOtpMock = new Mock<IHashOtp>();
            _loggerMock = new Mock<ILogger<UserService>>();

            _userService = new UserService(
                _userManagerMock.Object,
                _refreshTokenRepoMock.Object,
                _jwtServiceMock.Object,
                _emailServiceMock.Object,
                _hashOtpMock.Object,
                _loggerMock.Object
            );
        }

        #region RegisterAsync Tests

        [Fact]
        public async Task RegisterAsync_ShouldCreateUserAndSendOtpEmail()
        {
            // Arrange
            var dto = new RegisterUserDto
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "Password123!"
            };

            var user = new User { UserName = dto.Username, Email = dto.Email };

            _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<User>(), dto.Password))
                            .ReturnsAsync(IdentityResult.Success);

            _hashOtpMock.Setup(h => h.GenerateOtp()).Returns("123456");
            _hashOtpMock.Setup(h => h.Hashotp("123456")).Returns("hashed-otp");

            _userManagerMock.Setup(u => u.UpdateAsync(It.IsAny<User>()))
                            .ReturnsAsync(IdentityResult.Success);

            _emailServiceMock.Setup(e => e.SendAsync(dto.Email, It.IsAny<string>(), It.IsAny<string>()))
                             .Returns(Task.CompletedTask)
                             .Verifiable();

            // Act
            var result = await _userService.RegisterAsync(dto);

            // Assert
            result.Succeeded.Should().BeTrue();
            _userManagerMock.Verify(u => u.CreateAsync(It.Is<User>(
                x => x.UserName == dto.Username && x.Email == dto.Email
            ), dto.Password), Times.Once);

            _emailServiceMock.Verify();
        }

        [Fact]
        public async Task RegisterAsync_ShouldThrow_WhenUserManagerFails()
        {
            var dto = new RegisterUserDto
            {
                Username = "failuser",
                Email = "fail@example.com",
                Password = "Password123!"
            };

            var identityErrors = new List<IdentityError>
        {
            new IdentityError { Description = "Duplicate username" }
        };

            _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<User>(), dto.Password))
                            .ReturnsAsync(IdentityResult.Failed(identityErrors.ToArray()));

            Func<Task> act = async () => await _userService.RegisterAsync(dto);

            await act.Should().ThrowAsync<Exception>()
                .WithMessage("*Duplicate username*");
        }

        #endregion

        #region LoginAsync Tests

        [Fact]
        public async Task LoginAsync_ShouldReturnTokens_WhenCredentialsValid()
        {
            var dto = new LoginUserDto { Username = "user1", Password = "pass" };
            var user = new User { Id = "1", UserName = "user1" };

            _userManagerMock.Setup(u => u.FindByNameAsync(dto.Username))
                            .ReturnsAsync(user);
            _userManagerMock.Setup(u => u.CheckPasswordAsync(user, dto.Password))
                            .ReturnsAsync(true);

            _jwtServiceMock.Setup(j => j.GenerateAccessTokenAsync(user))
                           .ReturnsAsync("access-token");
            _jwtServiceMock.Setup(j => j.GenerateRefreshToken())
                           .Returns("refresh-token");

            _refreshTokenRepoMock.Setup(r => r.SaveAsync(It.IsAny<RefreshToken>()))
                                 .Returns(Task.CompletedTask);

            var tokens = await _userService.LoginAsync(dto);

            tokens.AccessToken.Should().Be("access-token");
            tokens.RefreshToken.Should().Be("refresh-token");
        }

        [Fact]
        public async Task LoginAsync_ShouldThrow_WhenUserNotFound()
        {
            var dto = new LoginUserDto { Username = "user1", Password = "pass" };
            _userManagerMock.Setup(u => u.FindByNameAsync(dto.Username))
                            .ReturnsAsync((User)null);

            Func<Task> act = async () => await _userService.LoginAsync(dto);
            await act.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        public async Task LoginAsync_ShouldThrow_WhenPasswordInvalid()
        {
            var dto = new LoginUserDto { Username = "user1", Password = "pass" };
            var user = new User { Id = "1", UserName = "user1" };

            _userManagerMock.Setup(u => u.FindByNameAsync(dto.Username))
                            .ReturnsAsync(user);
            _userManagerMock.Setup(u => u.CheckPasswordAsync(user, dto.Password))
                            .ReturnsAsync(false);

            Func<Task> act = async () => await _userService.LoginAsync(dto);
            await act.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        #endregion
    }

}

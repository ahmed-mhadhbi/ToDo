using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using ToDoApp.Application.DTOs;
using ToDoApp.Application.IRepo;
using ToDoApp.Application.Services.Users;
using TodoApp.Domain.Entities;

using Xunit;

namespace TodoApp.Application.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly IUserService _userService;

        public UserServiceTests()
        {
            // Mock UserManager requires IUserStore<User>
            var store = new Mock<IUserStore<User>>();
            _userManagerMock = new Mock<UserManager<User>>(
                store.Object, null, null, null, null, null, null, null, null
            );

            //  nraja3haforthetest _userService = new UserService(_userManagerMock.Object);
        }

        // -------------------------
        // RegisterAsync Test
        // -------------------------
        [Fact]
        public async Task RegisterAsync_ShouldCallUserManagerCreate()
        {
            // Arrange
            var dto = new RegisterUserDto
            {
                Username = "ahmed",
                Email = "ahmed@mail.com",
                Password = "Password123!"
            };

            _userManagerMock.Setup(x => x.CreateAsync(
                It.IsAny<User>(), dto.Password
            )).ReturnsAsync(IdentityResult.Success);

            // Act
            await _userService.RegisterAsync(dto);

            // Assert
            _userManagerMock.Verify(x =>
                x.CreateAsync(
                    It.Is<User>(u => u.UserName == "ahmed" && u.Email == "ahmed@mail.com"),
                    dto.Password
                ),
                Times.Once
            );
        }

        // -------------------------
        // GetAllAsync Test
        // -------------------------
        [Fact]
        public void GetAllAsync_ShouldReturnAllUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new User { UserName = "Ahmed", Email = "ahmed@mail.com" },
                new User { UserName = "Ali", Email = "ali@mail.com" }
            }.AsQueryable();

            _userManagerMock.Setup(x => x.Users).Returns(users);

            // Act
            var result = _userService.GetAllAsync().Result;

            // Assert
            result.Should().HaveCount(2);
            result[0].UserName.Should().Be("Ahmed");
        }

        // -------------------------
        // GetByIdAsync Test
        // -------------------------
        [Fact]
        public async Task GetByIdAsync_ShouldReturnUser()
        {
            // Arrange
            var user = new User { UserName = "Ahmed", Email = "ahmed@mail.com" };
            _userManagerMock.Setup(x => x.FindByIdAsync("1")).ReturnsAsync(user);

            // Act
            var result = await _userService.GetByIdAsync("1");

            // Assert
            result.Should().NotBeNull();
            result.UserName.Should().Be("Ahmed");
        }

        // -------------------------
        // DeleteAsync Test
        // -------------------------
        [Fact]
        public async Task DeleteAsync_ShouldCallUserManagerDelete()
        {
            // Arrange
            var user = new User { UserName = "Ahmed", Email = "ahmed@mail.com" };
            _userManagerMock.Setup(x => x.FindByIdAsync("1")).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.DeleteAsync(user)).ReturnsAsync(IdentityResult.Success);

            // Act
            await _userService.DeleteAsync("1");

            // Assert
            _userManagerMock.Verify(x => x.DeleteAsync(user), Times.Once);
        }
    }
}

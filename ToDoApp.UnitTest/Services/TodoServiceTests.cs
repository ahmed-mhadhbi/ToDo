using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
using TodoApp.Domain.Entities;
using ToDoApp.Application.IRepo;
using ToDoApp.Application.Services.Email;
using ToDoApp.Application.Services.ToDos;
using Xunit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ToDoApp.UnitTest.Services
{
   

    public class TodoServiceTests
    {
        private readonly Mock<ITodoRepository> _repoMock;
        private readonly TodoService _service;

        public TodoServiceTests()
        {
            _repoMock = new Mock<ITodoRepository>();
            _service = new TodoService(_repoMock.Object);
        }

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_ShouldCallAddAsync_WhenValidInput()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var title = "Test Todo";

            // Act
            await _service.CreateAsync(userId, title);

            // Assert
            _repoMock.Verify(r => r.AddAsync(It.Is<Todo>(
                t => t.UserId == userId && t.Title == title
            )), Times.Once); 
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task CreateAsync_ShouldThrowArgumentException_WhenUserIdInvalid(string userId)
        {
            var ex = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateAsync(userId, "Title")
            );
            ex.ParamName.Should().Be("userId");
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowArgumentException_WhenUserIdNotGuid()
        {
            var ex = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateAsync("not-a-guid", "Title")
            );
            ex.ParamName.Should().Be("userId");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task CreateAsync_ShouldThrowArgumentException_WhenTitleInvalid(string title)
        {
            var userId = Guid.NewGuid().ToString();

            var ex = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.CreateAsync(userId, title)
            );
            ex.ParamName.Should().Be("title");
        }

        [Fact]
        public async Task CreateAsync_ShouldThrowApplicationException_OnRepositoryError()
        {
            var userId = Guid.NewGuid().ToString();
            _repoMock.Setup(r => r.AddAsync(It.IsAny<Todo>()))
                     .ThrowsAsync(new Exception("DB error"));

            var ex = await Assert.ThrowsAsync<ApplicationException>(
                () => _service.CreateAsync(userId, "Test")
            );

            ex.Message.Should().Be("An error occurred while creating the todo item.");
            ex.InnerException.Should().NotBeNull();
            ex.InnerException!.Message.Should().Be("DB error");
        }

        #endregion

        #region GetByUserIdAsync Tests

        [Fact]
        public async Task GetByUserIdAsync_ShouldReturnTodos_WhenValidInput()
        {
            var userGuid = Guid.NewGuid();
            var todos = new List<Todo>
        {
            new Todo { UserId = userGuid.ToString(), Title = "Todo1" },
            new Todo { UserId = userGuid.ToString(), Title = "Todo2" }
        };

            _repoMock.Setup(r => r.GetByUserIdAsync(userGuid))
                     .ReturnsAsync(todos);

            var result = await _service.GetByUserIdAsync(userGuid.ToString());

            result.Should().BeEquivalentTo(todos);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task GetByUserIdAsync_ShouldThrowArgumentException_WhenUserIdInvalid(string userId)
        {
            var ex = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.GetByUserIdAsync(userId)
            );
            ex.ParamName.Should().Be("userId");
        }

        [Fact]
        public async Task GetByUserIdAsync_ShouldThrowArgumentException_WhenUserIdNotGuid()
        {
            var ex = await Assert.ThrowsAsync<ArgumentException>(
                () => _service.GetByUserIdAsync("not-a-guid")
            );
            ex.ParamName.Should().Be("userId");
        }

        [Fact]
        public async Task GetByUserIdAsync_ShouldThrowApplicationException_OnRepositoryError()
        {
            var userId = Guid.NewGuid().ToString();
            _repoMock.Setup(r => r.GetByUserIdAsync(It.IsAny<Guid>()))
                     .ThrowsAsync(new Exception("DB error"));

            var ex = await Assert.ThrowsAsync<ApplicationException>(
                () => _service.GetByUserIdAsync(userId)
            );

            ex.Message.Should().Be("An error occurred while retrieving todo items.");
            ex.InnerException!.Message.Should().Be("DB error");
        }

        #endregion
    }
}

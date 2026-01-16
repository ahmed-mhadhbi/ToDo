using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Entities;

namespace ToDoApp.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<User> // <User> is your custom User entity
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // Your custom tables
        public DbSet<Todo> Todos => Set<Todo>();
    }
}

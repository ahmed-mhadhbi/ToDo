using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Entities;
using ToDoApp.Domain.entities;

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
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    

    protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<RefreshToken>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId);
        }

    } 
}

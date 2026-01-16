using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Entities;



namespace ToDoApp.Infrastructure.Data
{
    public class AppDbContext : DbContext
    //IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {

        }
        public DbSet<Todo> Todos => Set<Todo>();
        public DbSet<User> UserLogins => Set<User>();
        /*

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //builder.Entity<Todo>()
            //    .HasOne(t => t.User)
            //.WithMany(u => u.Todos)
            //.HasForeignKey(t => t.UserId)
            //.OnDelete(DeleteBehavior.Cascade);
        }
        */
        //public DbSet<Todo> Todos => Set<Todo>();


    }
}

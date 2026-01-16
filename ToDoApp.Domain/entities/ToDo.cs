using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoApp.Domain.Entities;


[Table("Todos")]
public class Todo
{
    [Key]
    [Column("IdTodo")]
    public Guid Id { get; set; }

    [Required]               // Make sure title is not null
    [MaxLength(50)]
    public string Title { get; set; } = string.Empty;

    // Foreign key to User
    [Required]
    [Column("UserId")]
    public string UserId { get; set; } = null!;   // IdentityUser key is string by default

    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
}
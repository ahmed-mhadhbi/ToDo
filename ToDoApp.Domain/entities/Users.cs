using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;



namespace TodoApp.Domain.Entities;

[Table("Users")]
public class User : IdentityUser
{
    [MaxLength(100)]
    public string? FullName { get; set; }

    // Navigation property for Todos
    public ICollection<Todo> Todos { get; set; } = new List<Todo>();
}

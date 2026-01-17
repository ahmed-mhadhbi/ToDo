using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoApp.Domain.entities;
  //fel model
    public class TokenJwt
{
    public string? Value { get; set; }
    public DateTime? ExpireDate { get; set; }
    public int ExpiryDuration { get; set; } = 60;
}

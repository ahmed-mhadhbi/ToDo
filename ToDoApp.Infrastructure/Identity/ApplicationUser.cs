using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoApp.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? EmailOtpHash { get; set; }
        public DateTime? EmailOtpExpiresAt { get; set; }
    }
}

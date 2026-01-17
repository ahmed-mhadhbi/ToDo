using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoApp.Application.DTOs.Auth;
public class RefreshTokenRequestDto
{
    public string RefreshToken { get; set; } = null!;
}


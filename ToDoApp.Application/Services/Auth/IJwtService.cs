using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoApp.Application.Services.Auth;
using TodoApp.Domain.Entities;



public interface IJwtService
{
    Task<string> GenerateAccessTokenAsync(User user);
    string GenerateRefreshToken();
    Task<bool> ResetPasswordAsync(string email, string token, string newPassword);

    Task<string> GenerateAccessTokenMailAsync(User user);
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoApp.Application.IRepo;

using ToDoApp.Domain.entities;

public interface IRefreshTokenRepository
{
    Task SaveAsync(RefreshToken token);
    Task<RefreshToken?> GetValidTokenAsync(string token);
    Task RevokeAsync(RefreshToken token);
}

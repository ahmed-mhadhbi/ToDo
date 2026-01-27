using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoApp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using TodoApp.Domain.Entities;
using ToDoApp.Application.IRepo;

using ToDoApp.Domain.entities;
using ToDoApp.Infrastructure.Data;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AppDbContext _context;

    public RefreshTokenRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task SaveAsync(RefreshToken token)
    {
        _context.RefreshTokens.Add(token);
        await _context.SaveChangesAsync();
    }

    public async Task<RefreshToken?> GetValidTokenAsync(string token)
    {
        return await _context.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x =>
                x.Token == token &&
                !x.IsRevoked &&
                x.ExpiresAt > DateTime.UtcNow);
    }

    public async Task RevokeAsync(RefreshToken token)
    {
        token.IsRevoked = true;
        await _context.SaveChangesAsync();
    }
}

using Ewallet.Api.Data;
using Ewallet.Api.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Ewallet.Api.Repositories;

public sealed class EfUserRepository : IUserRepository
{
    private readonly AppDbContext _db;
    public EfUserRepository(AppDbContext db)
    {
        _db = db;
    }
    public async Task<bool> AddUserWithWalletAsync(Users users, Wallets wallets, CancellationToken cancellationToken)
    {
        _db.Users.Add(users);
        _db.Wallets.Add(wallets);
        try
        {
            await _db.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateException e) when (e.InnerException is SqlException { Number: 2601 or 2627 })
        {
            _db.Entry(users).State = EntityState.Detached;
            _db.Entry(wallets).State = EntityState.Detached;
            return false;
        }
    }

    public async Task<IReadOnlyList<Users>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _db.Users.AsNoTracking().OrderBy(u => u.FullName).ToArrayAsync(cancellationToken);
    }

    public async Task<Users?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, cancellationToken: cancellationToken);
    }

    public async Task<bool> UpdateUserAsync(Users users, CancellationToken cancellationToken)
    {
        var cek = await _db.Users.FirstOrDefaultAsync(u => u.Id == users.Id, cancellationToken);

        if (cek is null)
        {
            return false;
        }

        cek.UpdateDetail(
            users.UserName,
            users.FullName,
            users.Email,
            users.Phone
        );

        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
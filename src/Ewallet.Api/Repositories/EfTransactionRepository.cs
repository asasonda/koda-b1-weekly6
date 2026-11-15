using Ewallet.Api.Data;
using Ewallet.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Ewallet.Api.Repositories;

public sealed class EfTransactionRepository : ITransactionRepository
{
    private readonly AppDbContext _db;

    public EfTransactionRepository(AppDbContext db)
    {
        _db = db;
    }
    public async Task<string?> GetReferenceNo(CancellationToken cancellationToken)
    {
        return await _db.Transactions.OrderByDescending(t => t.ReferenceNo).Select(t => t.ReferenceNo).FirstOrDefaultAsync(cancellationToken);
    }
}
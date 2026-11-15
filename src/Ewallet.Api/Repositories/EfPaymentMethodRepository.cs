using Ewallet.Api.Data;
using Ewallet.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Ewallet.Api.Repositories;

public sealed class EfPaymentMethodRepository : IPaymentMethod
{
    private readonly AppDbContext _db;
    public EfPaymentMethodRepository(AppDbContext db)
    {
        _db = db;
    }
    public async Task<PaymentMethods?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await _db.PaymentMethods.AsNoTracking().FirstOrDefaultAsync(pm => pm.Id == id, cancellationToken);
    }
}
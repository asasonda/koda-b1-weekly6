using Ewallet.Api.Data;
using Ewallet.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Ewallet.Api.Repositories;

public sealed class EfWalletRepository : IWalletRepository
{
    private readonly AppDbContext _db;

    public EfWalletRepository(AppDbContext db)
    {
        _db = db;
    }
    public async Task<bool> AddBalanceAsync(long id, decimal amount, CancellationToken cancellationToken)
    {
        var wallet = await _db.Wallets.AsNoTracking().FirstOrDefaultAsync(w => w.Id == id, cancellationToken);

        if(wallet is null)
        {
            return false;
        }

        wallet.AddBalance(amount);

        return true;
    }

    public async Task<bool> ConfirmAsync(long id, CancellationToken cancellationToken)
    {
        var wallet = await _db.Wallets.AsNoTracking().FirstOrDefaultAsync(w => w.Id == id ,cancellationToken);
        if(wallet is null)
        {
            return false;
        }

        var topUp = await _db.TopUpRequests.AsNoTracking().FirstOrDefaultAsync(tp => tp.Id == id,cancellationToken);
        if (topUp is null)
        {
            return false;
        }

        wallet.ConfirmationSuccess();
        wallet.AddBalance(topUp.Amount);
        return true;
    }

    public async Task<bool> CreateTopUp(Transactions transactions, TopUpRequests topUpRequests, CancellationToken cancellationToken)
    {
        _db.Transactions.Add(transactions);
        _db.TopUpRequests.Add(topUpRequests);
        try
        {
            await _db.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (System.Exception)
        {
            _db.Entry(transactions).State = EntityState.Detached;
            _db.Entry(topUpRequests).State = EntityState.Detached;
            return false;
        }
    }
    

    public async Task<Wallets?> GetByIdWallet(long id, CancellationToken cancellationToken)
    {
        return await _db.Wallets.AsNoTracking().FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
    }

    public async Task<TopUpRequests?> GetByWalletIdTopUp(long id, CancellationToken cancellationToken)
    {
        return await _db.TopUpRequests.AsNoTracking().Include(tp => tp.PaymentMethods).FirstOrDefaultAsync(tp => tp.WalletId == id, cancellationToken);
    }

    public async Task<bool> ReduceBalanceAsync(long id, decimal amount, CancellationToken cancellationToken)
    {
        var wallet = await _db.Wallets.AsNoTracking().FirstOrDefaultAsync(w => w.Id == id, cancellationToken);

        if(wallet is null)
        {
            return false;
        }
        wallet.ReduceBalance(amount);
        return true;
    }

}
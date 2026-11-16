using Ewallet.Api.Data;
using Ewallet.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Ewallet.Api.Repositories;

public sealed class EfTransferRepository : ITransferRepository
{
    private readonly AppDbContext _db;

    public EfTransferRepository(AppDbContext db)
    {
        _db = db;
    }
    public async Task<bool> TransferAsync(Transactions transactions, CancellationToken cancellationToken)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var sender = await _db.Wallets.FirstOrDefaultAsync(w => w.Id == transactions.SenderWalletId, cancellationToken);
            var receiver = await _db.Wallets.FirstOrDefaultAsync(w => w.Id == transactions.ReceiverWalletId, cancellationToken);

            if (sender is null || receiver is null)
            {
                return false;
            }

            if (sender.Status != "ACTIVE" || receiver.Status != "ACTIVE")
            {
                return false;
            }

            if (sender.Balance < transactions.Amount)
            {
                return false;
            }

            var senderBefore = sender.Balance;
            var receiverBefore = receiver.Balance;

            sender.ReduceBalance(transactions.Amount);
            receiver.AddBalance(transactions.Amount);

            var senderMutation = new WalletMutations(
                transactions,
                sender.Id,
                "OUT",
                transactions.Amount,
                senderBefore,
                sender.Balance
            );

            var receiverMutation = new WalletMutations(
                transactions,
                receiver.Id,
                "IN",
                transactions.Amount,
                receiverBefore,
                receiver.Balance
            );

            _db.Transactions.Add(transactions);

            _db.WalletMutations.AddRange(
                senderMutation,
                receiverMutation
            );

            await _db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
            return true;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
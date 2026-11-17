using Ewallet.Api.Models;

namespace Ewallet.Api.Repositories;

public interface ITransferRepository
{
    Task<bool> TransferAsync(Transactions transactions, WalletMutations senderMutation, WalletMutations receiverMutation, CancellationToken cancellationToken); 
}
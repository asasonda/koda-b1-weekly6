using Ewallet.Api.Models;

namespace Ewallet.Api.Repositories;

public interface ITransferRepository
{
    Task<bool> TransferAsync(Transactions transactions, CancellationToken cancellationToken); 
}
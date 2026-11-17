using Ewallet.Api.DTOs;

namespace Ewallet.Api.Services;

public interface ITransferService
{
    Task<TransactionsTransferResponse?> TransferAsync(TransferRequest request, CancellationToken cancellationToken);
}
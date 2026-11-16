using Ewallet.Api.DTOs;
using Ewallet.Api.Models;

namespace Ewallet.Api.Repositories;

public interface ITopUpRequest
{
    Task<TopUpRequests?> GetByWalledIdTopUp(long walletId, CancellationToken cancellationToken);
    Task<TopUpRequests?> GetByIdTopUp(long walletId, CancellationToken cancellationToken);
    Task<bool> ConfirmAsync(long id, CancellationToken cancellationToken);
}
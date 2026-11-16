using Ewallet.Api.DTOs;

namespace Ewallet.Api.Services;

public interface ITopUpRequestServices
{
    Task<TopUpResponse?> GetByWalletIdTopUpAsync(long walletId, CancellationToken cancellationToken); 
    Task<TopUpResponse?> ConfirmTopUpAsync(long id, CancellationToken cancellationToken);
}
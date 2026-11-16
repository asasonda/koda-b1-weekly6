using Ewallet.Api.DTOs;

namespace Ewallet.Api.Services;

public interface IPaymentMethodServices
{
    Task<IReadOnlyList<PaymentMethodDetailsResponse>> GetAllPaymentMethodAsync(CancellationToken cancellationToken);
}
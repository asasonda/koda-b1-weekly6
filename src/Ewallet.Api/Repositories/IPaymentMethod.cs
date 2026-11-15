using Ewallet.Api.Models;

namespace Ewallet.Api.Repositories;

public interface IPaymentMethod
{
    Task<PaymentMethods?> GetByIdAsync(int id, CancellationToken cancellationToken);
}
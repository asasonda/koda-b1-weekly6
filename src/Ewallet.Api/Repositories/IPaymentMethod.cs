using Ewallet.Api.DTOs;
using Ewallet.Api.Models;

namespace Ewallet.Api.Repositories;

public interface IPaymentMethod
{
    Task<PaymentMethods?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<PaymentMethods>> GetAllAsync(CancellationToken cancellationToken);
}
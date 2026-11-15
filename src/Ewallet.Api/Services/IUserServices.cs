using Ewallet.Api.DTOs;
using Ewallet.Api.Models;

namespace Ewallet.Api.Services;

public interface IUserServices
{
    Task<IReadOnlyList<UserResponse>> GetAllUserAsync(CancellationToken cancellationToken);
    Task<UserResponse?> GetByUserIdAsync(long id, CancellationToken cancellationToken);
    Task<UserResponse?> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken);
    Task<UserResponse?> UpdateUserAsync(long id, UpdatedUserRequest request, CancellationToken cancellationToken);

    // Task<IReadOnlyList<TransactionResponse>> GetTransactionsByUsersIdAsync(int usersId, CancellationToken cancellationToken);
}
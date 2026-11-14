using Ewallet.Api.DTOs;
using Ewallet.Api.Models;

namespace Ewallet.Api.Services;

public interface IUserServices
{
    Task<IReadOnlyList<UserResponse>> GetAllUserAsync(CancellationToken cancellationToken);
    Task<UserResponse?> GetByUserIdAsync(int id, CancellationToken cancellationToken);
    Task<UserResponse?> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken);
    Task<UserResponse?> UpdateUserAsync(int id, UpdateUserRequest request, CancellationToken cancellationToken);

    // Task<IReadOnlyList<TransactionResponse>> GetTransactionsByUsersIdAsync(int usersId, CancellationToken cancellationToken);
}
using System.ComponentModel.DataAnnotations;

namespace Ewallet.Api.DTOs;

public sealed record TopUpRequest(
    int PaymentMethodId,
    decimal Amount
);

public sealed record TopUpResponse
(
    long Id, long TransactionId, long WalletId, int PaymentMethodId, decimal Amount, string Status, DateTime CreatedAtUtc
);

public sealed record TransactionsTopUp(
    long Id, 
    string ReferenceNo, 
    long? SenderWalletId, 
    long ReceiverWalletId, 
    string Type, 
    decimal Amount, 
    string Status, 
    TopUpResponse TopUp
);

public sealed record PaymentMethodResponse(
    string Code, bool IsActive, TopUpRequest TopUp
);
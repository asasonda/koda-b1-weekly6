namespace Ewallet.Api.Models;

public sealed class TopUpRequests
{
    TopUpRequests() {}
    public long Id {get; private set;}

    public long TransactionId {get; private set;}
    public Transactions Transactions {get; private set;} = null!;


    public long WalletId {get; private set;}
    public Wallets Wallets {get; private set;} = null!;


    public int PaymentMethodId {get; private set;}
    public PaymentMethods PaymentMethods {get; private set;} = null!;

    public decimal Amount {get; private set;}

    public string Status {get; private set;}
    public string? ProviderReference {get; private set;}
    public DateTime CreatedAtUtc {get; private set;}
    public DateTime? ExpiresAtUtc {get; private set;}
    public DateTime? PaidAtUtc {get; private set;}

    public TopUpRequests(long transactionId, long walletId, int paymentMethodId, decimal amount, string status, string? providerReference, DateTime? expiresAtUtc, DateTime? paidAtUtc)
    {
        if(transactionId < 0)
        {
            throw new ArgumentException("id transaction tidak boleh negative", nameof(transactionId));
        }
        if(walletId < 0)
        {
            throw new ArgumentException("Id wallet tidak boleh negative", nameof(walletId));
        }
        if(paymentMethodId < 0)
        {
            throw new ArgumentException("id PaymentMethod tidak boleh negative", nameof(paymentMethodId));
        }
        if(amount <= 0)
        {
            throw new ArgumentException("Amount tidak valid", nameof(amount));
        }
        if (string.IsNullOrWhiteSpace(status))
        {
            throw new ArgumentException("Status tidak valid", nameof(status));
        }

        TransactionId = transactionId;
        WalletId = walletId;
        PaymentMethodId = paymentMethodId;
        Amount = amount;
        Status = status;
        ProviderReference = providerReference;
        CreatedAtUtc = DateTime.UtcNow;
        ExpiresAtUtc =expiresAtUtc;
        PaidAtUtc = paidAtUtc;
    }
}
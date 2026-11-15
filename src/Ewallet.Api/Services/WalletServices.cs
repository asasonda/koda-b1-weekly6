using Ewallet.Api.DTOs;
using Ewallet.Api.Models;
using Ewallet.Api.Repositories;

namespace Ewallet.Api.Services;

public sealed class WalletServices : IWalletServices
{
    private readonly IWalletRepository _repository;
    private readonly IPaymentMethod _paymentMethod;
    private readonly ITransactionRepository _transactionRepository;

    public WalletServices(IWalletRepository repository, IPaymentMethod paymentMethod, ITransactionRepository transactionRepository)
    {
        _repository = repository;
        _paymentMethod = paymentMethod;
        _transactionRepository = transactionRepository;
    }

    public async Task<PaymentMethodResponse?> GetByIdTopUp(long id, CancellationToken cancellationToken)
    {
        if(id <= 0)
        {
            throw new ArgumentException("Id tidak valid");
        }
    }


    public async Task<WalletResponse?> GetByIdWalletAsync(long usersId, CancellationToken cancellationToken)
    {
        var wallets = await _repository.GetByIdWallet(usersId, cancellationToken);

        if(wallets is null)
        {
            return null;
        }
        return new WalletResponse(
            wallets.Id,
            wallets.WalletNumber,
            wallets.Balance,
            wallets.Status
        );
    }

    public async Task<TransactionsTopUp?> TopUpAsync(long id, TopUpRequest request, CancellationToken cancellationToken)
    {
        var wallet = await _repository.GetByIdWallet(id, cancellationToken);

        if (wallet is null)
        {
            return null;
        }

        if(request.Amount <= 0)
        {
            throw new ArgumentException("Nominal harus lebih dari 0");
        }

        var cekReference = await _transactionRepository.GetReferenceNo(cancellationToken);

        int number = 1;

        if(cekReference is not null)
        {
            number = int.Parse(cekReference[3..]) + 1;
        }

        var referenceNo = $"RF{number:D3}";

        var transaction = new Transactions(
            referenceNo,
            null,
            id,
            "TOPUP",
            request.Amount,
            "TopUp Saldo"
        );

        var paymentMethod = await _paymentMethod.GetByIdAsync(request.PaymentMethodId, cancellationToken);
        if(paymentMethod is null)
        {
            return null;
        }

        if (!paymentMethod.IsActive)
        {
            return null;
        }

        var topUp = new TopUpRequests(
            transaction,
            id,
            request.PaymentMethodId,
            request.Amount,
            null
        );

        var created = await _repository.CreateTopUp(transaction, topUp, cancellationToken);

        if (!created)
        {
            return null;
        }

        return new TransactionsTopUp(
            transaction.Id,
            transaction.ReferenceNo,
            transaction.SenderWalletId,
            transaction.ReceiverWalletId,
            transaction.Type,
            transaction.Amount,
            transaction.Status,
            new TopUpResponse(
                topUp.Id,
                topUp.TransactionId,
                topUp.WalletId,
                topUp.PaymentMethodId,
                topUp.Amount,
                topUp.Status,
                topUp.CreatedAtUtc
            )
        );

    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using bhb2core.Accounting.Interfaces;
using bhb2core.Accounting.Models;
using bhb2core.Utils.Logging;

namespace bhb2core.Accounting
{
  internal class TransactionEngine : ITransactionEngine
  {
    private readonly IAccountingDataAccess _accountingDataAccess;
    private readonly ILogger _logger;

    public TransactionEngine(
      in IAccountingDataAccess accountingDataAccess,
      in ILogger logger)
    {
      _accountingDataAccess = accountingDataAccess ?? throw new ArgumentNullException(nameof(accountingDataAccess));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task ProcessTransaction(Transaction transaction)
    {
      if (transaction == null)
      {
        throw new ArgumentNullException(nameof(transaction));
      }

      _logger.LogVerbose($"Transaction received: {transaction}.");

      IReadOnlyDictionary<string, Account> accounts = await _accountingDataAccess.GetAccountsById(
        new[]
        {
          transaction.DebitAccountId,
          transaction.CreditAccountId
        });

      decimal newDebitAccountBalance = accounts[transaction.DebitAccountId].Balance - transaction.Amount;
      decimal newCreditAccountBalance = accounts[transaction.CreditAccountId].Balance + transaction.Amount;

      await _accountingDataAccess.UpdateAccountBalances(
        new Dictionary<string, decimal>
        {
          { transaction.DebitAccountId, newDebitAccountBalance },
          { transaction.CreditAccountId, newCreditAccountBalance }
        });
    }
  }
}

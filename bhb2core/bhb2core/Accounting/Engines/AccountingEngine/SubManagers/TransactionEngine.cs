using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using bhb2core.Accounting.Engines.AccountingEngine.Interfaces;
using bhb2core.Accounting.Interfaces;
using bhb2core.Accounting.Models;
using bhb2core.Utils.Logging;

namespace bhb2core.Accounting.Engines.AccountingEngine.SubManagers
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

    // TODO: Split method up - move orchestration to manager.
    public async Task ProcessTransaction(Transaction transaction)
    {
      if (transaction == null)
      {
        throw new ArgumentNullException(nameof(transaction));
      }

      _logger.LogVerbose($"Transaction received: {transaction}.");

      IReadOnlyDictionary<string, Account> accounts = await _accountingDataAccess.GetAccounts(
        new[]
        {
          transaction.DebitAccountQualifiedName,
          transaction.CreditAccountQualifiedName
        });

      decimal newDebitAccountBalance = accounts[transaction.DebitAccountQualifiedName].Balance - transaction.Amount;
      decimal newCreditAccountBalance = accounts[transaction.CreditAccountQualifiedName].Balance + transaction.Amount;

      await _accountingDataAccess.UpdateAccountBalances(
        new Dictionary<string, decimal>
        {
          { transaction.DebitAccountQualifiedName, newDebitAccountBalance },
          { transaction.CreditAccountQualifiedName, newCreditAccountBalance }
        });
    }
  }
}

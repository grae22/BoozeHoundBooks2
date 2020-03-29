using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using bhb2core.Accounting.Interfaces;
using bhb2core.Accounting.Models;

namespace bhb2core.Accounting
{
  internal class TransactionEngine : ITransactionEngine
  {
    private readonly IAccountingDataAccess _accountingDataAccess;

    public TransactionEngine(in IAccountingDataAccess accountingDataAccess)
    {
      _accountingDataAccess = accountingDataAccess ?? throw new ArgumentNullException(nameof(accountingDataAccess));
    }

    public async Task ProcessTransaction(Transaction transaction)
    {
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

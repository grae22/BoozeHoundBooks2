using System.Threading.Tasks;

using bhb2core.Accounting.DataAccess.ActionResults;
using bhb2core.Accounting.Engines.AccountingEngine.Interfaces;
using bhb2core.Accounting.Engines.AccountingEngine.SubEngines;
using bhb2core.Accounting.Interfaces;
using bhb2core.Accounting.Models;
using bhb2core.Common.ActionResults;
using bhb2core.Utils.Logging;

namespace bhb2core.Accounting.Engines.AccountingEngine
{
  internal class AccountingEngine : IAccountingEngine
  {
    private readonly IAccountEngine _accountEngine;
    private readonly ITransactionEngine _transactionEngine;

    public AccountingEngine(
      in IAccountingDataAccess accountingDataAccess,
      in ILogger logger)
    {
      _accountEngine = new AccountEngine(
        accountingDataAccess,
        logger);

      _transactionEngine = new TransactionEngine(
        accountingDataAccess,
        logger);
    }

    public async Task<ActionResult> CreateBaseAccountsIfMissing()
    {
      return await _accountEngine.CreateBaseAccountsIfMissing();
    }

    public string BuildAccountQualifiedName(in string name, in string parentQualifiedName)
    {
      return _accountEngine.BuildAccountQualifiedName(name, parentQualifiedName);
    }

    public bool ValidateNewAccount(in NewAccount newAccount, out string error)
    {
      return _accountEngine.ValidateNewAccount(newAccount, out error);
    }

    public async Task<ActionResult> AddAccount(NewAccount newAccount)
    {
      return await _accountEngine.AddAccount(newAccount);
    }

    public async Task<bool> DoesAccountExist(string accountQualifiedName)
    {
      return await _accountEngine.DoesAccountExist(accountQualifiedName);
    }

    public async Task<DoubleEntryUpdateBalanceResult> PerformDoubleEntryUpdateAccountBalance(
      string debitAccountQualifiedName,
      string creditAccountQualifiedName,
      decimal amount)
    {
      return await _accountEngine.PerformDoubleEntryUpdateAccountBalance(
        debitAccountQualifiedName,
        creditAccountQualifiedName,
        amount);
    }

    public async Task<ActionResult> AddTransaction(Transaction transaction)
    {
      return await _transactionEngine.AddTransaction(transaction);
    }
  }
}

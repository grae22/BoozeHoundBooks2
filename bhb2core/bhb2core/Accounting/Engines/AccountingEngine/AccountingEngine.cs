using System.Threading.Tasks;

using bhb2core.Accounting.ActionResults;
using bhb2core.Accounting.Engines.AccountingEngine.Interfaces;
using bhb2core.Accounting.Engines.AccountingEngine.SubManagers;
using bhb2core.Accounting.Interfaces;
using bhb2core.Accounting.Models;
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

    public string BuildAccountQualifiedName(in string name, in string parentQualifiedName)
    {
      return _accountEngine.BuildAccountQualifiedName(name, parentQualifiedName);
    }

    public bool ValidateNewAccount(in NewAccount newAccount, out string error)
    {
      return _accountEngine.ValidateNewAccount(newAccount, out error);
    }

    public async Task<AddAccountResult> AddAccount(NewAccount newAccount)
    {
      return await _accountEngine.AddAccount(newAccount);
    }

    public async Task<bool> DoesAccountExist(string accountQualifiedName)
    {
      return await _accountEngine.DoesAccountExist(accountQualifiedName);
    }

    public async Task ProcessTransaction(Transaction transaction)
    {
      await _transactionEngine.ProcessTransaction(transaction);
    }
  }
}

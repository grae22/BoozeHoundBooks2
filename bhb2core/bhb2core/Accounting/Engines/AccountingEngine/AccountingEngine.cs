using System.Threading.Tasks;

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

    public async Task CreateBaseAccountsIfMissing()
    {
      await _accountEngine.CreateBaseAccountsIfMissing();
    }

    public async Task AddAccount(NewAccount newAccount)
    {
      await _accountEngine.AddAccount(newAccount);
    }

    public async Task ProcessTransaction(Transaction transaction)
    {
      await _transactionEngine.ProcessTransaction(transaction);
    }
  }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using bhb2core.Accounting.Dto;
using bhb2core.Accounting.Interfaces;
using bhb2core.Accounting.Managers.AccountingManager.ActionResults;
using bhb2core.Accounting.Managers.AccountingManager.Interfaces;
using bhb2core.Accounting.Managers.AccountingManager.SubManagers;
using bhb2core.Utils.Logging;
using bhb2core.Utils.Mapping;

namespace bhb2core.Accounting.Managers.AccountingManager
{
  // NOTE: Don't make this public - add a factory and other assemblies can use that.
  internal class AccountingManager : IAccountingManager
  {
    private readonly IAccountingEngine _accountingEngine;
    private readonly ILogger _logger;

    private readonly IAccountManager _accountManager;
    private readonly ITransactionManager _transactionManager;

    public AccountingManager(
      in IAccountingEngine accountingEngine,
      in IAccountingDataAccess accountingDataAccess,
      in IMapper mapper,
      in ILogger logger)
    {
      _accountingEngine = accountingEngine ?? throw new ArgumentNullException(nameof(accountingEngine));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));

      _accountManager = new AccountManager(
        accountingDataAccess,
        mapper,
        _logger);

      _transactionManager = new TransactionManager(
        _accountingEngine,
        mapper,
        _logger);
    }

    public async Task Initialise()
    {
      _logger.LogInformation("Initialising...");

      await _accountingEngine.CreateBaseAccountsIfMissing();
    }

    public async Task<IEnumerable<AccountDto>> GetAllAccounts()
    {
      return await _accountManager.GetAllAccounts();
    }

    public async Task<AddAccountResult> AddAccount(AccountDto accountDto)
    {
      return await _accountManager.AddAccount(accountDto);
    }

    public async Task ProcessTransaction(TransactionDto transactionDto)
    {
      await _transactionManager.ProcessTransaction(transactionDto);
    }
  }
}

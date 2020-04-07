using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using bhb2core.Accounting.Dto;
using bhb2core.Accounting.Interfaces;
using bhb2core.Accounting.Managers.AccountingManager.ActionResults;
using bhb2core.Accounting.Managers.AccountingManager.Interfaces;
using bhb2core.Accounting.Managers.AccountingManager.SubManagers;
using bhb2core.Common.ActionResults;
using bhb2core.Utils.Logging;
using bhb2core.Utils.Mapping;

namespace bhb2core.Accounting.Managers.AccountingManager
{
  // NOTE: Don't make this public - add a factory and other assemblies can use that.
  internal class AccountingManager : IAccountingManager
  {
    private readonly IAccountManager _accountManager;
    private readonly ITransactionManager _transactionManager;
    private readonly ILogger _logger;

    public AccountingManager(
      in IAccountingEngine accountingEngine,
      in IAccountingDataAccess accountingDataAccess,
      in IMapper mapper,
      in ILogger logger)
    {
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));

      _accountManager = new AccountManager(
        accountingEngine,
        accountingDataAccess,
        mapper,
        logger);

      _transactionManager = new TransactionManager(
        accountingDataAccess,
        accountingEngine,
        mapper,
        logger);
    }

    public async Task Initialise()
    {
      await _accountManager.Initialise();
    }

    public async Task<GetResult<IEnumerable<AccountDto>>> GetAllAccounts()
    {
      return await _accountManager.GetAllAccounts();
    }

    public async Task<GetResult<IEnumerable<AccountDto>>> GetTransactionDebitAccounts()
    {
      return await _accountManager.GetTransactionDebitAccounts();
    }

    public async Task<GetResult<IEnumerable<AccountDto>>> GetTransactionCreditAccounts()
    {
      return await _accountManager.GetTransactionCreditAccounts();
    }

    public async Task<ActionResult> AddAccount(NewAccountDto newAccountDto)
    {
      try
      {
        return await _accountManager.AddAccount(newAccountDto);
      }
      catch (Exception ex)
      {
        _logger.LogError("Unhandled exception.", ex);

        return ActionResult.CreateFailure($"Unhandled error: \"{ex.Message}\".");
      }
    }

    public async Task<ProcessTransactionResult> ProcessTransaction(TransactionDto transactionDto)
    {
      try
      {
        return await _transactionManager.ProcessTransaction(transactionDto);
      }
      catch (Exception ex)
      {
        _logger.LogError("Unhandled exception.", ex);

        return ProcessTransactionResult.CreateFailure($"Unhandled error: \"{ex.Message}\".");
      }
    }
  }
}

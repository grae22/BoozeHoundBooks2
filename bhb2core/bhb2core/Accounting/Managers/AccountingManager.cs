using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using bhb2core.Accounting.Dto;
using bhb2core.Accounting.Interfaces;
using bhb2core.Accounting.Managers.ActionResults;
using bhb2core.Accounting.Managers.Interfaces;
using bhb2core.Accounting.Managers.SubManagers;
using bhb2core.Common.ActionResults;
using bhb2core.Utils.Logging;
using bhb2core.Utils.Mapping;

namespace bhb2core.Accounting.Managers
{
  // NOTE: Don't make this public - add a factory and other assemblies can use that.
  internal class AccountingManager : IAccountingManager
  {
    private readonly IAccountingDataAccess _accountingDataAccess;
    private readonly IAccountManager _accountManager;
    private readonly ITransactionManager _transactionManager;
    private readonly ILogger _logger;

    public AccountingManager(
      in IAccountingEngine accountingEngine,
      in IAccountingDataAccess accountingDataAccess,
      in IMapper mapper,
      in ILogger logger)
    {
      _accountingDataAccess = accountingDataAccess ?? throw new ArgumentNullException(nameof(accountingDataAccess));
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

    public async Task<ActionResult> Initialise()
    {
      ActionResult dataAccessInitialiseResult = await _accountingDataAccess.Initialise();

      if (!dataAccessInitialiseResult.IsSuccess)
      {
        return dataAccessInitialiseResult;
      }

      ActionResult accountManagerInitialiseResult = await _accountManager.Initialise();

      if (!accountManagerInitialiseResult.IsSuccess)
      {
        return accountManagerInitialiseResult;
      }

      return ActionResult.CreateSuccess();
    }

    public async Task<GetResult<IEnumerable<AccountDto>>> GetAllAccounts()
    {
      try
      {
        return await _accountManager.GetAllAccounts();
      }
      catch (Exception ex)
      {
        _logger.LogError("Unhandled exception.", ex);

        return GetResult<IEnumerable<AccountDto>>.CreateFailure($"Unhandled error: \"{ex.Message}\".");
      }
    }

    public async Task<GetResult<IEnumerable<AccountDto>>> GetTransactionDebitAccounts()
    {
      try
      {
        return await _accountManager.GetTransactionDebitAccounts();
      }
      catch (Exception ex)
      {
        _logger.LogError("Unhandled exception.", ex);

        return GetResult<IEnumerable<AccountDto>>.CreateFailure($"Unhandled error: \"{ex.Message}\".");
      }
    }

    public async Task<GetResult<IEnumerable<AccountDto>>> GetTransactionCreditAccounts()
    {
      try
      {
        return await _accountManager.GetTransactionCreditAccounts();
      }
      catch (Exception ex)
      {
        _logger.LogError("Unhandled exception.", ex);

        return GetResult<IEnumerable<AccountDto>>.CreateFailure($"Unhandled error: \"{ex.Message}\".");
      }
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

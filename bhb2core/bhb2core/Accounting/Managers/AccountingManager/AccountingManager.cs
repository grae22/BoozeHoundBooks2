using System.Collections.Generic;
using System.Threading.Tasks;

using bhb2core.Accounting.DataAccess.ActionResults;
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
    private readonly IAccountManager _accountManager;
    private readonly ITransactionManager _transactionManager;

    public AccountingManager(
      in IAccountingEngine accountingEngine,
      in IAccountingDataAccess accountingDataAccess,
      in IMapper mapper,
      in ILogger logger)
    {
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

    public async Task<IEnumerable<AccountDto>> GetAllAccounts()
    {
      return await _accountManager.GetAllAccounts();
    }

    public async Task<IEnumerable<AccountDto>> GetTransactionDebitAccounts()
    {
      return await _accountManager.GetTransactionDebitAccounts();
    }

    public async Task<IEnumerable<AccountDto>> GetTransactionCreditAccounts()
    {
      return await _accountManager.GetTransactionCreditAccounts();
    }

    public async Task<AddAccountResult> AddAccount(NewAccountDto newAccountDto)
    {
      return await _accountManager.AddAccount(newAccountDto);
    }

    public async Task<ProcessTransactionResult> ProcessTransaction(TransactionDto transactionDto)
    {
      return await _transactionManager.ProcessTransaction(transactionDto);
    }
  }
}

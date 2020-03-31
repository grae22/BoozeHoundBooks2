using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using bhb2core.Accounting.Dto;
using bhb2core.Accounting.Exceptions;
using bhb2core.Accounting.Interfaces;
using bhb2core.Accounting.Managers.AccountingManager.ActionResults;
using bhb2core.Accounting.Models;
using bhb2core.Utils.Logging;
using bhb2core.Utils.Mapping;

namespace bhb2core.Accounting.Managers.AccountingManager
{
  // NOTE: Don't make this public - add a factory and other assemblies can use that.
  internal class AccountingManager : IAccountingManager
  {
    private readonly IAccountingEngine _accountingEngine;
    private readonly IAccountingDataAccess _accountingDataAccess;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public AccountingManager(
      in IAccountingEngine accountingEngine,
      in IAccountingDataAccess accountingDataAccess,
      in IMapper mapper,
      in ILogger logger)
    {
      _accountingEngine = accountingEngine ?? throw new ArgumentNullException(nameof(accountingEngine));
      _accountingDataAccess = accountingDataAccess ?? throw new ArgumentNullException(nameof(accountingDataAccess));
      _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Initialise()
    {
      _logger.LogInformation("Initialising...");

      await _accountingEngine.CreateBaseAccountsIfMissing();
    }

    public async Task<IEnumerable<AccountDto>> GetAllAccounts()
    {
      _logger.LogVerbose("Request received for all accounts.");

      IEnumerable<Account> accounts = await _accountingDataAccess.GetAllAccounts();

      IEnumerable<AccountDto> accountDtos = _mapper.Map<IEnumerable<Account>, IEnumerable<AccountDto>>(accounts);

      return accountDtos;
    }

    public async Task<AddAccountResult> AddAccount(AccountDto accountDto)
    {
      _logger.LogVerbose($"Add account request received, account details: {accountDto}");

      if (accountDto == null)
      {
        return AddAccountResult.CreateFailure($"Argument \"{nameof(accountDto)}\" cannot be null.");
      }

      Account account = _mapper.Map<AccountDto, Account>(accountDto);

      try
      {
        await _accountingDataAccess.AddAccount(account);
      }
      catch (AccountAlreadyExistsException ex)
      {
        _logger.LogError($"Failed to add account, already exists: \"{ex.Message}\".");

        return AddAccountResult.CreateFailure(ex.Message);
      }

      _logger.LogInformation($"Account added: {account}");

      return AddAccountResult.CreateSuccess();
    }

    public async Task ProcessTransaction(TransactionDto transactionDto)
    {
      _logger.LogVerbose($"Transaction received: {transactionDto}.");

      Transaction transaction = _mapper.Map<TransactionDto, Transaction>(transactionDto);

      await _accountingEngine.ProcessTransaction(transaction);

      _logger.LogInformation($"Transaction processed: {transaction}");
    }
  }
}

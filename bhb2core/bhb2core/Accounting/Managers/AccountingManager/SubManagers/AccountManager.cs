using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using bhb2core.Accounting.Dto;
using bhb2core.Accounting.Exceptions;
using bhb2core.Accounting.Interfaces;
using bhb2core.Accounting.Managers.AccountingManager.ActionResults;
using bhb2core.Accounting.Managers.AccountingManager.Interfaces;
using bhb2core.Accounting.Models;
using bhb2core.Utils.Logging;
using bhb2core.Utils.Mapping;

namespace bhb2core.Accounting.Managers.AccountingManager.SubManagers
{
  internal class AccountManager : IAccountManager
  {
    private static readonly string[] BaseAccountNames =
    {
      "Funds",
      "Income",
      "Expense",
      "Debtor",
      "Creditor"
    };

    private readonly IAccountingEngine _accountingEngine;
    private readonly IAccountingDataAccess _accountingDataAccess;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public AccountManager(
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

      await CreateBaseAccountsIfMissing();
    }

    public async Task<IEnumerable<AccountDto>> GetAllAccounts()
    {
      _logger.LogVerbose("Request received for all accounts.");

      IEnumerable<Account> accounts = await _accountingDataAccess.GetAllAccounts();

      IEnumerable<AccountDto> accountDtos = _mapper.Map<IEnumerable<Account>, IEnumerable<AccountDto>>(accounts);

      return accountDtos;
    }

    public async Task<IEnumerable<AccountDto>> GetTransactionDebitAccounts()
    {
      throw new NotImplementedException();
    }

    public async Task<IEnumerable<AccountDto>> GetTransactionCreditAccounts()
    {
      throw new NotImplementedException();
    }

    public async Task<AddAccountResult> AddAccount(NewAccountDto newAccountDto)
    {
      _logger.LogVerbose($"Add account request received, account details: {newAccountDto}");

      if (newAccountDto == null)
      {
        throw new ArgumentNullException(nameof(newAccountDto));
      }

      NewAccount newAccount = _mapper.Map<NewAccountDto, NewAccount>(newAccountDto);

      bool isNewAccountValid = _accountingEngine.ValidateNewAccount(
        newAccount,
        out string validationError);

      if (!isNewAccountValid)
      {
        _logger.LogError($"New account validation failed: \"{validationError}\" : {newAccount}");

        return AddAccountResult.CreateFailure(validationError);
      }

      bool parentAccountExists = await _accountingEngine.DoesAccountExist(newAccount.ParentAccountId);

      if (!parentAccountExists)
      {
        _logger.LogError($"Parent account id not found for new account: {newAccount}");

        return AddAccountResult.CreateFailure("Parent account not found.");
      }

      try
      {
        await _accountingEngine.AddAccount(newAccount);
      }
      catch (AccountException ex)
      {
        _logger.LogError($"Failed to add account: \"{ex.Message}\". Details: \"{ex.Details}\".");

        return AddAccountResult.CreateFailure(ex.Message);
      }

      return AddAccountResult.CreateSuccess();
    }

    private async Task CreateBaseAccountsIfMissing()
    {
      foreach (var name in BaseAccountNames)
      {
        string id = _accountingEngine.BuildAccountId(name, null);

        if (await _accountingEngine.DoesAccountExist(id))
        {
          _logger.LogVerbose($"Found \"{name}\" base account.");
          continue;
        }

        await _accountingEngine.AddAccount(
          new NewAccount
          {
            Name = name,
            ParentAccountId = null
          });

        _logger.LogInformation($"Added \"{name}\" base account.");
      }
    }
  }
}

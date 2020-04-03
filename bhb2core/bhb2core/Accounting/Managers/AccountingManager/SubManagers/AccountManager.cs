using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using bhb2core.Accounting.ActionResults;
using bhb2core.Accounting.Dto;
using bhb2core.Accounting.Exceptions;
using bhb2core.Accounting.Interfaces;
using bhb2core.Accounting.Managers.AccountingManager.Interfaces;
using bhb2core.Accounting.Models;
using bhb2core.Utils.Logging;
using bhb2core.Utils.Mapping;
using bhb2core.Utils.Serialisation;

namespace bhb2core.Accounting.Managers.AccountingManager.SubManagers
{
  internal class AccountManager : IAccountManager
  {
    private const string FundsAccountName = "Funds";
    private const string IncomeAccountName = "Income";
    private const string ExpenseAccountName = "Expense";
    private const string DebtorAccountName = "Debtor";
    private const string CreditorAccountName = "Creditor";

    private static readonly string[] BaseAccountNames =
    {
      FundsAccountName,
      IncomeAccountName,
      ExpenseAccountName,
      DebtorAccountName,
      CreditorAccountName
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

      _logger.LogVerbose($"Returning accounts: {Serialiser.Serialise(accountDtos)}");

      return accountDtos;
    }

    public async Task<IEnumerable<AccountDto>> GetTransactionDebitAccounts()
    {
      _logger.LogVerbose("Request received for transaction debit accounts.");

      IEnumerable<Account> accounts = await _accountingDataAccess.GetAccounts(
        isFunds: true,
        isIncome: true,
        isDebtor: true,
        isCreditor: true);

      IEnumerable<AccountDto> accountDtos = _mapper.Map<IEnumerable<Account>, IEnumerable<AccountDto>>(accounts);

      _logger.LogVerbose($"Returning transaction debit accounts: {Serialiser.Serialise(accountDtos)}");

      return accountDtos;
    }

    public async Task<IEnumerable<AccountDto>> GetTransactionCreditAccounts()
    {
      _logger.LogVerbose("Request received for transaction credit accounts.");

      IEnumerable<Account> accounts = await _accountingDataAccess.GetAccounts(
        isFunds: true,
        isExpense: true,
        isDebtor: true,
        isCreditor: true);

      IEnumerable<AccountDto> accountDtos = _mapper.Map<IEnumerable<Account>, IEnumerable<AccountDto>>(accounts);

      _logger.LogVerbose($"Returning transaction credit accounts: {Serialiser.Serialise(accountDtos)}");

      return accountDtos;
    }

    public async Task<AddAccountResult> AddAccount(NewAccountDto newAccountDto)
    {
      _logger.LogVerbose($"Add account request received, account details: {newAccountDto}");

      if (newAccountDto == null)
      {
        throw new ArgumentNullException(nameof(newAccountDto));
      }

      GetAccountResult getParentAccountResult = await _accountingDataAccess.GetAccountById(newAccountDto.ParentAccountId);

      if (!getParentAccountResult.IsSuccess)
      {
        _logger.LogError($"Parent account not found for new account: {newAccountDto}");

        return AddAccountResult.CreateFailure("Parent account not found.");
      }

      var newAccount = new NewAccount
      {
        Name = newAccountDto.Name,
        ParentAccount = getParentAccountResult.Result
      };

      bool isNewAccountValid = _accountingEngine.ValidateNewAccount(
        newAccount,
        out string validationError);

      if (!isNewAccountValid)
      {
        _logger.LogError($"New account validation failed: \"{validationError}\" : {newAccount}");

        return AddAccountResult.CreateFailure(validationError);
      }

      try
      {
        return await _accountingEngine.AddAccount(newAccount);
      }
      catch (AccountException ex)
      {
        _logger.LogError($"Failed to add account: \"{ex.Message}\". Details: \"{ex.Details}\".");

        return AddAccountResult.CreateFailure(ex.Message);
      }
    }

    private async Task CreateBaseAccountsIfMissing()
    {
      _logger.LogInformation("Creating base accounts if missing...");

      var missingAccounts = new List<string>();

      foreach (var name in BaseAccountNames)
      {
        string id = _accountingEngine.BuildAccountId(name, null);

        if (await _accountingEngine.DoesAccountExist(id))
        {
          _logger.LogVerbose($"Found \"{name}\" base account.");
          continue;
        }

        missingAccounts.Add(name);

        _logger.LogInformation($"\"{name}\" base account not found.");
      }

      if (missingAccounts.Contains(FundsAccountName))
      {
        await CreateBaseAccount(FundsAccountName, isFunds: true);
      }

      if (missingAccounts.Contains(IncomeAccountName))
      {
        await CreateBaseAccount(IncomeAccountName, isIncome: true);
      }

      if (missingAccounts.Contains(ExpenseAccountName))
      {
        await CreateBaseAccount(ExpenseAccountName, isExpense: true);
      }

      if (missingAccounts.Contains(DebtorAccountName))
      {
        await CreateBaseAccount(DebtorAccountName, isDebtor: true);
      }

      if (missingAccounts.Contains(CreditorAccountName))
      {
        await CreateBaseAccount(CreditorAccountName, isCreditor: true);
      }
    }

    private async Task CreateBaseAccount(
      string name,
      bool isFunds = false,
      bool isIncome = false,
      bool isExpense = false,
      bool isDebtor = false,
      bool isCreditor = false)
    {
      var account = new Account
      {
        Id = _accountingEngine.BuildAccountId(name, null),
        Name = name,
        ParentAccountId = null,
        Balance = 0m,
        IsFunds = isFunds,
        IsIncome = isIncome,
        IsExpense = isExpense,
        IsDebtor = isDebtor,
        IsCreditor = isCreditor
      };

      await _accountingDataAccess.AddAccount(account);

      _logger.LogInformation($"Added \"{name}\" base account: {account}");
    }
  }
}

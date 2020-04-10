using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using bhb2core.Accounting.Dto;
using bhb2core.Accounting.Exceptions;
using bhb2core.Accounting.Interfaces;
using bhb2core.Accounting.Managers.Interfaces;
using bhb2core.Accounting.Models;
using bhb2core.Common.ActionResults;
using bhb2core.Utils.Logging;
using bhb2core.Utils.Mapping;
using bhb2core.Utils.Serialisation;

namespace bhb2core.Accounting.Managers.SubManagers
{
  internal class AccountManager : IAccountManager
  {
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

    public async Task<ActionResult> Initialise()
    {
      _logger.LogInformation("Initialising...");

      ActionResult createBaseAccountsResult = await _accountingEngine.CreateBaseAccountsIfMissing();

      if (!createBaseAccountsResult.IsSuccess)
      {
        _logger.LogError(
          $"Initialisation failed - error while creating base accounts: {createBaseAccountsResult.FailureMessage}");

        return ActionResult.CreateFailure("Error while creating base accounts.");
      }

      return ActionResult.CreateSuccess();
    }

    public async Task<GetResult<IEnumerable<AccountDto>>> GetAllAccounts()
    {
      _logger.LogVerbose("Request received for all accounts.");

      GetResult<IEnumerable<Account>> accountsResult = await _accountingDataAccess.GetAllAccounts();

      if (!accountsResult.IsSuccess)
      {
        return GetResult<IEnumerable<AccountDto>>.CreateFailure(accountsResult.FailureMessage);
      }

      IEnumerable<AccountDto> accountDtos = _mapper.Map<IEnumerable<Account>, IEnumerable<AccountDto>>(accountsResult.Result);

      _logger.LogVerbose($"Returning accounts: {Serialiser.Serialise(accountDtos)}");

      return GetResult<IEnumerable<AccountDto>>.CreateSuccess(accountDtos);
    }

    public async Task<GetResult<IEnumerable<AccountDto>>> GetTransactionDebitAccounts()
    {
      _logger.LogVerbose("Request received for transaction debit accounts.");

      GetResult<IEnumerable<Account>> accountsResult = await _accountingDataAccess.GetAccounts(
        isFunds: true,
        isIncome: true,
        isDebtor: true,
        isCreditor: true);

      if (!accountsResult.IsSuccess)
      {
        return GetResult<IEnumerable<AccountDto>>.CreateFailure(accountsResult.FailureMessage);
      }

      var leafChildrenOnly = new List<Account>(accountsResult.Result);

      RemoveAccountsWithChildren(leafChildrenOnly);

      IEnumerable<AccountDto> accountDtos = _mapper.Map<IEnumerable<Account>, IEnumerable<AccountDto>>(leafChildrenOnly);

      _logger.LogVerbose($"Returning transaction debit accounts: {Serialiser.Serialise(accountDtos)}");

      return GetResult<IEnumerable<AccountDto>>.CreateSuccess(accountDtos);
    }

    public async Task<GetResult<IEnumerable<AccountDto>>> GetTransactionCreditAccounts()
    {
      _logger.LogVerbose("Request received for transaction credit accounts.");

      GetResult<IEnumerable<Account>> accountsResult = await _accountingDataAccess.GetAccounts(
        isFunds: true,
        isExpense: true,
        isDebtor: true,
        isCreditor: true);

      if (!accountsResult.IsSuccess)
      {
        return GetResult<IEnumerable<AccountDto>>.CreateFailure(accountsResult.FailureMessage);
      }

      var leafChildrenOnly = new List<Account>(accountsResult.Result);

      RemoveAccountsWithChildren(leafChildrenOnly);

      IEnumerable<AccountDto> accountDtos = _mapper.Map<IEnumerable<Account>, IEnumerable<AccountDto>>(leafChildrenOnly);

      _logger.LogVerbose($"Returning transaction credit accounts: {Serialiser.Serialise(accountDtos)}");

      return GetResult<IEnumerable<AccountDto>>.CreateSuccess(accountDtos);
    }

    public async Task<ActionResult> AddAccount(NewAccountDto newAccountDto)
    {
      _logger.LogVerbose($"Add account request received, account details: {newAccountDto}");

      if (newAccountDto == null)
      {
        throw new ArgumentNullException(nameof(newAccountDto));
      }

      GetResult<Account> getParentAccountResult =
        await _accountingDataAccess.GetAccount(newAccountDto.ParentAccountQualifiedName);

      if (!getParentAccountResult.IsSuccess)
      {
        _logger.LogError($"Parent account not found for new account: {newAccountDto}");

        return ActionResult.CreateFailure("Parent account not found.");
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

        return ActionResult.CreateFailure(validationError);
      }

      try
      {
        return await _accountingEngine.AddAccount(newAccount);
      }
      catch (AccountException ex)
      {
        _logger.LogError($"Failed to add account: \"{ex.Message}\". Details: \"{ex.Details}\".");

        return ActionResult.CreateFailure(ex.Message);
      }
    }

    private static void RemoveAccountsWithChildren(in List<Account> accounts)
    {
      var accountsToRemove = new List<Account>();

      foreach (var account in accounts)
      {
        bool accountIsParentToOtherAccounts =
          accounts.Any(a =>
            a.HasParent &&
            a.ParentAccountQualifiedName.Equals(account.QualifiedName));

        if (accountIsParentToOtherAccounts)
        {
          accountsToRemove.Add(account);
        }
      }

      foreach (var account in accountsToRemove)
      {
        accounts.Remove(account);
      }
    }
  }
}

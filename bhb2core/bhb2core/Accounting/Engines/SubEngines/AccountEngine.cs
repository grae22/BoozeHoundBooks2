using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using bhb2core.Accounting.DataAccess.ActionResults;
using bhb2core.Accounting.Engines.Interfaces;
using bhb2core.Accounting.Interfaces;
using bhb2core.Accounting.Models;
using bhb2core.Common.ActionResults;
using bhb2core.Utils.Extensions;
using bhb2core.Utils.Logging;
using bhb2core.Utils.Serialisation;

namespace bhb2core.Accounting.Engines.SubEngines
{
  internal class AccountEngine : IAccountEngine
  {
    private const string FundsAccountName = "Funds";
    private const string IncomeAccountName = "Income";
    private const string ExpenseAccountName = "Expense";
    private const string DebtorAccountName = "Debtor";
    private const string CreditorAccountName = "Creditor";

    private const char AccountQualifiedNameSeparator = '.';

    private static readonly string[] BaseAccountNames =
    {
      FundsAccountName,
      IncomeAccountName,
      ExpenseAccountName,
      DebtorAccountName,
      CreditorAccountName
    };

    private readonly IAccountingDataAccess _accountingDataAccess;
    private readonly ILogger _logger;

    public AccountEngine(
      in IAccountingDataAccess accountingDataAccess,
      in ILogger logger)
    {
      _accountingDataAccess = accountingDataAccess ?? throw new ArgumentNullException(nameof(accountingDataAccess));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ActionResult> CreateBaseAccountsIfMissing()
    {
      _logger.LogInformation("Creating base accounts if missing...");

      var missingAccounts = new List<string>();

      foreach (var name in BaseAccountNames)
      {
        if (await DoesAccountExist(name))
        {
          _logger.LogVerbose($"Found \"{name}\" base account.");
          continue;
        }

        missingAccounts.Add(name);

        _logger.LogInformation($"\"{name}\" base account not found.");
      }

      if (missingAccounts.Contains(FundsAccountName))
      {
        ActionResult result = await CreateBaseAccount(
          FundsAccountName,
          new[]
          {
            AccountType.CreateFunds(),
            AccountType.CreateExpense(),
            AccountType.CreateDebtor(),
            AccountType.CreateCreditor()
          },
          new[]
          {
            AccountType.CreateFunds(),
            AccountType.CreateIncome(),
            AccountType.CreateDebtor(),
            AccountType.CreateCreditor()
          },
          isFunds: true);

        if (!result.IsSuccess)
        {
          return result;
        }
      }

      if (missingAccounts.Contains(IncomeAccountName))
      {
        ActionResult result = await CreateBaseAccount(
          IncomeAccountName,
          new[]
          {
            AccountType.CreateFunds()
          },
          new AccountType[] { },
          isIncome: true);

        if (!result.IsSuccess)
        {
          return result;
        }
      }

      if (missingAccounts.Contains(ExpenseAccountName))
      {
        ActionResult result = await CreateBaseAccount(
          ExpenseAccountName,
          new AccountType[] { },
          new[]
          {
            AccountType.CreateFunds()
          },
          isExpense: true);

        if (!result.IsSuccess)
        {
          return result;
        }
      }

      if (missingAccounts.Contains(DebtorAccountName))
      {
        ActionResult result = await CreateBaseAccount(
          DebtorAccountName,
          new[]
          {
            AccountType.CreateFunds()
          },
          new[]
          {
            AccountType.CreateFunds()
          },
          isDebtor: true);

        if (!result.IsSuccess)
        {
          return result;
        }
      }

      if (missingAccounts.Contains(CreditorAccountName))
      {
        ActionResult result = await CreateBaseAccount(
          CreditorAccountName,
          new[]
          {
            AccountType.CreateFunds(),
          },
          new[]
          {
            AccountType.CreateFunds(),
          },
          isCreditor: true);

        if (!result.IsSuccess)
        {
          return result;
        }
      }

      return ActionResult.CreateSuccess();
    }

    public string BuildAccountQualifiedName(in string name, in string parentQualifiedName)
    {
      string qualifiedName;

      if (parentQualifiedName == null)
      {
        qualifiedName = name;
      }
      else
      {
        qualifiedName = $"{parentQualifiedName}{AccountQualifiedNameSeparator}{name}";
      }

      return qualifiedName;
    }

    public bool ValidateNewAccount(in NewAccount newAccount, out string error)
    {
      error = null;

      if (string.IsNullOrWhiteSpace(newAccount.Name))
      {
        error = "Account name cannot be null, empty or whitespace.";
        return false;
      }

      if (newAccount.Name.Contains(AccountQualifiedNameSeparator))
      {
        error = $"Account name cannot contain the character '{AccountQualifiedNameSeparator}'.";
        return false;
      }

      if (newAccount.ParentAccount == null)
      {
        error = "Parent account cannot be null.";
        return false;
      }

      return true;
    }

    public async Task<ActionResult> AddAccount(NewAccount newAccount)
    {
      _logger.LogVerbose($"Add account request received, account details: {newAccount}");

      string sanitisedAccountName = newAccount.Name.Trim();
      string accountQualifiedName = BuildAccountQualifiedName(sanitisedAccountName, newAccount.ParentAccount.QualifiedName);

      var account = new Account
      {
        AccountType = newAccount.ParentAccount.AccountType,
        QualifiedName = accountQualifiedName,
        Name = sanitisedAccountName,
        ParentAccountQualifiedName = newAccount.ParentAccount.QualifiedName,
        AccountTypesWithDebitPermission = newAccount.ParentAccount.AccountTypesWithDebitPermission.Clone(),
        AccountTypesWithCreditPermission = newAccount.ParentAccount.AccountTypesWithCreditPermission.Clone(),
        Balance = 0
      };

      ActionResult result = await _accountingDataAccess.AddAccount(account);

      if (result.IsSuccess)
      {
        _logger.LogInformation($"Account added: {account}");
      }
      else
      {
        _logger.LogError($"Failed to add account: {result.FailureMessage}");
      }

      return result;
    }

    public async Task<bool> DoesAccountExist(string accountQualifiedName)
    {
      GetResult<Account> result = await _accountingDataAccess.GetAccount(accountQualifiedName);

      return result.IsSuccess;
    }

    // TODO: Method needs refactoring - decompose?
    public async Task<UpdateAccountBalancesResult> PerformDoubleEntryUpdateAccountBalance(
      string debitAccountQualifiedName,
      string creditAccountQualifiedName,
      decimal amount)
    {
      _logger.LogInformation($"Attempting to move {amount:N} from \"{debitAccountQualifiedName}\" to \"{creditAccountQualifiedName}\"...");

      // Get accounts.
      GetResult<IReadOnlyDictionary<string, Account>> getAccountsResult = await _accountingDataAccess.GetAccounts(
        new[]
        {
          debitAccountQualifiedName,
          creditAccountQualifiedName
        });

      if (!getAccountsResult.IsSuccess)
      {
        var message = $"Error retrieving double-entry accounts: {getAccountsResult.FailureMessage}";

        _logger.LogError(message);

        return UpdateAccountBalancesResult.CreateFailure(message);
      }

      IReadOnlyDictionary<string, Account> accounts = getAccountsResult.Result;

      Account debitAccount = accounts[debitAccountQualifiedName];
      Account creditAccount = accounts[creditAccountQualifiedName];

      bool isParentAccountShared =
        debitAccount.HasParent &&
        creditAccount.HasParent &&
        debitAccount.ParentAccountQualifiedName.Equals(
          creditAccount.ParentAccountQualifiedName,
          StringComparison.Ordinal);

      // Don't allow transfer from an account which isn't a leaf (no children) account.
      if (await _accountingDataAccess.IsParentAccount(debitAccountQualifiedName))
      {
        string message = $"Parent accounts cannot be transacted upon, \"{debitAccountQualifiedName}\" has children.";

        _logger.LogError(message);

        return UpdateAccountBalancesResult.CreateFailure(message);
      }

      if (await _accountingDataAccess.IsParentAccount(creditAccountQualifiedName))
      {
        string message = $"Parent accounts cannot be transacted upon, \"{creditAccountQualifiedName}\" has children.";

        _logger.LogError(message);

        return UpdateAccountBalancesResult.CreateFailure(message);
      }

      // Check accounts can transact.
      if (!debitAccount.AccountTypesWithDebitPermission.Contains(creditAccount.AccountType))
      {
        string message = $"Funds cannot be moved from \"{debitAccount.QualifiedName}\" to \"{creditAccount.QualifiedName}\".";

        _logger.LogError(message);

        return UpdateAccountBalancesResult.CreateFailure(message);
      }

      if (!creditAccount.AccountTypesWithCreditPermission.Contains(debitAccount.AccountType))
      {
        string message = $"Funds cannot be moved to \"{creditAccount.QualifiedName}\" from \"{debitAccount.QualifiedName}\".";

        _logger.LogError(message);

        return UpdateAccountBalancesResult.CreateFailure(message);
      }

      IEnumerable<Account> parentDebitAccounts = new Account[0];
      IEnumerable<Account> parentCreditAccounts = new Account[0];

      if (!isParentAccountShared)
      {
        // Get debit account parent accounts.
        GetResult<IEnumerable<Account>> getParentAccountsResult =
          await _accountingDataAccess.GetParentAccountsOrdered(debitAccountQualifiedName);

        if (!getParentAccountsResult.IsSuccess)
        {
          return UpdateAccountBalancesResult.CreateFailure(getParentAccountsResult.FailureMessage);
        }

        parentDebitAccounts = getParentAccountsResult.Result;

        // Get credit account parent accounts.
        getParentAccountsResult =
          await _accountingDataAccess.GetParentAccountsOrdered(creditAccountQualifiedName);

        if (!getParentAccountsResult.IsSuccess)
        {
          return UpdateAccountBalancesResult.CreateFailure(getParentAccountsResult.FailureMessage);
        }

        parentCreditAccounts = getParentAccountsResult.Result;
      }

      // Update balances for all affected accounts.
      var updatedBalancesByAccount = new Dictionary<string, decimal>
      {
        { debitAccount.QualifiedName, debitAccount.Balance -= amount },
        { creditAccount.QualifiedName, creditAccount.Balance += amount }
      };

      parentDebitAccounts
        .ToList()
        .ForEach(a => updatedBalancesByAccount.Add(
          a.QualifiedName,
          a.Balance -= amount));

      parentCreditAccounts
        .ToList()
        .ForEach(a => updatedBalancesByAccount.Add(
          a.QualifiedName,
          a.Balance += amount));

      _logger.LogVerbose($"Attempting to update account balances: {Serialiser.Serialise(updatedBalancesByAccount)}");

      ActionResult updateBalanceResult = await _accountingDataAccess.UpdateAccountBalances(updatedBalancesByAccount);

      if (!updateBalanceResult.IsSuccess)
      {
        var message = $"Error updating all account balances: {updateBalanceResult.FailureMessage}";

        _logger.LogError(message);

        return UpdateAccountBalancesResult.CreateFailure(message);
      }

      var allUpdatedAccounts = new List<Account>(parentDebitAccounts);
      allUpdatedAccounts.AddRange(parentCreditAccounts);
      allUpdatedAccounts.Add(debitAccount);
      allUpdatedAccounts.Add(creditAccount);

      return UpdateAccountBalancesResult.CreateSuccess(allUpdatedAccounts);
    }

    private async Task<ActionResult> CreateBaseAccount(
      string name,
      IEnumerable<AccountType> accountTypesWithDebitPermission,
      IEnumerable<AccountType> accountTypesWithCreditPermission,
      bool isFunds = false,
      bool isIncome = false,
      bool isExpense = false,
      bool isDebtor = false,
      bool isCreditor = false)
    {
      var account = new Account
      {
        AccountType = new AccountType
        {
          IsFunds = isFunds,
          IsIncome = isIncome,
          IsExpense = isExpense,
          IsDebtor = isDebtor,
          IsCreditor = isCreditor
        },
        QualifiedName = name,
        Name = name,
        ParentAccountQualifiedName = null,
        Balance = 0m,
        AccountTypesWithDebitPermission = accountTypesWithDebitPermission,
        AccountTypesWithCreditPermission = accountTypesWithCreditPermission
      };

      ActionResult result = await _accountingDataAccess.AddAccount(account);

      if (!result.IsSuccess)
      {
        var message = $"Failed to create base account \"{name}\": {result.FailureMessage}";

        _logger.LogError(message);

        return ActionResult.CreateFailure(message);
      }

      _logger.LogInformation($"Added \"{name}\" base account: {account}");

      return ActionResult.CreateSuccess();
    }
  }
}

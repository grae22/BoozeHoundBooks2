using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using bhb2core.Accounting.Engines.AccountingEngine.Interfaces;
using bhb2core.Accounting.Exceptions;
using bhb2core.Accounting.Interfaces;
using bhb2core.Accounting.Models;
using bhb2core.Utils.Logging;

namespace bhb2core.Accounting.Engines.AccountingEngine.SubManagers
{
  internal class AccountEngine : IAccountEngine
  {
    private const char AccountIdSeparator = '.';

    private static readonly string[] BaseAccountNames =
    {
      "Funds",
      "Income",
      "Expense",
      "Debtor",
      "Creditor"
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

    public async Task CreateBaseAccountsIfMissing()
    {
      var baseAccountIds = new List<string>();

      BaseAccountNames
        .ToList()
        .ForEach(n => baseAccountIds.Add(BuildAccountId(n, null)));

      IReadOnlyDictionary<string, Account> accountsById = await _accountingDataAccess.GetAccountsById(baseAccountIds);

      foreach (var name in BaseAccountNames)
      {
        string id = BuildAccountId(name, null);

        if (accountsById.ContainsKey(id))
        {
          _logger.LogVerbose($"Found \"{name}\" base account.");
          continue;
        }

        await _accountingDataAccess.AddAccount(
          new Account
          {
            Id = id,
            Name = name,
            Balance = 0m
          });

        _logger.LogInformation($"Added \"{name}\" base account.");
      }
    }

    public async Task AddAccount(NewAccount newAccount)
    {
      _logger.LogVerbose($"Add account request received, account details: {newAccount}");

      ValidateNewAccount(newAccount);

      bool parentAccountExists = await DoesAccountExist(newAccount.ParentAccountId);

      if (!parentAccountExists)
      {
        throw new AccountException(
          $"Parent account id \"{newAccount.ParentAccountId}\" not found.",
          newAccount.ToString());
      }

      string sanitisedAccountName = newAccount.Name.Trim();
      string accountId = BuildAccountId(sanitisedAccountName, newAccount.ParentAccountId);

      var account = new Account
      {
        Id = accountId,
        Name = sanitisedAccountName,
        ParentAccountId = newAccount.ParentAccountId,
        Balance = 0
      };

      await _accountingDataAccess.AddAccount(account);

      _logger.LogInformation($"Account added: {account}");
    }

    private async Task<bool> DoesAccountExist(string accountId)
    {
      IReadOnlyDictionary<string, Account> accounts =
        await _accountingDataAccess.GetAccountsById(new[] { accountId });

      return accounts.Any();
    }

    private static string BuildAccountId(in string name, in string parentId)
    {
      string id;

      if (parentId == null)
      {
        id = name.ToLower();
      }
      else
      {
        id = $"{parentId}{AccountIdSeparator}{name}".ToLower();
      }

      return id.ToLower();
    }

    private static void ValidateNewAccount(NewAccount newAccount)
    {
      if (string.IsNullOrWhiteSpace(newAccount.Name))
      {
        throw new AccountException(
          "Account name cannot be null, empty or whitespace.",
          newAccount.ToString());
      }

      if (newAccount.Name.Contains(AccountIdSeparator))
      {
        throw new AccountException(
          $"Account name cannot contain the character '{AccountIdSeparator}'.",
          newAccount.ToString());
      }

      if (string.IsNullOrWhiteSpace(newAccount.ParentAccountId))
      {
        throw new AccountException(
          "Parent account id is cannot be null, empty or whitespace.",
          newAccount.ToString());
      }
    }
  }
}

using System;
using System.Collections.Generic;
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
    private static readonly string[] BaseAccountIds =
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
      IReadOnlyDictionary<string, Account> accountsById = await _accountingDataAccess.GetAccountsById(BaseAccountIds);

      foreach (var id in BaseAccountIds)
      {
        if (accountsById.ContainsKey(id))
        {
          _logger.LogVerbose($"Found \"{id}\" base account.");
          continue;
        }

        await _accountingDataAccess.AddAccount(
          new Account
          {
            Id = id,
            Name = id,
            Balance = 0m
          });

        _logger.LogInformation($"Added \"{id}\" base account.");
      }
    }

    public async Task AddAccount(NewAccount newAccount)
    {
      _logger.LogVerbose($"Add account request received, account details: {newAccount}");

      ValidateNewAccount(newAccount);

      var account = new Account
      {
        Id = $"{newAccount.ParentAccountId}.{newAccount.Name}",
        Name = newAccount.Name,
        Balance = 0
      };

      await _accountingDataAccess.AddAccount(account);

      _logger.LogInformation($"Account added: {account}");
    }

    private static void ValidateNewAccount(NewAccount newAccount)
    {
      if (string.IsNullOrWhiteSpace(newAccount.Name))
      {
        throw new AccountException($"Account name is invalid in {newAccount}");
      }

      if (string.IsNullOrWhiteSpace(newAccount.ParentAccountId))
      {
        throw new AccountException($"Parent account id is invalid in {newAccount}");
      }
    }
  }
}

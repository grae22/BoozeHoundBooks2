﻿using System;
using System.Threading.Tasks;

using bhb2core.Accounting.ActionResults;
using bhb2core.Accounting.Engines.AccountingEngine.Interfaces;
using bhb2core.Accounting.Interfaces;
using bhb2core.Accounting.Models;
using bhb2core.Utils.Logging;

namespace bhb2core.Accounting.Engines.AccountingEngine.SubManagers
{
  internal class AccountEngine : IAccountEngine
  {
    private const char AccountIdSeparator = '.';

    private readonly IAccountingDataAccess _accountingDataAccess;
    private readonly ILogger _logger;

    public AccountEngine(
      in IAccountingDataAccess accountingDataAccess,
      in ILogger logger)
    {
      _accountingDataAccess = accountingDataAccess ?? throw new ArgumentNullException(nameof(accountingDataAccess));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public string BuildAccountId(in string name, in string parentId)
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

    public bool ValidateNewAccount(in NewAccount newAccount, out string error)
    {
      error = null;

      if (string.IsNullOrWhiteSpace(newAccount.Name))
      {
        error = "Account name cannot be null, empty or whitespace.";
        return false;
      }

      if (newAccount.Name.Contains(AccountIdSeparator))
      {
        error = $"Account name cannot contain the character '{AccountIdSeparator}'.";
        return false;
      }

      if (newAccount.ParentAccount == null)
      {
        error = "Parent account cannot be null.";
        return false;
      }

      return true;
    }

    public async Task<AddAccountResult> AddAccount(NewAccount newAccount)
    {
      _logger.LogVerbose($"Add account request received, account details: {newAccount}");

      string sanitisedAccountName = newAccount.Name.Trim();
      string accountId = BuildAccountId(sanitisedAccountName, newAccount.ParentAccount.Id);

      var account = new Account
      {
        Id = accountId,
        Name = sanitisedAccountName,
        ParentAccountId = newAccount.ParentAccount.Id,
        Balance = 0,
        IsFunds = newAccount.ParentAccount.IsFunds,
        IsIncome = newAccount.ParentAccount.IsIncome,
        IsExpense = newAccount.ParentAccount.IsExpense,
        IsDebtor = newAccount.ParentAccount.IsDebtor,
        IsCreditor = newAccount.ParentAccount.IsCreditor
      };

      await _accountingDataAccess.AddAccount(account);

      _logger.LogInformation($"Account added: {account}");

      return AddAccountResult.CreateSuccess();
    }

    public async Task<bool> DoesAccountExist(string accountId)
    {
      GetAccountResult result = await _accountingDataAccess.GetAccountById(accountId);

      return result.IsSuccess;
    }
  }
}

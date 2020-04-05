﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using bhb2core.Accounting.ActionResults;
using bhb2core.Accounting.Engines.AccountingEngine.Interfaces;
using bhb2core.Accounting.Interfaces;
using bhb2core.Accounting.Models;
using bhb2core.Utils.Extensions;
using bhb2core.Utils.Logging;

namespace bhb2core.Accounting.Engines.AccountingEngine.SubEngines
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

    public async Task CreateBaseAccountsIfMissing()
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
        await CreateBaseAccount(
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
      }

      if (missingAccounts.Contains(IncomeAccountName))
      {
        await CreateBaseAccount(
          IncomeAccountName,
          new[]
          {
            AccountType.CreateFunds()
          },
          new AccountType[] { },
          isIncome: true);
      }

      if (missingAccounts.Contains(ExpenseAccountName))
      {
        await CreateBaseAccount(
          ExpenseAccountName,
          new AccountType[] { },
          new[]
          {
            AccountType.CreateFunds()
          },
          isExpense: true);
      }

      if (missingAccounts.Contains(DebtorAccountName))
      {
        await CreateBaseAccount(
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
      }

      if (missingAccounts.Contains(CreditorAccountName))
      {
        await CreateBaseAccount(
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
      }
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

    public async Task<AddAccountResult> AddAccount(NewAccount newAccount)
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

      await _accountingDataAccess.AddAccount(account);

      _logger.LogInformation($"Account added: {account}");

      return AddAccountResult.CreateSuccess();
    }

    public async Task<bool> DoesAccountExist(string accountQualifiedName)
    {
      GetAccountResult result = await _accountingDataAccess.GetAccount(accountQualifiedName);

      return result.IsSuccess;
    }

    private async Task CreateBaseAccount(
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

      await _accountingDataAccess.AddAccount(account);

      _logger.LogInformation($"Added \"{name}\" base account: {account}");
    }
  }
}
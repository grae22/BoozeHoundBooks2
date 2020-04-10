using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using bhb2core.Accounting.DataAccess.Interfaces;
using bhb2core.Accounting.Exceptions;
using bhb2core.Accounting.Models;
using bhb2core.Common.ActionResults;
using bhb2core.Utils.Serialisation;

namespace bhb2core.Accounting.DataAccess.DataAccessors
{
  internal class AccountDataAccess : IAccountDataAccess
  {
    private readonly List<Account> _accounts = new List<Account>();

    public async Task<GetResult<IEnumerable<Account>>> GetAllAccounts()
    {
      return await Task.FromResult(
        GetResult<IEnumerable<Account>>.CreateSuccess(_accounts));
    }

    public async Task<GetResult<IEnumerable<Account>>> GetAccounts(
      bool isFunds = false,
      bool isIncome = false,
      bool isExpense = false,
      bool isDebtor = false,
      bool isCreditor = false)
    {
      var accounts = new List<Account>();

      if (isFunds)
      {
        accounts.AddRange(_accounts.Where(a => a.AccountType.IsFunds));
      }

      if (isIncome)
      {
        accounts.AddRange(_accounts.Where(a => a.AccountType.IsIncome));
      }

      if (isExpense)
      {
        accounts.AddRange(_accounts.Where(a => a.AccountType.IsExpense));
      }

      if (isDebtor)
      {
        accounts.AddRange(_accounts.Where(a => a.AccountType.IsDebtor));
      }

      if (isCreditor)
      {
        accounts.AddRange(_accounts.Where(a => a.AccountType.IsCreditor));
      }

      return await Task.FromResult(
        GetResult<IEnumerable<Account>>.CreateSuccess(accounts));
    }

    public async Task<GetResult<Account>> GetAccount(string accountQualifiedName)
    {
      Account account =
        await Task.FromResult(
          _accounts.FirstOrDefault(a => a.QualifiedName.Equals(accountQualifiedName)));

      if (account == null)
      {
        return GetResult<Account>.CreateFailure($"Account not found \"{accountQualifiedName}\".");
      }

      return GetResult<Account>.CreateSuccess(account);
    }

    public async Task<GetResult<IReadOnlyDictionary<string, Account>>> GetAccounts(
      IEnumerable<string> accountQualifiedNames)
    {
      Dictionary<string, Account> accounts =
        _accounts
          .Where(a => accountQualifiedNames.Contains(a.QualifiedName))
          .ToDictionary(a => a.QualifiedName);

      if (accounts.Count != accountQualifiedNames.Count())
      {
        IEnumerable<string> missingAccounts = accountQualifiedNames.Where(a => !accounts.ContainsKey(a));

        return GetResult<IReadOnlyDictionary<string, Account>>.CreateFailure(
          $"Failed to find account(s): {Serialiser.Serialise(missingAccounts)}");
      }

      return await Task.FromResult(
        GetResult<IReadOnlyDictionary<string, Account>>.CreateSuccess(accounts));
    }

    public async Task<GetResult<IEnumerable<Account>>> GetParentAccountsOrdered(string accountQualifiedName)
    {
      Account account =
        _accounts.FirstOrDefault(a => a.QualifiedName.Equals(accountQualifiedName, StringComparison.Ordinal));

      var parentAccounts = new List<Account>();

      try
      {
        await GetAccountParentsRecursive(account, parentAccounts);
      }
      catch (AccountException ex)
      {
        GetResult<IEnumerable<Account>>.CreateFailure(ex.Message);
      }

      return GetResult<IEnumerable<Account>>.CreateSuccess(parentAccounts);
    }

    public async Task<bool> IsParentAccount(string accountQualifiedName)
    {
      bool hasChildren = _accounts
        .Any(a => a.ParentAccountQualifiedName?.Equals(
          accountQualifiedName,
          StringComparison.Ordinal) ?? false);

      return await Task.FromResult(hasChildren);
    }

    public async Task<ActionResult> AddAccount(Account account)
    {
      bool accountAlreadyExists =
        _accounts
          .Exists(a =>
            a.QualifiedName.Equals(
              account.QualifiedName,
              StringComparison.OrdinalIgnoreCase));

      if (accountAlreadyExists)
      {
        return ActionResult.CreateFailure($"Account already exists \"{account.QualifiedName}\".");
      }

      _accounts.Add(account);

      return await Task.FromResult(
        ActionResult.CreateSuccess());
    }

    public async Task<ActionResult> UpdateAccountBalances(
      IReadOnlyDictionary<string, decimal> balancesByAccountQualifiedName)
    {
      foreach (var accountNameAndBalance in balancesByAccountQualifiedName)
      {
        string accountQualifiedName = accountNameAndBalance.Key;
        decimal balance = accountNameAndBalance.Value;

        _accounts
          .Single(a => a.QualifiedName.Equals(accountQualifiedName))
          .Balance = balance;
      }

      return await Task.FromResult(
        ActionResult.CreateSuccess());
    }

    private async Task GetAccountParentsRecursive(
      Account account,
      ICollection<Account> parentAccounts)
    {
      if (!account.HasParent)
      {
        return;
      }

      Account parentAccount = _accounts
        .FirstOrDefault(a => a.QualifiedName.Equals(account.ParentAccountQualifiedName, StringComparison.Ordinal));

      if (parentAccount == null)
      {
        throw new AccountException($"Account not found: \"{account.ParentAccountQualifiedName}\".", string.Empty);
      }

      parentAccounts.Add(parentAccount);

      await GetAccountParentsRecursive(parentAccount, parentAccounts);
    }
  }
}

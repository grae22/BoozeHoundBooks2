using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using bhb2core.Accounting.DataAccess.ActionResults;
using bhb2core.Accounting.Exceptions;
using bhb2core.Accounting.Interfaces;
using bhb2core.Accounting.Models;

namespace bhb2core.Accounting.DataAccess
{
  internal class AccountingDataAccess : IAccountingDataAccess
  {
    private readonly List<Account> _accounts = new List<Account>();

    public async Task<IEnumerable<Account>> GetAllAccounts()
    {
      return await Task.FromResult(_accounts);
    }

    public async Task<IEnumerable<Account>> GetAccounts(
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

      return await Task.FromResult(accounts);
    }

    public async Task<GetAccountResult> GetAccount(string accountQualifiedName)
    {
      Account account =
        await Task.FromResult(
          _accounts.FirstOrDefault(a => a.QualifiedName.Equals(accountQualifiedName)));

      if (account == null)
      {
        return GetAccountResult.CreateFailure($"Account not found \"{accountQualifiedName}\".");
      }

      return GetAccountResult.CreateSuccess(account);
    }

    public async Task<IReadOnlyDictionary<string, Account>> GetAccounts(
      IEnumerable<string> accountQualifiedNames)
    {
      Dictionary<string, Account> accounts =
        _accounts
          .Where(a => accountQualifiedNames.Contains(a.QualifiedName))
          .ToDictionary(a => a.QualifiedName);

      return await Task.FromResult(accounts);
    }

    public async Task AddAccount(Account account)
    {
      bool accountAlreadyExists =
        _accounts
          .Exists(a =>
            a.QualifiedName.Equals(
              account.QualifiedName,
              StringComparison.OrdinalIgnoreCase));

      if (accountAlreadyExists)
      {
        throw new AccountException(
          $"Account already exists \"{account.QualifiedName}\".",
          account.ToString());
      }

      _accounts.Add(account);

      await Task.Delay(0);
    }

    public async Task UpdateAccountBalances(
      IReadOnlyDictionary<string, decimal> balancesByAccountQualifiedName)
    {
      foreach (var accountNameAndBalance in balancesByAccountQualifiedName)
      {
        _accounts
          .Single(a => a.QualifiedName.Equals(accountNameAndBalance.Key))
          .Balance = accountNameAndBalance.Value;
      }

      await Task.Delay(0);
    }
  }
}
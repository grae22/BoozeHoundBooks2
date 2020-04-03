﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using bhb2core.Accounting.ActionResults;
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
        accounts.AddRange(_accounts.Where(a => a.IsFunds));
      }

      if (isIncome)
      {
        accounts.AddRange(_accounts.Where(a => a.IsIncome));
      }

      if (isExpense)
      {
        accounts.AddRange(_accounts.Where(a => a.IsExpense));
      }

      if (isDebtor)
      {
        accounts.AddRange(_accounts.Where(a => a.IsDebtor));
      }

      if (isCreditor)
      {
        accounts.AddRange(_accounts.Where(a => a.IsCreditor));
      }

      return await Task.FromResult(accounts);
    }

    public async Task<GetAccountResult> GetAccountById(string accountId)
    {
      Account account =
        await Task.FromResult(
          _accounts.FirstOrDefault(a => a.Id.Equals(accountId)));

      if (account == null)
      {
        return GetAccountResult.CreateFailure($"Account not found with id \"{accountId}\".");
      }

      return GetAccountResult.CreateSuccess(account);
    }

    public async Task<IReadOnlyDictionary<string, Account>> GetAccountsById(IEnumerable<string> accountIds)
    {
      Dictionary<string, Account> accounts =
        _accounts
          .Where(a => accountIds.Contains(a.Id))
          .ToDictionary(a => a.Id);

      return await Task.FromResult(accounts);
    }

    public async Task AddAccount(Account account)
    {
      bool accountAlreadyExists =
        _accounts
          .Exists(a =>
            a.Id.Equals(
              account.Id,
              StringComparison.OrdinalIgnoreCase));

      if (accountAlreadyExists)
      {
        throw new AccountException(
          $"Account already exists with id \"{account.Id}\".",
          account.ToString());
      }

      _accounts.Add(account);

      await Task.Delay(0);
    }

    public async Task UpdateAccountBalances(IReadOnlyDictionary<string, decimal> balancesByAccountId)
    {
      foreach (var accountIdAndBalance in balancesByAccountId)
      {
        _accounts
          .Single(a => a.Id.Equals(accountIdAndBalance.Key))
          .Balance = accountIdAndBalance.Value;
      }

      await Task.Delay(0);
    }
  }
}
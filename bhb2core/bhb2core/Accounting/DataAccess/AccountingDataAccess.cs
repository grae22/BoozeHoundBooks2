using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
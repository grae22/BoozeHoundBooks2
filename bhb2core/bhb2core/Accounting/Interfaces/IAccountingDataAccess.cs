using System.Collections.Generic;
using System.Threading.Tasks;

using bhb2core.Accounting.Models;

namespace bhb2core.Accounting.Interfaces
{
  internal interface IAccountingDataAccess
  {
    Task<IReadOnlyDictionary<string, Account>> GetAccountsById(IEnumerable<string> accountIds);
    Task UpdateAccountBalances(IReadOnlyDictionary<string, decimal> balancesByAccountId);
  }
}

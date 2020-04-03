using System.Collections.Generic;
using System.Threading.Tasks;

using bhb2core.Accounting.Models;

namespace bhb2core.Accounting.Interfaces
{
  internal interface IAccountingDataAccess
  {
    Task<IEnumerable<Account>> GetAllAccounts();

    Task<IEnumerable<Account>> GetAccounts(
      bool isFunds = false,
      bool isIncome = false,
      bool isExpense = false,
      bool isDebtor = false,
      bool isCreditor = false);

    Task<IReadOnlyDictionary<string, Account>> GetAccountsById(IEnumerable<string> accountIds);

    Task AddAccount(Account account);

    Task UpdateAccountBalances(IReadOnlyDictionary<string, decimal> balancesByAccountId);
  }
}

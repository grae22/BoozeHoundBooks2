using System.Collections.Generic;
using System.Threading.Tasks;

using bhb2core.Accounting.DataAccess.ActionResults;
using bhb2core.Accounting.Models;

namespace bhb2core.Accounting.Interfaces
{
  internal interface IAccountingDataAccess
  {
    Task<IEnumerable<Account>> GetAllAccounts();

    Task<GetResult<Account>> GetAccount(string accountQualifiedName);

    Task<IEnumerable<Account>> GetAccounts(
      bool isFunds = false,
      bool isIncome = false,
      bool isExpense = false,
      bool isDebtor = false,
      bool isCreditor = false);

    Task<IReadOnlyDictionary<string, Account>> GetAccounts(
      IEnumerable<string> accountQualifiedNames);

    Task<GetResult<IEnumerable<Account>>> GetParentAccountsOrdered(string accountQualifiedName);

    Task<bool> IsParentAccount(string accountQualifiedName);

    Task AddAccount(Account account);

    Task UpdateAccountBalances(
      IReadOnlyDictionary<string, decimal> balancesByAccountQualifiedName);
  }
}

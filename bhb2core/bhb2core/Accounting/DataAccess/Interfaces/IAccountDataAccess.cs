using System.Collections.Generic;
using System.Threading.Tasks;

using bhb2core.Accounting.Models;
using bhb2core.Common.ActionResults;

namespace bhb2core.Accounting.DataAccess.Interfaces
{
  internal interface IAccountDataAccess
  {
    Task<GetResult<IEnumerable<Account>>> GetAllAccounts();

    Task<GetResult<Account>> GetAccount(string accountQualifiedName);

    Task<GetResult<IEnumerable<Account>>> GetAccounts(
      bool isFunds = false,
      bool isIncome = false,
      bool isExpense = false,
      bool isDebtor = false,
      bool isCreditor = false);

    Task<GetResult<IReadOnlyDictionary<string, Account>>> GetAccounts(
      IEnumerable<string> accountQualifiedNames);

    Task<GetResult<IEnumerable<Account>>> GetParentAccountsOrdered(string accountQualifiedName);

    Task<bool> IsParentAccount(string accountQualifiedName);

    Task<ActionResult> AddAccount(Account account);

    Task<ActionResult> UpdateAccountBalances(
      IReadOnlyDictionary<string, decimal> balancesByAccountQualifiedName);
  }
}

using System.Collections.Generic;
using System.Threading.Tasks;

using bhb2core.Accounting.Models;

namespace bhb2core.Accounting.Interfaces
{
  internal interface IAccountingEngine
  {
    Task CreateBaseAccountsIfMissing();
    Task<IEnumerable<Account>> GetAllAccounts();
    Task ProcessTransaction(Transaction transaction);
  }
}

using System.Threading.Tasks;

using bhb2core.Accounting.Models;

namespace bhb2core.Accounting.Engines.AccountingEngine.Interfaces
{
  internal interface IAccountEngine
  {
    Task CreateBaseAccountsIfMissing();

    Task AddAccount(NewAccount newAccount);
  }
}

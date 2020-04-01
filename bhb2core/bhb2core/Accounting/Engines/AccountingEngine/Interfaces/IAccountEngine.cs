using System.Threading.Tasks;

namespace bhb2core.Accounting.Engines.AccountingEngine.Interfaces
{
  internal interface IAccountEngine
  {
    Task CreateBaseAccountsIfMissing();
  }
}

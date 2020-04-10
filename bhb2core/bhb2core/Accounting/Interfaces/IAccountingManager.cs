using System.Threading.Tasks;

using bhb2core.Accounting.Managers.Interfaces;

namespace bhb2core.Accounting.Interfaces
{
  public interface IAccountingManager :
    IAccountManager,
    ITransactionManager
  {
    new Task<bool> Initialise();
  }
}

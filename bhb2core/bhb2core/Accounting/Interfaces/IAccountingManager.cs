using System.Threading.Tasks;

using bhb2core.Accounting.Managers.Interfaces;
using bhb2core.Common.ActionResults;

namespace bhb2core.Accounting.Interfaces
{
  public interface IAccountingManager :
    IAccountManager,
    ITransactionManager,
    IPeriodManager
  {
    new Task<ActionResult> Initialise();
  }
}

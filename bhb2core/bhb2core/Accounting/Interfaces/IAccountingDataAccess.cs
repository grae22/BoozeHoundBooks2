using System.Threading.Tasks;

using bhb2core.Accounting.DataAccess.Interfaces;
using bhb2core.Common.ActionResults;

namespace bhb2core.Accounting.Interfaces
{
  internal interface IAccountingDataAccess :
    IAccountDataAccess,
    ITransactionDataAccess
  {
    public Task<ActionResult> Initialise();
  }
}

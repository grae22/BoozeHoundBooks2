using System.Threading.Tasks;

using bhb2core.Accounting.Models;
using bhb2core.Common.ActionResults;

namespace bhb2core.Accounting.DataAccess.Interfaces
{
  internal interface ITransactionDataAccess
  {
    Task<ActionResult> AddTransaction(Transaction transaction);
  }
}

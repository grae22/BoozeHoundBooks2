using System.Threading.Tasks;

using bhb2core.Accounting.Models;
using bhb2core.Common.ActionResults;

namespace bhb2core.Accounting.Engines.Interfaces
{
  internal interface ITransactionEngine
  {
    Task<ActionResult> AddTransaction(Transaction transaction);
  }
}

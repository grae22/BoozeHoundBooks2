using System.Threading.Tasks;

using bhb2core.Accounting.Models;

namespace bhb2core.Accounting.Interfaces
{
  internal interface ITransactionEngine
  {
    public Task ProcessTransaction(Transaction transaction);
  }
}

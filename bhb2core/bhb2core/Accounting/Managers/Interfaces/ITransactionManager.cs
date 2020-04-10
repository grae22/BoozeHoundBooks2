using System.Threading.Tasks;

using bhb2core.Accounting.Dto;
using bhb2core.Accounting.Managers.ActionResults;

namespace bhb2core.Accounting.Managers.Interfaces
{
  public interface ITransactionManager
  {
    Task<ProcessTransactionResult> ProcessTransaction(TransactionDto transactionDto);
  }
}

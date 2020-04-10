using System.Collections.Generic;
using System.Threading.Tasks;

using bhb2core.Accounting.Dto;
using bhb2core.Accounting.Managers.ActionResults;
using bhb2core.Common.ActionResults;

namespace bhb2core.Accounting.Managers.Interfaces
{
  public interface ITransactionManager
  {
    Task<ProcessTransactionResult> ProcessTransaction(TransactionDto transactionDto);

    Task<GetResult<IEnumerable<TransactionDto>>> GetTransactions();
  }
}

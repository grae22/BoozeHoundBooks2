using System.Threading.Tasks;

using bhb2core.Accounting.Dto;

namespace bhb2core.Accounting.Managers.AccountingManager.Interfaces
{
  internal interface ITransactionManager
  {
    Task ProcessTransaction(TransactionDto transactionDto);
  }
}

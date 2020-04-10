using System;
using System.Threading.Tasks;

using bhb2core.Accounting.DataAccess.Interfaces;
using bhb2core.Accounting.Models;
using bhb2core.Common.ActionResults;

namespace bhb2core.Accounting.DataAccess.DataAccessors
{
  internal class TransactionDataAccess : ITransactionDataAccess
  {
    public Task<ActionResult> AddTransaction(Transaction transaction)
    {
      throw new System.NotImplementedException();
    }

    public Task<bool> DoesTransactionExist(Guid idempotencyId)
    {
      throw new NotImplementedException();
    }
  }
}

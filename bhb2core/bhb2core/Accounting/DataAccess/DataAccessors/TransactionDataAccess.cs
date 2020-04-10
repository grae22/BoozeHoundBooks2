using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using bhb2core.Accounting.DataAccess.Interfaces;
using bhb2core.Accounting.Models;
using bhb2core.Common.ActionResults;

namespace bhb2core.Accounting.DataAccess.DataAccessors
{
  internal class TransactionDataAccess : ITransactionDataAccess
  {
    private readonly Dictionary<Guid, Transaction> _transactions = new Dictionary<Guid, Transaction>();

    public async Task<ActionResult> AddTransaction(Transaction transaction)
    {
      if (_transactions.TryAdd(transaction.IdempotencyId, transaction))
      {
        return await Task.FromResult(
          ActionResult.CreateSuccess());
      }

      return ActionResult.CreateFailure("Failed to add transaction to transaction log.");
    }

    public async Task<bool> DoesTransactionExist(Guid idempotencyId)
    {
      return await Task.FromResult(
        _transactions.ContainsKey(idempotencyId));
    }
  }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using bhb2core.Accounting.DataAccess.Interfaces;
using bhb2core.Accounting.Models;
using bhb2core.Common.ActionResults;
using bhb2core.Utils.Persistence;
using bhb2core.Utils.Serialisation;

namespace bhb2core.Accounting.DataAccess.DataAccessors
{
  internal class TransactionDataAccess : ITransactionDataAccess, IPersistable
  {
    public string PersistenceId => "Transactions";

    private const string SerialisedDataAccountsKey = "transactions";

    private readonly IPersistor _persistor;
    private Dictionary<Guid, Transaction> _transactions = new Dictionary<Guid, Transaction>();

    public TransactionDataAccess(in IPersistor persistor)
    {
      _persistor = persistor ?? throw new ArgumentNullException(nameof(persistor));
    }

    public async Task<ActionResult> AddTransaction(Transaction transaction)
    {
      if (_transactions.TryAdd(transaction.IdempotencyId, transaction))
      {
        return await _persistor.Persist();
      }

      return ActionResult.CreateFailure("Failed to add transaction to transaction log.");
    }

    public async Task<bool> DoesTransactionExist(Guid idempotencyId)
    {
      return await Task.FromResult(
        _transactions.ContainsKey(idempotencyId));
    }

    public string Serialise()
    {
      var serialisedDataByKey = new Dictionary<string, string>
      {
        { SerialisedDataAccountsKey, Serialiser.Serialise(_transactions) }
      };

      return Serialiser.Serialise(serialisedDataByKey);
    }

    public void Deserialise(in string serialisedData)
    {
      var serialisedDataByKey = Serialiser.Deserialise<IReadOnlyDictionary<string, string>>(serialisedData);

      _transactions.Clear();

      _transactions =
        Serialiser.Deserialise<Dictionary<Guid, Transaction>>(
          serialisedDataByKey[SerialisedDataAccountsKey]);
    }
  }
}

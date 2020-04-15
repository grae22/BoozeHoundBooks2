using System;
using System.Threading.Tasks;

using bhb2core.Accounting.Engines.Interfaces;
using bhb2core.Accounting.Interfaces;
using bhb2core.Accounting.Models;
using bhb2core.Common.ActionResults;
using bhb2core.Utils.Logging;

namespace bhb2core.Accounting.Engines.SubEngines
{
  internal class TransactionEngine : ITransactionEngine
  {
    private readonly IAccountingDataAccess _accountingDataAccess;
    private readonly ILogger _logger;

    public TransactionEngine(
      in IAccountingDataAccess accountingDataAccess,
      in ILogger logger)
    {
      _accountingDataAccess = accountingDataAccess ?? throw new ArgumentNullException(nameof(accountingDataAccess));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ActionResult> AddTransaction(Transaction transaction)
    {
      _logger.LogVerbose($"Received transaction: {transaction}");

      if (await _accountingDataAccess.DoesTransactionExist(transaction.IdempotencyId))
      {
        _logger.LogError($"Transaction already exists with idempotency-id: {transaction.IdempotencyId}");

        return ActionResult.CreateFailure("Transaction already exists.");
      }

      GetResult<Period> getPeriodResult = await _accountingDataAccess.GetPeriodForDate(transaction.Date);

      if (!getPeriodResult.IsSuccess)
      {
        string message = $"Cannot add transaction due to period error: \"{getPeriodResult.FailureMessage}\".";

        _logger.LogError(message);

        return ActionResult.CreateFailure(message);
      }

      return await _accountingDataAccess.AddTransaction(transaction);
    }
  }
}

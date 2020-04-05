using System;

using bhb2core.Accounting.Engines.AccountingEngine.Interfaces;
using bhb2core.Accounting.Interfaces;
using bhb2core.Utils.Logging;

namespace bhb2core.Accounting.Engines.AccountingEngine.SubEngines
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
  }
}

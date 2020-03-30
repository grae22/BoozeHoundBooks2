using bhb2core;
using bhb2core.Accounting.Engines;
using bhb2core.Accounting.Interfaces;
using bhb2core.Accounting.Managers;
using bhb2core.Utils.Logging;
using bhb2core.Utils.Mapping;

namespace bhb2coreTests.Accounting.TestUtils
{
  internal static class AccountingManagerFactory
  {
    public static AccountingManager Create(out IAccountingDataAccess accountingDataAccess)
    {
      Bhb2Core.Initialise(
        out ILogger logger,
        out IMapper mapper,
        out IAccountingManager ignoredAccountingManager);

      accountingDataAccess = new MockAccountingDataAccess();

      var transactionEngine = new AccountingEngine(accountingDataAccess, logger);

      var accountingManager = new AccountingManager(
        transactionEngine,
        mapper,
        logger);

      accountingManager
        .Initialise()
        .GetAwaiter()
        .GetResult();

      return accountingManager;
    }
  }
}

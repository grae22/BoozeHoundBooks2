using bhb2core;
using bhb2core.Accounting.Engines.AccountingEngine;
using bhb2core.Accounting.Interfaces;
using bhb2core.Accounting.Managers.AccountingManager;
using bhb2core.Utils.Logging;
using bhb2core.Utils.Mapping;

using NSubstitute;

namespace bhb2coreTests.Accounting.TestUtils
{
  internal static class AccountingManagerFactory
  {
    public static AccountingManager Create(
      out IAccountingDataAccess accountingDataAccess,
      in bool performInitialisation = true,
      in bool useConcreteDataAccessMock = false)
    {
      Bhb2Core.Initialise(
        out ILogger logger,
        out IMapper mapper,
        out IAccountingManager ignoredAccountingManager);

      accountingDataAccess = useConcreteDataAccessMock
        ? new MockAccountingDataAccess()
        : Substitute.For<IAccountingDataAccess>();

      var transactionEngine = new AccountingEngine(accountingDataAccess, logger);

      var accountingManager = new AccountingManager(
        transactionEngine,
        accountingDataAccess,
        mapper,
        logger);

      if (performInitialisation)
      {
        accountingManager
          .Initialise()
          .GetAwaiter()
          .GetResult();
      }

      return accountingManager;
    }
  }
}

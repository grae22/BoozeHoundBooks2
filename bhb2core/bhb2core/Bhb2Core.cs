using bhb2core.Accounting.DataAccess;
using bhb2core.Accounting.Engines;
using bhb2core.Accounting.Interfaces;
using bhb2core.Accounting.Managers.AccountingManager;
using bhb2core.Utils.Logging;
using bhb2core.Utils.Mapping;

namespace bhb2core
{
  public static class Bhb2Core
  {
    public static void Initialise(
      out ILogger logger,
      out IMapper mapper,
      out IAccountingManager accountingManager)
    {
      logger = new ConsoleLogger();
      mapper = Mapper.CreateAndInitialiseMappings(logger);

      var accountingDataAccess = new AccountingDataAccess();
      var accountingEngine = new AccountingEngine(accountingDataAccess, logger);

      accountingManager = new AccountingManager(
        accountingEngine,
        accountingDataAccess,
        mapper,
        logger);

      ((AccountingManager)accountingManager)
        .Initialise()
        .GetAwaiter()
        .GetResult();
    }
  }
}

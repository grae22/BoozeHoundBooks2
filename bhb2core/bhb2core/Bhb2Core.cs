using System;
using System.Threading.Tasks;

using bhb2core.Accounting.DataAccess;
using bhb2core.Accounting.Engines;
using bhb2core.Accounting.Interfaces;
using bhb2core.Accounting.Managers;
using bhb2core.Common.ActionResults;
using bhb2core.Utils.Configuration;
using bhb2core.Utils.Logging;
using bhb2core.Utils.Mapping;
using bhb2core.Utils.Persistence;

namespace bhb2core
{
  public static class Bhb2Core
  {
    public static ILogger Logger { get; private set; }
    public static IAccountingManager AccountingManager { get; private set; }

    private const string ConfigurationKey_DataFilename = "dataFilename";

    public static async Task<ActionResult> Initialise(IConfiguration configuration)
    {
      try
      {
        Logger = new ConsoleLogger();

        IMapper mapper = Mapper.CreateAndInitialiseMappings(Logger);

        ActionResult createPersistorResult = CreatePersistor(configuration, out IPersistor persistor);

        if (!createPersistorResult.IsSuccess)
        {
          return createPersistorResult;
        }

        var accountingDataAccess = new AccountingDataAccess(persistor, Logger);
        var accountingEngine = new AccountingEngine(accountingDataAccess, Logger);

        AccountingManager = new AccountingManager(
          accountingEngine,
          accountingDataAccess,
          mapper,
          Logger);

        return await AccountingManager.Initialise();
      }
      catch (Exception ex)
      {
        var message = $"Unhandled exception: \"{ex.Message}\".";

        Logger?.LogError(message, ex);

        return ActionResult.CreateFailure(message);
      }
    }

    private static ActionResult CreatePersistor(
      in IConfiguration configuration,
      out IPersistor persistor)
    {
      persistor = null;

      string dataFilename;

      try
      {
        dataFilename = configuration.GetValue(ConfigurationKey_DataFilename);
      }
      catch (MissingConfigurationException ex)
      {
        return ActionResult.CreateFailure(ex.Message);
      }

      persistor = new FileBasedPersistor(dataFilename, Logger);

      return ActionResult.CreateSuccess();
    }
  }
}

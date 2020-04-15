using System;
using System.Threading.Tasks;

using bhb2core.Accounting.Engines.Interfaces;
using bhb2core.Accounting.Interfaces;
using bhb2core.Accounting.Models;
using bhb2core.Common.ActionResults;
using bhb2core.Utils.Logging;

namespace bhb2core.Accounting.Engines.SubEngines
{
  internal class PeriodEngine : IPeriodEngine
  {
    private readonly IAccountingDataAccess _accountingDataAccess;
    private readonly ILogger _logger;

    public PeriodEngine(
      in IAccountingDataAccess accountingDataAccess,
      in ILogger logger)
    {
      _accountingDataAccess = accountingDataAccess ?? throw new ArgumentNullException(nameof(accountingDataAccess));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public bool ValidatePeriod(in Period period, out string message)
    {
      if (period == null)
      {
        throw new ArgumentNullException(nameof(period));
      }

      message = null;

      if (period.Start > period.End)
      {
        message = "Period start cannot be before period end.";
      }

      return message == null;
    }

    public async Task<ActionResult> AddPeriod(Period period)
    {
      _logger.LogVerbose($"Attempting to add period: {period}");

      GetResult<Period> getLastPeriodResult = await _accountingDataAccess.GetLastPeriod();

      if (!getLastPeriodResult.IsSuccess)
      {
        _logger.LogInformation("No periods found, adding period as the first period.");

        return await _accountingDataAccess.AddPeriod(period);
      }

      Period lastPeriod = getLastPeriodResult.Result;

      DateTime expectedNewPeriodStartDate = lastPeriod.End.AddDays(1);

      if (period.Start != expectedNewPeriodStartDate)
      {
        var message =
          $"New period must start the day after the end of the last period, i.e. {expectedNewPeriodStartDate:yyyy-MM-dd}. ";

        _logger.LogError(message);

        return ActionResult.CreateFailure(message);
      }

      return await _accountingDataAccess.AddPeriod(period);
    }
  }
}

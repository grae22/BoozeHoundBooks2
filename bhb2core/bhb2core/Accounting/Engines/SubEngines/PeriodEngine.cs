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

    public async Task<ActionResult> CreateCurrentPeriodIfNoneExist()
    {
      GetResult<Period> getLastPeriodResult = await _accountingDataAccess.GetLastPeriod();

      if (getLastPeriodResult.IsSuccess)
      {
        _logger.LogInformation("Periods found, nothing to do.");

        return ActionResult.CreateSuccess();
      }

      _logger.LogInformation("No periods found, creating current period...");

      DateTime now = DateTime.Now;

      var period = new Period(
        new DateTime( 
          now.Year,
          now.Month,
          1),
        new DateTime(
          now.Year,
          now.Month,
          DateTime.DaysInMonth(now.Year, now.Month)));

      return await _accountingDataAccess.AddPeriod(period);
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
      _logger.LogInformation($"Attempting to add period: {period}");

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

    public async Task<ActionResult> UpdatePeriodEndDate(UpdatePeriodEndDate updatePeriodEndDate)
    {
      _logger.LogInformation($"Attempting to update period end date: {updatePeriodEndDate}");

      GetResult<Period> getLastPeriodResult = await _accountingDataAccess.GetLastPeriod();

      if (!getLastPeriodResult.IsSuccess)
      {
        string message = $"Update failed, couldn't retrieve last period: \"{getLastPeriodResult.FailureMessage}\".";

        _logger.LogError(message);

        return ActionResult.CreateFailure(message);
      }

      Period lastPeriod = getLastPeriodResult.Result;

      if (lastPeriod.Start > updatePeriodEndDate.DateInPeriod ||
          lastPeriod.End < updatePeriodEndDate.DateInPeriod)
      {
        var message = "Update failed, only the last period's end-date can be changed.";

        _logger.LogError(message);

        return ActionResult.CreateFailure(message);
      }

      if (lastPeriod.Start > updatePeriodEndDate.NewEnd)
      {
        var message = "Period end-date cannot be before its start date.";

        _logger.LogError(message);

        return ActionResult.CreateFailure(message);
      }

      return await _accountingDataAccess.UpdateLastPeriodEndDate(updatePeriodEndDate.NewEnd);
    }
  }
}

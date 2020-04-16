using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using bhb2core.Accounting.Dto;
using bhb2core.Accounting.Interfaces;
using bhb2core.Accounting.Managers.Interfaces;
using bhb2core.Accounting.Models;
using bhb2core.Common.ActionResults;
using bhb2core.Utils.Logging;
using bhb2core.Utils.Mapping;
using bhb2core.Utils.Serialisation;

namespace bhb2core.Accounting.Managers.SubManagers
{
  internal class PeriodManager : IPeriodManager
  {
    private readonly IAccountingDataAccess _accountingDataAccess;
    private readonly IAccountingEngine _accountingEngine;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public PeriodManager(
      in IAccountingDataAccess accountingDataAccess,
      in IAccountingEngine accountingEngine,
      in IMapper mapper,
      in ILogger logger)
    {
      _accountingDataAccess = accountingDataAccess ?? throw new ArgumentNullException(nameof(accountingDataAccess));
      _accountingEngine = accountingEngine ?? throw new ArgumentNullException(nameof(accountingEngine));
      _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ActionResult> Initialise()
    {
      _logger.LogInformation("Initialising...");

      return await _accountingEngine.CreateCurrentPeriodIfNoneExist();
    }

    public async Task<ActionResult> AddPeriod(PeriodDto periodDto)
    {
      _logger.LogInformation($"Call received for period: {periodDto}");

      Period period = _mapper.Map<PeriodDto, Period>(periodDto);

      if (!_accountingEngine.ValidatePeriod(period, out string validatePeriodMessage))
      {
        var message = $"Period validation failed: \"{validatePeriodMessage}\".";

        _logger.LogError(message);

        return ActionResult.CreateFailure(message);
      }

      return await _accountingEngine.AddPeriod(period);
    }

    public async Task<GetResult<IEnumerable<PeriodDto>>> GetAllPeriods()
    {
      _logger.LogInformation("Call received.");

      GetResult<IEnumerable<Period>> getAllPeriodsResult = await _accountingDataAccess.GetAllPeriods();

      if (!getAllPeriodsResult.IsSuccess)
      {
        var message = $"Failed to retrieve all periods: \"{getAllPeriodsResult.FailureMessage}\".";

        _logger.LogError(message);

        return GetResult<IEnumerable<PeriodDto>>.CreateFailure(message);
      }

      IEnumerable<PeriodDto> periods =
        _mapper.Map<IEnumerable<Period>, IEnumerable<PeriodDto>>(getAllPeriodsResult.Result);

      _logger.LogVerbose($"Returning periods: {Serialiser.Serialise(periods)}");

      return GetResult<IEnumerable<PeriodDto>>.CreateSuccess(periods);
    }

    public async Task<ActionResult> UpdateLastPeriodEndDate(UpdateLastPeriodEndDateDto updatePeriodEndDateDto)
    {
      _logger.LogInformation($"Call received with: {updatePeriodEndDateDto}");

      UpdateLastPeriodEndDate updatePeriodEndDate =
        _mapper.Map<UpdateLastPeriodEndDateDto, UpdateLastPeriodEndDate>(updatePeriodEndDateDto);

      return await _accountingEngine.UpdateLastPeriodEndDate(updatePeriodEndDate);
    }
  }
}

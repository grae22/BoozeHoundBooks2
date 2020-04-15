using System;
using System.Threading.Tasks;

using bhb2core.Accounting.Dto;
using bhb2core.Accounting.Interfaces;
using bhb2core.Accounting.Managers.Interfaces;
using bhb2core.Accounting.Models;
using bhb2core.Common.ActionResults;
using bhb2core.Utils.Logging;
using bhb2core.Utils.Mapping;

namespace bhb2core.Accounting.Managers.SubManagers
{
  internal class PeriodManager : IPeriodManager
  {
    private readonly IAccountingEngine _accountingEngine;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public PeriodManager(
      in IAccountingEngine accountingEngine,
      in IMapper mapper,
      in ILogger logger)
    {
      _accountingEngine = accountingEngine ?? throw new ArgumentNullException(nameof(accountingEngine));
      _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
  }
}

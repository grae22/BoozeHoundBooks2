﻿using System.Threading.Tasks;

using bhb2core.Accounting.Dto;
using bhb2core.Common.ActionResults;

namespace bhb2core.Accounting.Managers.Interfaces
{
  public interface IPeriodManager
  {
    Task<ActionResult> AddPeriod(PeriodDto period);

    Task<ActionResult> UpdatePeriodEndDate(UpdatePeriodEndDateDto updatePeriodEndDate);
  }
}

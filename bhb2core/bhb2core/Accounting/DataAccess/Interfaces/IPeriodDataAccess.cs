using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using bhb2core.Accounting.Models;
using bhb2core.Common.ActionResults;

namespace bhb2core.Accounting.DataAccess.Interfaces
{
  internal interface IPeriodDataAccess
  {
    Task<ActionResult> AddPeriod(Period period);

    Task<GetResult<IEnumerable<Period>>> GetAllPeriods();

    Task<GetResult<Period>> GetLastPeriod();

    Task<GetResult<Period>> GetPeriodForDate(DateTime date);

    Task<ActionResult> UpdateLastPeriodEndDate(DateTime newEnd);
  }
}

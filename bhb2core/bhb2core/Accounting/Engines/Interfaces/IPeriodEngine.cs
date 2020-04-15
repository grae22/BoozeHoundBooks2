using System.Threading.Tasks;

using bhb2core.Accounting.Models;
using bhb2core.Common.ActionResults;

namespace bhb2core.Accounting.Engines.Interfaces
{
  internal interface IPeriodEngine
  {
    bool ValidatePeriod(
      in Period period,
      out string message);

    Task<ActionResult> AddPeriod(Period period);
  }
}

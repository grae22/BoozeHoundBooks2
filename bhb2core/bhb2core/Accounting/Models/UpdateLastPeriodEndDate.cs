using System;

using bhb2core.Utils.Serialisation;

namespace bhb2core.Accounting.Models
{
  public class UpdateLastPeriodEndDate : ToStringSerialiser
  {
    public DateTime NewEnd { get; }

    public UpdateLastPeriodEndDate(DateTime newEnd)
    {
      NewEnd = newEnd.Date;
    }
  }
}

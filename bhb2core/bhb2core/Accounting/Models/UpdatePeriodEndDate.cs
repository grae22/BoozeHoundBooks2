using System;

using bhb2core.Utils.Serialisation;

namespace bhb2core.Accounting.Models
{
  public class UpdatePeriodEndDate : ToStringSerialiser
  {
    public DateTime DateInPeriod { get; }
    public DateTime NewEnd { get; }

    public UpdatePeriodEndDate(
      DateTime dateInPeriod,
      DateTime newEnd)
    {
      DateInPeriod = dateInPeriod.Date;
      NewEnd = newEnd.Date;
    }
  }
}

using System;

using bhb2core.Utils.Serialisation;

namespace bhb2core.Accounting.Dto
{
  public class UpdatePeriodEndDateDto : ToStringSerialiser
  {
    public DateTime DateInPeriod { get; }
    public DateTime NewEnd { get; }

    public UpdatePeriodEndDateDto(
      DateTime dateInPeriod,
      DateTime newEnd)
    {
      DateInPeriod = dateInPeriod.Date;
      NewEnd = newEnd.Date;
    }
  }
}

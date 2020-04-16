using System;

using bhb2core.Utils.Serialisation;

namespace bhb2core.Accounting.Dto
{
  public class UpdateLastPeriodEndDateDto : ToStringSerialiser
  {
    public DateTime NewEnd { get; }

    public UpdateLastPeriodEndDateDto(DateTime newEnd)
    {
      NewEnd = newEnd.Date;
    }
  }
}

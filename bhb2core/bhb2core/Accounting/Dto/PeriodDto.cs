using System;

using bhb2core.Utils.Serialisation;

namespace bhb2core.Accounting.Dto
{
  public class PeriodDto : ToStringSerialiser
  {
    public DateTime Start { get; }
    public DateTime End { get; }

    public PeriodDto(
      DateTime start,
      DateTime end)
    {
      Start = start.Date;
      End = end.Date;
    }
  }
}

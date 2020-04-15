using System;

namespace bhb2core.Accounting.Models
{
  internal class Period
  {
    public DateTime Start { get; }
    public DateTime End { get; }

    public Period(
      DateTime start,
      DateTime end)
    {
      Start = start.Date;
      End = end.Date;
    }
  }
}

using System;
using System.Collections.Generic;

using bhb2core.Utils.Serialisation;

namespace bhb2core.Accounting.Dto
{
  public class TransactionDto : ToStringSerialiser
  {
    public string DebitAccountQualifiedName { get; set; }
    public string CreditAccountQualifiedName { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public Dictionary<string, byte> AllocationPercentByAllocationId { get; set; }
    public bool IsCommitted { get; set; }
    public TransactionRecurrence Recurrence { get; set; }
    public DateTime Timestamp { get; set; }
  }
}

using System;
using System.Collections.Generic;

using bhb2core.Utils.Serialisation;

namespace bhb2core.Accounting.Models
{
  internal class Transaction : ToStringSerialiser
  {
    public string DebitAccountId { get; set; }
    public string CreditAccountId { get; set; }
    public decimal Amount { get; set; }
    public DateTime DateAndTime { get; set; }
    public Dictionary<string, byte> AllocationPercentByAllocationId { get; set; }
    public bool IsCommitted { get; set; }
    public Dto.TransactionRecurrence Recurrence { get; set; }
  }
}

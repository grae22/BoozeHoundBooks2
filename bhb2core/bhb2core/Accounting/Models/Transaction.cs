using System;
using System.Collections.Generic;

namespace bhb2core.Accounting.Models
{
  public class Transaction
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

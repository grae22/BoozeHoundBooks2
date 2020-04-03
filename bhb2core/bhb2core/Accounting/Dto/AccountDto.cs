﻿using bhb2core.Utils.Serialisation;

namespace bhb2core.Accounting.Dto
{
  public class AccountDto : ToStringSerialiser
  {
    public string Id { get; set; }
    public string Name { get; set; }
    public string ParentAccountId { get; set; }
    public decimal Balance { get; set; }
    public bool IsFunds { get; set; }
    public bool IsIncome { get; set; }
    public bool IsExpense { get; set; }
    public bool IsDebtor { get; set; }
    public bool IsCreditor { get; set; }
  }
}

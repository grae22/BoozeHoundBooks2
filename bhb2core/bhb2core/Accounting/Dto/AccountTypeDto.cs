namespace bhb2core.Accounting.Dto
{
  public struct AccountTypeDto
  {
    public bool IsFunds { get; set; }
    public bool IsIncome { get; set; }
    public bool IsExpense { get; set; }
    public bool IsDebtor { get; set; }
    public bool IsCreditor { get; set; }
  }
}

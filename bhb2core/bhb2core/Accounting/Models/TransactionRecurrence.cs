namespace bhb2core.Accounting.Models
{
  public struct TransactionRecurrence
  {
    public bool IsRecurring { get; set; }
    public bool IsAmountVariable { get; set; }
    public bool IsDescriptionVariable { get; set; }
  }
}

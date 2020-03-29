namespace bhb2core.Accounting.Dto
{
  public struct TransactionRecurrence
  {
    public bool IsRecurring { get; set; }
    public bool IsAmountVariable { get; set; }
    public bool IsDescriptionVariable { get; set; }
  }
}

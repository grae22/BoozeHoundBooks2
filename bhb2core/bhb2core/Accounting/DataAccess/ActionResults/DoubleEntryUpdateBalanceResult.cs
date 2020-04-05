namespace bhb2core.Accounting.DataAccess.ActionResults
{
  internal struct DoubleEntryUpdateBalanceResult
  {
    public static DoubleEntryUpdateBalanceResult CreateSuccess()
    {
      return new DoubleEntryUpdateBalanceResult(true, null);
    }

    public static DoubleEntryUpdateBalanceResult CreateFailure(in string message)
    {
      return new DoubleEntryUpdateBalanceResult(false, message);
    }

    public bool IsSuccess { get; }
    public string FailureMessage { get; }

    private DoubleEntryUpdateBalanceResult(
      in bool isSuccess,
      in string failureMessage)
    {
      IsSuccess = isSuccess;
      FailureMessage = failureMessage;
    }
  }
}

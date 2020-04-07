namespace bhb2core.Common.ActionResults
{
  public struct ActionResult
  {
    public static ActionResult CreateSuccess()
    {
      return new ActionResult(true, null);
    }

    public static ActionResult CreateFailure(in string failureMessage)
    {
      return new ActionResult(false, failureMessage);
    }

    public bool IsSuccess { get; }
    public string FailureMessage { get; }

    private ActionResult(
      in bool isSuccess,
      in string failureMessage)
    {
      IsSuccess = isSuccess;
      FailureMessage = failureMessage;
    }
  }
}

namespace bhb2core.Common.ActionResults
{
  public struct GetResult<T>
  {
    public static GetResult<T> CreateSuccess(in T result)
    {
      return new GetResult<T>(true, null, result);
    }

    public static GetResult<T> CreateFailure(in string failureMessage)
    {
      return new GetResult<T>(false, failureMessage, default);
    }

    public bool IsSuccess { get; }
    public string FailureMessage { get; }
    public T Result { get; }

    private GetResult(
      in bool isSuccess,
      in string failureMessage,
      in T result)
    {
      IsSuccess = isSuccess;
      FailureMessage = failureMessage;
      Result = result;
    }
  }
}

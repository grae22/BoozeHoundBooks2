using bhb2core.Accounting.Models;

namespace bhb2core.Accounting.DataAccess.ActionResults
{
  internal struct GetAccountResult
  {
    public static GetAccountResult CreateSuccess(in Account account)
    {
      return new GetAccountResult(true, null, account);
    }

    public static GetAccountResult CreateFailure(in string failureMessage)
    {
      return new GetAccountResult(false, failureMessage, null);
    }

    public bool IsSuccess { get; }
    public string FailureMessage { get; }
    public Account Result { get; }

    private GetAccountResult(
      in bool isSuccess,
      in string failureMessage,
      in Account result)
    {
      IsSuccess = isSuccess;
      FailureMessage = failureMessage;
      Result = result;
    }
  }
}

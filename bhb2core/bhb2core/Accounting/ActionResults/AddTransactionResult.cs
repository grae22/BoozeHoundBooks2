using bhb2core.Accounting.Models;

namespace bhb2core.Accounting.ActionResults
{
  internal struct AddTransactionResult
  {
    public static AddTransactionResult CreateSuccess(in Transaction transaction)
    {
      return new AddTransactionResult(true, null, transaction);
    }

    public static AddTransactionResult CreateFailure(in string failureMessage)
    {
      return new AddTransactionResult(false, failureMessage, null);
    }

    public bool IsSuccess { get; }
    public string FailureMessage { get; }
    public Transaction Result { get; }

    private AddTransactionResult(
      in bool isSuccess,
      in string failureMessage,
      in Transaction result)
    {
      IsSuccess = isSuccess;
      FailureMessage = failureMessage;
      Result = result;
    }
  }
}

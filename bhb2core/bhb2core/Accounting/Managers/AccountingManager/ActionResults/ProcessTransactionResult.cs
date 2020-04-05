using bhb2core.Accounting.Dto;

namespace bhb2core.Accounting.Managers.AccountingManager.ActionResults
{
  public struct ProcessTransactionResult
  {
    public static ProcessTransactionResult CreateSuccess(in TransactionDto transaction)
    {
      return new ProcessTransactionResult(true, null, transaction);
    }

    public static ProcessTransactionResult CreateFailure(in string failureMessage)
    {
      return new ProcessTransactionResult(false, failureMessage, null);
    }

    public bool IsSuccess { get; }
    public string FailureMessage { get; }
    public TransactionDto Result { get; }

    private ProcessTransactionResult(
      in bool isSuccess,
      in string failureMessage,
      in TransactionDto result)
    {
      IsSuccess = isSuccess;
      FailureMessage = failureMessage;
      Result = result;
    }
  }
}

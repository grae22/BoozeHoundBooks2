using bhb2core.Accounting.Models;

namespace bhb2core.Accounting.DataAccess.ActionResults
{
  internal struct DoubleEntryUpdateBalanceResult
  {
    public static DoubleEntryUpdateBalanceResult CreateSuccess(
      in Account debitAccount,
      in Account creditAccount)
    {
      return new DoubleEntryUpdateBalanceResult(
        true,
        null,
        debitAccount,
        creditAccount);
    }

    public static DoubleEntryUpdateBalanceResult CreateFailure(in string message)
    {
      return new DoubleEntryUpdateBalanceResult(
        false,
        message,
        null,
        null);
    }

    public bool IsSuccess { get; }
    public string FailureMessage { get; }
    public Account DebitAccount { get; }
    public Account CreditAccount { get; }

    private DoubleEntryUpdateBalanceResult(
      in bool isSuccess,
      in string failureMessage,
      in Account debitAccount,
      in Account creditAccount)
    {
      IsSuccess = isSuccess;
      FailureMessage = failureMessage;
      DebitAccount = debitAccount;
      CreditAccount = creditAccount;
    }
  }
}

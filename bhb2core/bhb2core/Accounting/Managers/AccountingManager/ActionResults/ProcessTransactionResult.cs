using bhb2core.Accounting.Dto;

namespace bhb2core.Accounting.Managers.AccountingManager.ActionResults
{
  public struct ProcessTransactionResult
  {
    public static ProcessTransactionResult CreateSuccess(
      in TransactionDto transaction,
      in AccountDto debitAccount,
      in AccountDto creditAccount)
    {
      return new ProcessTransactionResult(
        true,
        null,
        transaction,
        debitAccount,
        creditAccount);
    }

    public static ProcessTransactionResult CreateFailure(in string failureMessage)
    {
      return new ProcessTransactionResult(
        false,
        failureMessage,
        null,
        null,
        null);
    }

    public bool IsSuccess { get; }
    public string FailureMessage { get; }
    public TransactionDto Result { get; }
    public AccountDto DebitAccount { get; }
    public AccountDto CreditAccount { get; }

    private ProcessTransactionResult(
      in bool isSuccess,
      in string failureMessage,
      in TransactionDto result,
      in AccountDto debitAccount,
      in AccountDto creditAccount)
    {
      IsSuccess = isSuccess;
      FailureMessage = failureMessage;
      Result = result;
      DebitAccount = debitAccount;
      CreditAccount = creditAccount;
    }
  }
}

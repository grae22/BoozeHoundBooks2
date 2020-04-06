using System.Collections.Generic;

using bhb2core.Accounting.Dto;

namespace bhb2core.Accounting.Managers.AccountingManager.ActionResults
{
  public struct ProcessTransactionResult
  {
    public static ProcessTransactionResult CreateSuccess(
      in TransactionDto transaction,
      in IEnumerable<AccountDto> updatedAccounts)
    {
      return new ProcessTransactionResult(
        true,
        null,
        transaction,
        updatedAccounts);
    }

    public static ProcessTransactionResult CreateFailure(in string failureMessage)
    {
      return new ProcessTransactionResult(
        false,
        failureMessage,
        null,
        null);
    }

    public bool IsSuccess { get; }
    public string FailureMessage { get; }
    public TransactionDto Result { get; }
    public IEnumerable<AccountDto> UpdatedAccounts { get; }

    private ProcessTransactionResult(
      in bool isSuccess,
      in string failureMessage,
      in TransactionDto result,
      in IEnumerable<AccountDto> updatedAccounts)
    {
      IsSuccess = isSuccess;
      FailureMessage = failureMessage;
      Result = result;
      UpdatedAccounts = updatedAccounts;
    }
  }
}

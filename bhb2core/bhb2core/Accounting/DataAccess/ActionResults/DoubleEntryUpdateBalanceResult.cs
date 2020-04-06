using System.Collections.Generic;

using bhb2core.Accounting.Models;

namespace bhb2core.Accounting.DataAccess.ActionResults
{
  internal struct DoubleEntryUpdateBalanceResult
  {
    public static DoubleEntryUpdateBalanceResult CreateSuccess(in IEnumerable<Account> updatedAccounts)
    {
      return new DoubleEntryUpdateBalanceResult(
        true,
        null,
        updatedAccounts);
    }

    public static DoubleEntryUpdateBalanceResult CreateFailure(in string message)
    {
      return new DoubleEntryUpdateBalanceResult(
        false,
        message,
        null);
    }

    public bool IsSuccess { get; }
    public string FailureMessage { get; }
    public IEnumerable<Account> UpdatedAccounts { get; }

    private DoubleEntryUpdateBalanceResult(
      in bool isSuccess,
      in string failureMessage,
      in IEnumerable<Account> updatedAccounts)
    {
      IsSuccess = isSuccess;
      FailureMessage = failureMessage;
      UpdatedAccounts = updatedAccounts;
    }
  }
}

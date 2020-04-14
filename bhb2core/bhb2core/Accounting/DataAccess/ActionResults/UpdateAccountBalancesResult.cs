using System.Collections.Generic;

using bhb2core.Accounting.Models;

namespace bhb2core.Accounting.DataAccess.ActionResults
{
  internal struct UpdateAccountBalancesResult
  {
    public static UpdateAccountBalancesResult CreateSuccess(in IEnumerable<Account> updatedAccounts)
    {
      return new UpdateAccountBalancesResult(
        true,
        null,
        updatedAccounts);
    }

    public static UpdateAccountBalancesResult CreateFailure(in string message)
    {
      return new UpdateAccountBalancesResult(
        false,
        message,
        null);
    }

    public bool IsSuccess { get; }
    public string FailureMessage { get; }
    public IEnumerable<Account> UpdatedAccounts { get; }

    private UpdateAccountBalancesResult(
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

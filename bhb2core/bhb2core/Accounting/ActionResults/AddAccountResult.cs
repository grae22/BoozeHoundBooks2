﻿namespace bhb2core.Accounting.ActionResults
{
  public struct AddAccountResult
  {
    public static AddAccountResult CreateSuccess()
    {
      return new AddAccountResult(true, null);
    }

    public static AddAccountResult CreateFailure(in string failureMessage)
    {
      return new AddAccountResult(false, failureMessage);
    }

    public bool IsSuccess { get; }
    public string FailureMessage { get; }

    private AddAccountResult(
      in bool isSuccess,
      in string failureMessage)
    {
      IsSuccess = isSuccess;
      FailureMessage = failureMessage;
    }
  }
}
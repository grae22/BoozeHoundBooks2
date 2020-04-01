using System;

namespace bhb2core.Accounting.Exceptions
{
  internal class AccountException : Exception
  {
    public string Details { get; }

    public AccountException(
      in string message,
      in string details)
    :
      base(message)
    {
      Details = details;
    }
  }
}

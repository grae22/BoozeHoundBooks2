using System;

namespace bhb2core.Accounting.Exceptions
{
  internal class AccountException : Exception
  {
    public AccountException(in string message)
    :
      base(message)
    {
    }
  }
}

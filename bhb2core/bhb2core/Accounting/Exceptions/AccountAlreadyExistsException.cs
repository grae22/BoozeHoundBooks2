using System;

using bhb2core.Accounting.Models;

namespace bhb2core.Accounting.Exceptions
{
  internal class AccountAlreadyExistsException : Exception
  {
    public AccountAlreadyExistsException(in Account account)
    :
      base($"An account with id {account?.Id} already exists.")
    {
    }
  }
}

using System;

namespace bhb2core.Accounting.Exceptions
{
  internal class MappingException : Exception
  {
    public MappingException(
      in string message,
      in Exception ex)
    :
      base(message, ex)
    {
    }
  }
}

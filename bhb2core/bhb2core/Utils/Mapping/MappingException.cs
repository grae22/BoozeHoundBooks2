using System;

namespace bhb2core.Utils.Mapping
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

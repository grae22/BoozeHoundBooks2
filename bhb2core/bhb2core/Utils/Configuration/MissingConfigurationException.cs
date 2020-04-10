using System;

namespace bhb2core.Utils.Configuration
{
  internal class MissingConfigurationException : Exception
  {
    public MissingConfigurationException(
      in string missingKey)
    :
      base($"Configuration not found for key \"{missingKey}\".")
    {
    }
  }
}

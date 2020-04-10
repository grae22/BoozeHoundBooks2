using System.Collections.Generic;

namespace bhb2core.Utils.Configuration
{
  public static class ConfigurationBuilder
  {
    public static IConfiguration Build(IReadOnlyDictionary<string, string> configurationsByKey)
    {
      return new Configuration(configurationsByKey);
    }
  }
}

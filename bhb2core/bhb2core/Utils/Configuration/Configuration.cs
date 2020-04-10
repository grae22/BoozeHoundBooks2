using System.Collections.Generic;

using bhb2core.Utils.Serialisation;

namespace bhb2core.Utils.Configuration
{
  internal class Configuration : IConfiguration
  {
    private readonly Dictionary<string, string> _configurationsByKey;

    public Configuration(in IReadOnlyDictionary<string, string> configurationsByKey)
    {
      _configurationsByKey = new Dictionary<string, string>(configurationsByKey);
    }

    public string GetValue(in string key)
    {
      if (_configurationsByKey.ContainsKey(key))
      {
        return _configurationsByKey[key];
      }

      throw new MissingConfigurationException(key);
    }

    public T GetValue<T>(string key)
    {
      if (_configurationsByKey.ContainsKey(key))
      {
        return Serialiser.Deserialise<T>(_configurationsByKey[key]);
      }

      throw new MissingConfigurationException(key);
    }

    public T GetValueOrDefault<T>(in string key, T defaultValue)
    {
      if (_configurationsByKey.ContainsKey(key))
      {
        return Serialiser.Deserialise<T>(_configurationsByKey[key]);
      }

      return defaultValue;
    }
  }
}

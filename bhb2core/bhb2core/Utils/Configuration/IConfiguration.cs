namespace bhb2core.Utils.Configuration
{
  public interface IConfiguration
  {
    public string GetValue(in string key);

    public T GetValue<T>(string key);

    public T GetValueOrDefault<T>(in string key, T defaultValue);
  }
}

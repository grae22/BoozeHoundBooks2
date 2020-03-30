namespace bhb2core.Utils.Mapping
{
  public interface IMapper
  {
    void VerifyConfiguration();
    TOut Map<TIn, TOut>(TIn objectToMap);
  }
}

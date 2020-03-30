namespace bhb2core.Utils.Mapping
{
  public interface IMapper
  {
    TOut Map<TIn, TOut>(TIn objectToMap);
  }
}

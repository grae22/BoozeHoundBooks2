namespace bhb2core.Utils.Mapping
{
  internal static class Mapper
  {
    private static IMapper _mapper;

    public static IMapper CreateAndInitialiseMappings()
    {
      if (_mapper != null)
      {
        return _mapper;
      }

      _mapper = new AutoMapperImplementation();

      return _mapper;
    }
  }
}

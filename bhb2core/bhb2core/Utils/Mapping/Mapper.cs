using bhb2core.Utils.Logging;

namespace bhb2core.Utils.Mapping
{
  internal static class Mapper
  {
    private static IMapper _mapper;

    public static IMapper CreateAndInitialiseMappings(in ILogger logger)
    {
      if (_mapper != null)
      {
        return _mapper;
      }

      _mapper = new AutoMapperImplementation(logger);

      return _mapper;
    }

    public static void VerifyConfiguration()
    {
      _mapper.VerifyConfiguration();
    }
  }
}

using bhb2core.Utils.Logging;
using bhb2core.Utils.Mapping;

namespace bhb2core
{
  public static class Bhb2Core
  {
    public static void Initialise(
      out ILogger logger,
      out IMapper mapper)
    {
      logger = new ConsoleLogger();
      mapper = Mapper.CreateAndInitialiseMappings(logger);
    }
  }
}

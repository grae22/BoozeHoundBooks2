using bhb2core.Utils.Mapping;

namespace bhb2core
{
  public static class Bhb2Core
  {
    public static void Initialise(out IMapper mapper)
    {
      mapper = Mapper.CreateAndInitialiseMappings();
    }
  }
}

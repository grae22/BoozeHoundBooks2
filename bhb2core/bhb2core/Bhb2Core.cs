using AutoMapper;

using Mapper = bhb2core.Utils.Mapping.Mapper;

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

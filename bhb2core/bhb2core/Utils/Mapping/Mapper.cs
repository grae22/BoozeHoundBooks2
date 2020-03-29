using AutoMapper;

using bhb2core.Accounting.Dto;
using bhb2core.Accounting.Models;

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

      var config = new MapperConfiguration(
        cfg => cfg.CreateMap<TransactionDto, Transaction>());

      _mapper = config.CreateMapper();

      return _mapper;
    }
  }
}

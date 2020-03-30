using AutoMapper;

using bhb2core.Accounting.Dto;
using bhb2core.Accounting.Models;

namespace bhb2core.Utils.Mapping
{
  internal class AutoMapperImplementation : IMapper
  {
    private readonly AutoMapper.IMapper _mapper;

    public AutoMapperImplementation()
    {
      var config = new MapperConfiguration(
        cfg => cfg.CreateMap<TransactionDto, Transaction>());

      _mapper = config.CreateMapper();
    }

    public TOut Map<TIn, TOut>(TIn objectToMap)
    {
      return _mapper.Map<TIn, TOut>(objectToMap);
    }
  }
}

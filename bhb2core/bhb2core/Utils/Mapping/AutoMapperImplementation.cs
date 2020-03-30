using System;
using System.Diagnostics;

using AutoMapper;

using bhb2core.Accounting.Dto;
using bhb2core.Accounting.Models;
using bhb2core.Utils.Logging;

namespace bhb2core.Utils.Mapping
{
  internal class AutoMapperImplementation : IMapper
  {
    private readonly ILogger _logger;
    private readonly AutoMapper.IMapper _mapper;

    public AutoMapperImplementation(in ILogger logger)
    {
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));

      MapperConfiguration config = CreateMappings();

      _mapper = config.CreateMapper();
    }

    public void VerifyConfiguration()
    {
      _mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }

    public TOut Map<TIn, TOut>(TIn objectToMap)
    {
      try
      {
        return _mapper.Map<TIn, TOut>(objectToMap);
      }
      catch (AutoMapperMappingException ex)
      {
        _logger.LogError(
          $"Failed to map {nameof(TIn)} to {nameof(TOut)}.",
          ex);

        Debug.Assert(
          false,
          "Mapper failed to perform mapping");

        throw;
      }
    }

    private MapperConfiguration CreateMappings()
    {
      _logger.LogInformation("Initialising mapper mappings...");

      return new MapperConfiguration(cfg =>
        cfg.CreateMap<TransactionDto, Transaction>());
    }
  }
}

using System;

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
        var message = $"Failed to map {nameof(TIn)} to {nameof(TOut)}.";

        _logger.LogError(message, ex);

        throw new MappingException(message, ex);
      }
    }

    private MapperConfiguration CreateMappings()
    {
      _logger.LogInformation("Initialising mapper mappings...");

      return new MapperConfiguration(
        cfg =>
        {
          cfg.CreateMap<Transaction, TransactionDto>();
          cfg.CreateMap<TransactionDto, Transaction>();
          cfg.CreateMap<Account, AccountDto>();
          cfg.CreateMap<AccountDto, Account>();
          cfg.CreateMap<AccountType, AccountTypeDto>();
          cfg.CreateMap<AccountTypeDto, AccountType>();
          cfg.CreateMap<PeriodDto, Period>();
          cfg.CreateMap<UpdatePeriodEndDateDto, UpdatePeriodEndDate>();
        });
    }
  }
}

using System;
using System.Threading.Tasks;

using bhb2core.Accounting.Dto;
using bhb2core.Accounting.Interfaces;
using bhb2core.Accounting.Managers.SubManagers.Interfaces;
using bhb2core.Accounting.Models;
using bhb2core.Utils.Logging;
using bhb2core.Utils.Mapping;

namespace bhb2core.Accounting.Managers.SubManagers
{
  internal class TransactionManager : ITransactionManager
  {
    private readonly IAccountingEngine _accountingEngine;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public TransactionManager(
      IAccountingEngine accountingEngine,
      IMapper mapper,
      ILogger logger)
    {
      _accountingEngine = accountingEngine ?? throw new ArgumentNullException(nameof(accountingEngine));
      _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task ProcessTransaction(TransactionDto transactionDto)
    {
      _logger.LogVerbose($"Transaction received: {transactionDto}.");

      Transaction transaction = _mapper.Map<TransactionDto, Transaction>(transactionDto);

      await _accountingEngine.ProcessTransaction(transaction);

      _logger.LogInformation($"Transaction processed: {transaction}");
    }
  }
}

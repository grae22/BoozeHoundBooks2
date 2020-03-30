using System;
using System.Threading.Tasks;

using bhb2core.Accounting.Dto;
using bhb2core.Accounting.Interfaces;
using bhb2core.Accounting.Models;
using bhb2core.Utils.Logging;
using bhb2core.Utils.Mapping;

namespace bhb2core.Accounting.Managers
{
  // NOTE: Don't make this public - add a factory and other assemblies can use that.
  internal class AccountingManager
  {
    private readonly ITransactionEngine _transactionEngine;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public AccountingManager(
      in ITransactionEngine transactionEngine,
      in IMapper mapper,
      in ILogger logger)
    {
      _transactionEngine = transactionEngine ?? throw new ArgumentNullException(nameof(transactionEngine));
      _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task ProcessTransaction(TransactionDto transactionDto)
    {
      _logger.LogVerbose($"Transaction received: {transactionDto}.");

      Transaction transaction = _mapper.Map<TransactionDto, Transaction>(transactionDto);

      await _transactionEngine.ProcessTransaction(transaction);
    }
  }
}

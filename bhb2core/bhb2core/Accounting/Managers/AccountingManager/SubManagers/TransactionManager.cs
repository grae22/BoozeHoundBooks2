using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using bhb2core.Accounting.DataAccess.ActionResults;
using bhb2core.Accounting.Dto;
using bhb2core.Accounting.Interfaces;
using bhb2core.Accounting.Managers.AccountingManager.ActionResults;
using bhb2core.Accounting.Managers.AccountingManager.Interfaces;
using bhb2core.Accounting.Models;
using bhb2core.Utils.Logging;
using bhb2core.Utils.Mapping;

namespace bhb2core.Accounting.Managers.AccountingManager.SubManagers
{
  internal class TransactionManager : ITransactionManager
  {
    private readonly IAccountingDataAccess _accountingDataAccess;
    private readonly IAccountingEngine _accountingEngine;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public TransactionManager(
      IAccountingDataAccess accountingDataAccess,
      IAccountingEngine accountingEngine,
      IMapper mapper,
      ILogger logger)
    {
      _accountingDataAccess = accountingDataAccess ?? throw new ArgumentNullException(nameof(accountingDataAccess));
      _accountingEngine = accountingEngine ?? throw new ArgumentNullException(nameof(accountingEngine));
      _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ProcessTransactionResult> ProcessTransaction(TransactionDto transactionDto)
    {
      _logger.LogVerbose($"Transaction received: {transactionDto}.");

      if (transactionDto == null)
      {
        return ProcessTransactionResult.CreateFailure("Input transaction is null.");
      }

      DoubleEntryUpdateBalanceResult updateBalanceResult = await _accountingEngine.PerformDoubleEntryUpdateAccountBalance(
        transactionDto.DebitAccountQualifiedName,
        transactionDto.CreditAccountQualifiedName,
        transactionDto.Amount);

      if (!updateBalanceResult.IsSuccess)
      {
        return ProcessTransactionResult.CreateFailure(updateBalanceResult.FailureMessage);
      }

      Transaction transaction = _mapper.Map<TransactionDto, Transaction>(transactionDto);

      _logger.LogInformation($"Transaction processed: {transaction}");

      return ProcessTransactionResult.CreateSuccess(null);
    }
  }
}

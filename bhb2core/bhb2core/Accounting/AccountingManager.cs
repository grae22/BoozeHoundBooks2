using System;
using System.Diagnostics;
using System.Threading.Tasks;

using AutoMapper;

using bhb2core.Accounting.Dto;
using bhb2core.Accounting.Interfaces;
using bhb2core.Accounting.Models;

namespace bhb2core.Accounting
{
  // NOTE: Don't make this public - add a factory and other assemblies can use that.
  internal class AccountingManager
  {
    private readonly ITransactionEngine _transactionEngine;
    private readonly IMapper _mapper;

    public AccountingManager(
      in ITransactionEngine transactionEngine,
      in IMapper mapper)
    {
      _transactionEngine = transactionEngine ?? throw new ArgumentNullException(nameof(transactionEngine));
      _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task ProcessTransaction(TransactionDto transactionDto)
    {
      Transaction transaction = _mapper.Map<TransactionDto, Transaction>(transactionDto);

      Debug.Assert(
        transaction != null,
        "Mapper failed to map transaction dto -> transaction.");

      await _transactionEngine.ProcessTransaction(transaction);
    }
  }
}

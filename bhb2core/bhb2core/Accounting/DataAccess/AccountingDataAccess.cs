using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using bhb2core.Accounting.DataAccess.DataAccessors;
using bhb2core.Accounting.DataAccess.Interfaces;
using bhb2core.Accounting.Interfaces;
using bhb2core.Accounting.Models;
using bhb2core.Common.ActionResults;
using bhb2core.Utils.Logging;
using bhb2core.Utils.Persistence;

namespace bhb2core.Accounting.DataAccess
{
  internal class AccountingDataAccess : IAccountingDataAccess
  {
    private readonly IPersistor _persistor;
    private readonly ILogger _logger;
    private readonly IAccountDataAccess _accountDataAccess;
    private readonly ITransactionDataAccess _transactionDataAccess;
    private readonly IPeriodDataAccess _periodDataAccess;

    public AccountingDataAccess(
      in IPersistor persistor,
      in ILogger logger)
    {
      _persistor = persistor ?? throw new ArgumentNullException(nameof(persistor));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));

      _accountDataAccess = new AccountDataAccess(persistor);
      _transactionDataAccess = new TransactionDataAccess(persistor);
      _periodDataAccess = new PeriodDataAccess(persistor);

      _persistor.Register(_accountDataAccess as IPersistable);
      _persistor.Register(_transactionDataAccess as IPersistable);
      _persistor.Register(_periodDataAccess as IPersistable);
    }

    public async Task<ActionResult> Initialise()
    {
      return await LoadData();
    }

    public async Task<GetResult<IEnumerable<Account>>> GetAllAccounts()
    {
      return await _accountDataAccess.GetAllAccounts();
    }

    public async Task<GetResult<IEnumerable<Account>>> GetAccounts(
      bool isFunds = false,
      bool isIncome = false,
      bool isExpense = false,
      bool isDebtor = false,
      bool isCreditor = false)
    {
      return await _accountDataAccess.GetAccounts(
        isFunds,
        isIncome,
        isExpense,
        isDebtor,
        isCreditor);
    }

    public async Task<GetResult<Account>> GetAccount(string accountQualifiedName)
    {
      return await _accountDataAccess.GetAccount(accountQualifiedName);
    }

    public async Task<GetResult<IReadOnlyDictionary<string, Account>>> GetAccounts(
      IEnumerable<string> accountQualifiedNames)
    {
      return await _accountDataAccess.GetAccounts(accountQualifiedNames);
    }

    public async Task<GetResult<IEnumerable<Account>>> GetParentAccountsOrdered(string accountQualifiedName)
    {
      return await _accountDataAccess.GetParentAccountsOrdered(accountQualifiedName);
    }

    public async Task<bool> IsParentAccount(string accountQualifiedName)
    {
      return await _accountDataAccess.IsParentAccount(accountQualifiedName);
    }

    public async Task<ActionResult> AddAccount(Account account)
    {
      return await _accountDataAccess.AddAccount(account);
    }

    public async Task<ActionResult> UpdateAccountBalances(
      IReadOnlyDictionary<string, decimal> balancesByAccountQualifiedName)
    {
      return await _accountDataAccess.UpdateAccountBalances(balancesByAccountQualifiedName);
    }

    public async Task<ActionResult> AddTransaction(Transaction transaction)
    {
      return await _transactionDataAccess.AddTransaction(transaction);
    }

    public async Task<GetResult<IEnumerable<Transaction>>> GetTransactions()
    {
      return await _transactionDataAccess.GetTransactions();
    }

    public async Task<bool> DoesTransactionExist(Guid idempotencyId)
    {
      return await _transactionDataAccess.DoesTransactionExist(idempotencyId);
    }

    public async Task<ActionResult> AddPeriod(Period period)
    {
      return await _periodDataAccess.AddPeriod(period);
    }

    public async Task<GetResult<Period>> GetLastPeriod()
    {
      return await _periodDataAccess.GetLastPeriod();
    }

    public async Task<GetResult<Period>> GetPeriodForDate(DateTime date)
    {
      return await _periodDataAccess.GetPeriodForDate(date);
    }

    public async Task<ActionResult> UpdateLastPeriodEndDate(DateTime newEnd)
    {
      return await _periodDataAccess.UpdateLastPeriodEndDate(newEnd);
    }

    private async Task<ActionResult> LoadData()
    {
      return await _persistor.Restore();
    }
  }
}
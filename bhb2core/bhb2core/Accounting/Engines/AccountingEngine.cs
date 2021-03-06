﻿using System.Threading.Tasks;

using bhb2core.Accounting.DataAccess.ActionResults;
using bhb2core.Accounting.Engines.Interfaces;
using bhb2core.Accounting.Engines.SubEngines;
using bhb2core.Accounting.Interfaces;
using bhb2core.Accounting.Models;
using bhb2core.Common.ActionResults;
using bhb2core.Utils.Logging;

namespace bhb2core.Accounting.Engines
{
  internal class AccountingEngine : IAccountingEngine
  {
    private readonly IAccountEngine _accountEngine;
    private readonly ITransactionEngine _transactionEngine;
    private readonly IPeriodEngine _periodEngine;

    public AccountingEngine(
      in IAccountingDataAccess accountingDataAccess,
      in ILogger logger)
    {
      _accountEngine = new AccountEngine(
        accountingDataAccess,
        logger);

      _transactionEngine = new TransactionEngine(
        accountingDataAccess,
        logger);

      _periodEngine = new PeriodEngine(
        accountingDataAccess,
        logger);
    }

    public async Task<ActionResult> CreateCurrentPeriodIfNoneExist()
    {
      return await _periodEngine.CreateCurrentPeriodIfNoneExist();
    }

    public async Task<ActionResult> CreateBaseAccountsIfMissing()
    {
      return await _accountEngine.CreateBaseAccountsIfMissing();
    }

    public string BuildAccountQualifiedName(in string name, in string parentQualifiedName)
    {
      return _accountEngine.BuildAccountQualifiedName(name, parentQualifiedName);
    }

    public bool ValidateNewAccount(in NewAccount newAccount, out string error)
    {
      return _accountEngine.ValidateNewAccount(newAccount, out error);
    }

    public async Task<ActionResult> AddAccount(NewAccount newAccount)
    {
      return await _accountEngine.AddAccount(newAccount);
    }

    public async Task<bool> DoesAccountExist(string accountQualifiedName)
    {
      return await _accountEngine.DoesAccountExist(accountQualifiedName);
    }

    public async Task<UpdateAccountBalancesResult> PerformDoubleEntryUpdateAccountBalance(
      string debitAccountQualifiedName,
      string creditAccountQualifiedName,
      decimal amount)
    {
      return await _accountEngine.PerformDoubleEntryUpdateAccountBalance(
        debitAccountQualifiedName,
        creditAccountQualifiedName,
        amount);
    }

    public async Task<ActionResult> AddTransaction(Transaction transaction)
    {
      return await _transactionEngine.AddTransaction(transaction);
    }

    public bool ValidatePeriod(
      in Period period,
      out string message)
    {
      return _periodEngine.ValidatePeriod(period, out message);
    }

    public async Task<ActionResult> AddPeriod(Period period)
    {
      return await _periodEngine.AddPeriod(period);
    }

    public async Task<ActionResult> UpdateLastPeriodEndDate(UpdateLastPeriodEndDate updatePeriodEndDate)
    {
      return await _periodEngine.UpdateLastPeriodEndDate(updatePeriodEndDate);
    }
  }
}

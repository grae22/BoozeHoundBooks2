using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using bhb2core.Accounting.DataAccess.DataAccessors;
using bhb2core.Accounting.DataAccess.Interfaces;
using bhb2core.Accounting.Interfaces;
using bhb2core.Accounting.Models;
using bhb2core.Common.ActionResults;

namespace bhb2core.Accounting.DataAccess
{
  internal class AccountingDataAccess : IAccountingDataAccess
  {
    private readonly IAccountDataAccess _accountDataAccess;
    private readonly ITransactionDataAccess _transactionDataAccess;

    public AccountingDataAccess()
    {
      _accountDataAccess = new AccountDataAccess();
      _transactionDataAccess = new TransactionDataAccess();
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

    public async Task<bool> DoesTransactionExist(Guid idempotencyId)
    {
      return await _transactionDataAccess.DoesTransactionExist(idempotencyId);
    }
  }
}
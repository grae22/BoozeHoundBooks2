﻿using System.Threading.Tasks;

using bhb2core.Accounting.DataAccess.ActionResults;
using bhb2core.Accounting.Models;
using bhb2core.Common.ActionResults;

namespace bhb2core.Accounting.Engines.Interfaces
{
  internal interface IAccountEngine
  {
    Task<ActionResult> CreateBaseAccountsIfMissing();

    string BuildAccountQualifiedName(
      in string name,
      in string parentQualifiedName);

    bool ValidateNewAccount(
      in NewAccount newAccount,
      out string error);

    Task<ActionResult> AddAccount(NewAccount newAccount);

    Task<bool> DoesAccountExist(string accountQualifiedName);

    Task<UpdateAccountBalancesResult> PerformDoubleEntryUpdateAccountBalance(
      string debitAccountQualifiedName,
      string creditAccountQualifiedName,
      decimal amount);
  }
}

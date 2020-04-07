using System.Collections.Generic;
using System.Threading.Tasks;

using bhb2core.Accounting.Dto;
using bhb2core.Common.ActionResults;

namespace bhb2core.Accounting.Managers.AccountingManager.Interfaces
{
  public interface IAccountManager
  {
    Task Initialise();

    Task<GetResult<IEnumerable<AccountDto>>> GetAllAccounts();

    Task<GetResult<IEnumerable<AccountDto>>> GetTransactionDebitAccounts();

    Task<GetResult<IEnumerable<AccountDto>>> GetTransactionCreditAccounts();

    Task<ActionResult> AddAccount(NewAccountDto newAccountDto);
  }
}

using System.Collections.Generic;
using System.Threading.Tasks;

using bhb2core.Accounting.Dto;
using bhb2core.Accounting.Managers.AccountingManager.ActionResults;

namespace bhb2core.Accounting.Managers.AccountingManager.Interfaces
{
  public interface IAccountManager
  {
    Task Initialise();

    Task<IEnumerable<AccountDto>> GetAllAccounts();

    Task<IEnumerable<AccountDto>> GetTransactionDebitAccounts();

    Task<IEnumerable<AccountDto>> GetTransactionCreditAccounts();

    Task<AddAccountResult> AddAccount(NewAccountDto newAccountDto);
  }
}

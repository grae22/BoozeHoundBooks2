using System.Collections.Generic;
using System.Threading.Tasks;

using bhb2core.Accounting.Dto;
using bhb2core.Accounting.Managers.AccountingManager.ActionResults;

namespace bhb2core.Accounting.Interfaces
{
  public interface IAccountingManager
  {
    Task<IEnumerable<AccountDto>> GetAllAccounts();

    Task<AddAccountResult> AddAccount(AccountDto account);
  }
}

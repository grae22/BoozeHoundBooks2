using System.Collections.Generic;
using System.Threading.Tasks;

using bhb2core.Accounting.Dto;

namespace bhb2core.Accounting.Interfaces
{
  public interface IAccountingManager
  {
    Task<IEnumerable<AccountDto>> GetAllAccounts();
  }
}

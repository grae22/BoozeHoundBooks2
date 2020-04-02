using System.Threading.Tasks;

using bhb2core.Accounting.Models;

namespace bhb2core.Accounting.Engines.AccountingEngine.Interfaces
{
  internal interface IAccountEngine
  {
    string BuildAccountId(in string name, in string parentId);

    bool ValidateNewAccount(in NewAccount newAccount, out string error);

    Task AddAccount(NewAccount newAccount);

    Task<bool> DoesAccountExist(string accountId);
  }
}

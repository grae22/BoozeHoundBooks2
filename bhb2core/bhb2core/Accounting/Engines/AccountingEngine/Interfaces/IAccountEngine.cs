using System.Threading.Tasks;

using bhb2core.Accounting.ActionResults;
using bhb2core.Accounting.Models;

namespace bhb2core.Accounting.Engines.AccountingEngine.Interfaces
{
  internal interface IAccountEngine
  {
    string BuildAccountQualifiedName(in string name, in string parentQualifiedName);

    bool ValidateNewAccount(in NewAccount newAccount, out string error);

    Task<AddAccountResult> AddAccount(NewAccount newAccount);

    Task<bool> DoesAccountExist(string accountQualifiedName);
  }
}

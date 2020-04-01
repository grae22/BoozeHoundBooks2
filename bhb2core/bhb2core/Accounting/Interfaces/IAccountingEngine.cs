using bhb2core.Accounting.Engines.AccountingEngine.Interfaces;

namespace bhb2core.Accounting.Interfaces
{
  internal interface IAccountingEngine :
    IAccountEngine,
    ITransactionEngine
  {
  }
}

using bhb2core.Accounting.Engines.Interfaces;

namespace bhb2core.Accounting.Interfaces
{
  internal interface IAccountingEngine :
    IAccountEngine,
    ITransactionEngine,
    IPeriodEngine
  {
  }
}

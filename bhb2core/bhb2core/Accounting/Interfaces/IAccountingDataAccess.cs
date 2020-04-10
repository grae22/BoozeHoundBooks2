using bhb2core.Accounting.DataAccess.Interfaces;

namespace bhb2core.Accounting.Interfaces
{
  internal interface IAccountingDataAccess :
    IAccountDataAccess,
    ITransactionDataAccess
  {
  }
}

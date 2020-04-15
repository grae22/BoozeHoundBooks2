using System.Threading.Tasks;

using bhb2core.Accounting.DataAccess.Interfaces;
using bhb2core.Accounting.Models;
using bhb2core.Common.ActionResults;
using bhb2core.Utils.Persistence;

namespace bhb2core.Accounting.DataAccess.DataAccessors
{
  internal class PeriodDataAccess : IPeriodDataAccess, IPersistable
  {
    public string PersistenceId => "Periods";

    public async Task<ActionResult> AddPeriod(Period period)
    {
      throw new System.NotImplementedException();
    }

    public async Task<GetResult<Period>> GetLastPeriod()
    {
      throw new System.NotImplementedException();
    }

    public string Serialise()
    {
      throw new System.NotImplementedException();
    }

    public void Deserialise(in string serialisedData)
    {
      throw new System.NotImplementedException();
    }
  }
}

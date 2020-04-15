using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using bhb2core.Accounting.DataAccess.Interfaces;
using bhb2core.Accounting.Models;
using bhb2core.Common.ActionResults;
using bhb2core.Utils.Persistence;
using bhb2core.Utils.Serialisation;

namespace bhb2core.Accounting.DataAccess.DataAccessors
{
  internal class PeriodDataAccess : IPeriodDataAccess, IPersistable
  {
    public string PersistenceId => "Periods";

    private const string SerialisedDataPeriodsKey = "periods";

    private readonly IPersistor _persistor;
    private readonly List<Period> _periods = new List<Period>();

    public PeriodDataAccess(IPersistor persistor)
    {
      _persistor = persistor ?? throw new ArgumentNullException(nameof(persistor));
    }

    public async Task<ActionResult> AddPeriod(Period period)
    {
      _periods.Add(period);

      return await _persistor.Persist();
    }

    public async Task<GetResult<Period>> GetLastPeriod()
    {
      if (_periods.Count == 0)
      {
        return await Task.FromResult(
          GetResult<Period>.CreateFailure("No periods found"));
      }

      Period period = _periods
        .OrderBy(p => p.Start)
        .LastOrDefault();

      return await Task.FromResult(
        GetResult<Period>.CreateSuccess(period));
    }

    public async Task<GetResult<Period>> GetPeriodForDate(DateTime date)
    {
      Period period = _periods
        .FirstOrDefault(p =>
          p.Start <= date &&
          p.End >= date);

      if (period == null)
      {
        return await Task.FromResult(
          GetResult<Period>.CreateFailure($"No period found for date: {date:yyyy-MM-dd}"));
      }

      return await Task.FromResult(
        GetResult<Period>.CreateSuccess(period));
    }

    public async Task<ActionResult> UpdateLastPeriodEndDate(DateTime newEnd)
    {
      GetResult<Period> getLastPeriodResult = await GetLastPeriod();

      if (!getLastPeriodResult.IsSuccess)
      {
        return ActionResult.CreateFailure("Failed to retrieve last period");
      }

      Period oldLastPeriod = getLastPeriodResult.Result;

      var newLastPeriod = new Period(
        oldLastPeriod.Start,
        newEnd);

      _periods.Remove(oldLastPeriod);
      _periods.Add(newLastPeriod);

      return await _persistor.Persist();
    }

    public string Serialise()
    {
      var serialisedData = new Dictionary<string, string>
      {
        { SerialisedDataPeriodsKey, Serialiser.Serialise(_periods) }
      };

      return Serialiser.Serialise(serialisedData);
    }

    public void Deserialise(in string serialisedData)
    {
      var serialisedDataByKey = Serialiser.Deserialise<IReadOnlyDictionary<string, string>>(serialisedData);

      _periods.Clear();

      _periods.AddRange(
        Serialiser.Deserialise<List<Period>>(
          serialisedDataByKey[SerialisedDataPeriodsKey]));
    }
  }
}

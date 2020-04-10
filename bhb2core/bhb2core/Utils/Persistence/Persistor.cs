using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using bhb2core.Common.ActionResults;
using bhb2core.Utils.Logging;

namespace bhb2core.Utils.Persistence
{
  internal abstract class Persistor : IPersistor
  {
    protected ILogger Logger { get; }
    protected IReadOnlyDictionary<string, IPersistable> PersistablesById => _persistablesById;

    private readonly Dictionary<string, IPersistable> _persistablesById = new Dictionary<string, IPersistable>();

    protected Persistor(in ILogger logger)
    {
      Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void Register(in IPersistable persistable)
    {
      if (persistable == null)
      {
        throw new ArgumentNullException(nameof(persistable));
      }

      if (_persistablesById.ContainsKey(persistable.PersistenceId))
      {
        Logger.LogError($"Another persistable is already registered with id \"{persistable.PersistenceId}\".");

        return;
      }

      _persistablesById.Add(persistable.PersistenceId, persistable);

      Logger.LogInformation($"Persistable registered with id \"{persistable.PersistenceId}\".");
    }

    public abstract Task<ActionResult> Persist();

    public abstract Task<ActionResult> Restore();
  }
}

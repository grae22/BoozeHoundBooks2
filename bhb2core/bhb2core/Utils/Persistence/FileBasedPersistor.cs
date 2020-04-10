using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using bhb2core.Common.ActionResults;
using bhb2core.Utils.Logging;
using bhb2core.Utils.Serialisation;

namespace bhb2core.Utils.Persistence
{
  internal class FileBasedPersistor : Persistor
  {
    private readonly string _filename;

    public FileBasedPersistor(
      in string filename,
      in ILogger logger)
    :
      base(logger)
    {
      _filename = filename;
    }

    public override async Task<ActionResult> Persist()
    {
      try
      {
        Logger.LogInformation("Persisting data started.");

        var serialisedData = new Dictionary<string, string>();

        foreach (var idAndPersistable in PersistablesById)
        {
          Logger.LogInformation($"Adding \"{idAndPersistable.Key}\"...");

          serialisedData.Add(
            idAndPersistable.Key,
            idAndPersistable.Value.Serialise());
        }

        Logger.LogInformation($"Persisting to \"{_filename}\"...");

        await File.WriteAllTextAsync(
          _filename,
          Serialiser.Serialise(serialisedData));

        Logger.LogInformation("Persisting data completed.");

        return ActionResult.CreateSuccess();
      }
      catch (Exception ex)
      {
        Logger.LogError("Unhandled exception while persisting data.", ex);

        return ActionResult.CreateFailure("Persist operation failed due to an unhandled exception.");
      }
    }

    public override async Task<ActionResult> Restore()
    {
      try
      {
        Logger.LogInformation("Restoring data started.");

        Logger.LogInformation($"Restoring from \"{_filename}\"...");

        string data = await File.ReadAllTextAsync(_filename);

        var serialisedData = Serialiser.Deserialise<IReadOnlyDictionary<string, string>>(data);

        foreach (var idAndSerialisedData in serialisedData)
        {
          string id = idAndSerialisedData.Key;
          string serialisedDataForPersistable = idAndSerialisedData.Value;

          if (!PersistablesById.ContainsKey(id))
          {
            Logger.LogError($"\"{id}\" is not a registered persistable.");
          }

          Logger.LogInformation($"Restoring \"{id}\"...");

          PersistablesById[id].Deserialise(serialisedDataForPersistable);
        }

        Logger.LogInformation("Restoring data completed.");

        return ActionResult.CreateSuccess();
      }
      catch (Exception ex)
      {
        Logger.LogError("Unhandled exception while restoring data.", ex);

        return ActionResult.CreateFailure("Restore operation failed due to an unhandled exception.");
      }
    }
  }
}

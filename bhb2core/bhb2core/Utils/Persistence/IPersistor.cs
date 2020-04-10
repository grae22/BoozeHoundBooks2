using System.Threading.Tasks;

using bhb2core.Common.ActionResults;

namespace bhb2core.Utils.Persistence
{
  internal interface IPersistor
  {
    void Register(in IPersistable persistable);

    Task<ActionResult> Persist();

    Task<ActionResult> Restore();
  }
}

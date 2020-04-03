using bhb2core.Utils.Serialisation;

namespace bhb2core.Accounting.Models
{
  internal class NewAccount : ToStringSerialiser
  {
    public string Name { get; set; }
    public Account ParentAccount { get; set; }
  }
}

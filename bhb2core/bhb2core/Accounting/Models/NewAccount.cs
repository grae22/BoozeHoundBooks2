using bhb2core.Utils.Serialisation;

namespace bhb2core.Accounting.Models
{
  public class NewAccount : ToStringSerialiser
  {
    public string Name { get; set; }
    public string ParentAccountId { get; set; }
  }
}

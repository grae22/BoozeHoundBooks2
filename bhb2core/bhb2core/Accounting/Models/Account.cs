using bhb2core.Utils.Serialisation;

namespace bhb2core.Accounting.Models
{
  internal class Account : ToStringSerialiser
  {
    public string Id { get; set; }
    public string Name { get; set; }
    public decimal Balance { get; set; }
  }
}

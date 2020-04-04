using bhb2core.Utils.Serialisation;

namespace bhb2core.Accounting.Dto
{
  public class NewAccountDto : ToStringSerialiser
  {
    public string Name { get; set; }
    public string ParentAccountQualifiedName { get; set; }
  }
}

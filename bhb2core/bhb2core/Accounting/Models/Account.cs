using bhb2core.Utils.Serialisation;

namespace bhb2core.Accounting.Models
{
  internal class Account : ToStringSerialiser
  {
    public string QualifiedName { get; set; }
    public string Name { get; set; }
    public string ParentAccountQualifiedName { get; set; }
    public decimal Balance { get; set; }
    public bool IsFunds { get; set; }
    public bool IsIncome { get; set; }
    public bool IsExpense { get; set; }
    public bool IsDebtor { get; set; }
    public bool IsCreditor { get; set; }

    public bool HasParent => ParentAccountQualifiedName != null;
  }
}

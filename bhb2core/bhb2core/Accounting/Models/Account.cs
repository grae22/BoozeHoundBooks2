using System.Collections.Generic;

using bhb2core.Utils.Serialisation;

namespace bhb2core.Accounting.Models
{
  internal class Account : ToStringSerialiser
  {
    public AccountType AccountType { get; set; }
    public string QualifiedName { get; set; }
    public string Name { get; set; }
    public string ParentAccountQualifiedName { get; set; }
    public decimal Balance { get; set; }
    public IEnumerable<AccountType> AccountTypesWithDebitPermission { get; set; }
    public IEnumerable<AccountType> AccountTypesWithCreditPermission { get; set; }

    public bool HasParent => ParentAccountQualifiedName != null;
  }
}

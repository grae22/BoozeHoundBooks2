﻿using bhb2core.Utils.Serialisation;

namespace bhb2core.Accounting.Dto
{
  public class AccountDto : ToStringSerialiser
  {
    public string QualifiedName { get; set; }
    public string Name { get; set; }
    public string ParentAccountQualifiedName { get; set; }
    public decimal Balance { get; set; }
    public AccountTypeDto AccountType { get; set; }

    public bool HasParent => ParentAccountQualifiedName != null;
  }
}

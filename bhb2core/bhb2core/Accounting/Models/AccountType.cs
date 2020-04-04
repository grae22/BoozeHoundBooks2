using System.Diagnostics;

namespace bhb2core.Accounting.Models
{
  internal struct AccountType
  {
    public static AccountType CreateFunds() { return new AccountType(isFunds: true); }
    public static AccountType CreateIncome() { return new AccountType(isIncome: true); }
    public static AccountType CreateExpense() { return new AccountType(isExpense: true); }
    public static AccountType CreateDebtor() { return new AccountType(isDebtor: true); }
    public static AccountType CreateCreditor() { return new AccountType(isCreditor: true); }

    public bool IsFunds { get; set; }
    public bool IsIncome { get; set; }
    public bool IsExpense { get; set; }
    public bool IsDebtor { get; set; }
    public bool IsCreditor { get; set; }

    public AccountType(
      bool isFunds = false,
      bool isIncome = false,
      bool isExpense = false,
      bool isDebtor = false,
      bool isCreditor = false)
    {
      IsFunds = isFunds;
      IsIncome = isIncome;
      IsExpense = isExpense;
      IsDebtor = isDebtor;
      IsCreditor = isCreditor;

      Debug.Assert(
        (IsFunds ^ IsIncome ^ IsExpense ^ IsDebtor ^ IsCreditor) ||
        (!IsFunds && !IsIncome && !IsExpense && !IsDebtor && !IsCreditor),
        "Account types should mutually exclusive.");
    }
  }
}

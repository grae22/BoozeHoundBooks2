using System.Collections.Generic;
using System.Linq;

using bhb2core.Accounting.Dto;

namespace bhb2desktop.Utils
{
  internal static class AccountUtils
  {
    public static string GetFullyQualifiedAccountName(
      in string accountId,
      in IEnumerable<AccountDto> accounts,
      in char accountSeparator)
    {
      string id = accountId;

      AccountDto account = accounts.First(a => a.Id.Equals(id));

      if (account.ParentAccountId == null)
      {
        return account.Name;
      }

      var accountNames = new List<string>
      {
        account.Name
      };

      AppendAccountParentNameToListRecursive(
        account.ParentAccountId,
        accounts,
        accountNames);

      accountNames.Reverse();

      return string.Join(accountSeparator, accountNames);
    }

    private static void AppendAccountParentNameToListRecursive(
      in string accountParentId,
      in IEnumerable<AccountDto> accounts,
      in List<string> names)
    {
      string parentId = accountParentId;

      AccountDto parent =
        accounts
          .First(a => a.Id.Equals(parentId));

      names.Add(parent.Name);

      if (parent.ParentAccountId == null)
      {
        return;
      }

      AppendAccountParentNameToListRecursive(
        parent.ParentAccountId,
        accounts,
        names);
    }
  }
}

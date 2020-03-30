using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using bhb2core.Accounting.Dto;
using bhb2core.Accounting.Interfaces;
using bhb2core.Accounting.Managers;

using bhb2coreTests.Accounting.TestUtils;

using NUnit.Framework;

namespace bhb2coreTests.Accounting
{
  [TestFixture]
  public class AccountTests
  {
    [Test]
    public async Task Given_NoAccounts_When_RetrievingAllAccounts_Then_BaseAccountsAreReturned()
    {
      // Arrange.
      AccountingManager testObject = AccountingManagerFactory.Create(out IAccountingDataAccess accountingDataAccess);

      // Act.
      IEnumerable<AccountDto> accounts = await testObject.GetAllAccounts();

      // Assert.
      Assert.NotNull(accounts.SingleOrDefault(a => a.Id.Equals("Funds")));
      Assert.NotNull(accounts.SingleOrDefault(a => a.Id.Equals("Income")));
      Assert.NotNull(accounts.SingleOrDefault(a => a.Id.Equals("Expense")));
      Assert.NotNull(accounts.SingleOrDefault(a => a.Id.Equals("Debtor")));
      Assert.NotNull(accounts.SingleOrDefault(a => a.Id.Equals("Creditor")));
    }
  }
}

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using bhb2core.Accounting.Dto;
using bhb2core.Accounting.Exceptions;
using bhb2core.Accounting.Interfaces;
using bhb2core.Accounting.Managers.AccountingManager;
using bhb2core.Accounting.Managers.AccountingManager.ActionResults;
using bhb2core.Accounting.Models;

using bhb2coreTests.Accounting.TestUtils;

using NSubstitute;
using NSubstitute.ExceptionExtensions;

using NUnit.Framework;

namespace bhb2coreTests.Accounting
{
  [TestFixture]
  public class AccountTests
  {
    private static readonly string[] BaseAccountIds =
    {
      "Funds",
      "Income",
      "Expense",
      "Debtor",
      "Creditor"
    };

    [Test]
    public async Task Given_NoAccountsAndAccountingManagerInitialised_When_AllAccountsRetrieved_Then_BaseAccountsAreReturned()
    {
      // Arrange.
      AccountingManager testObject = AccountingManagerFactory.Create(
        out IAccountingDataAccess accountingDataAccess,
        useConcreteDataAccessMock: true);

      // Act.
      IEnumerable<AccountDto> accounts = await testObject.GetAllAccounts();

      // Assert.
      foreach (var id in BaseAccountIds)
      {
        Assert.NotNull(accounts.SingleOrDefault(a => a.Id.Equals(id)));
      }
    }

    [Test]
    public async Task Given_BaseAccountsAlreadyExist_When_AccountingManagerIntialised_Then_BaseAccountsAreNotDuplicated()
    {
      // Arrange.
      AccountingManager testObject = AccountingManagerFactory.Create(
        out IAccountingDataAccess accountingDataAccess,
        performInitialisation: false,
        useConcreteDataAccessMock: true);

      foreach (var id in BaseAccountIds)
      {
        await accountingDataAccess.AddAccount(
          new Account
          {
            Id = id,
            Name = id,
            Balance = 0m
          });
      }

      // Act.
      await testObject.Initialise();

      // Assert.
      IEnumerable<AccountDto> accounts = await testObject.GetAllAccounts();

      foreach (var id in BaseAccountIds)
      {
        Assert.NotNull(accounts.SingleOrDefault(a => a.Id.Equals(id)));
      }
    }

    [Test]
    public async Task Given_ExistingAccount_When_AddAccountCalledWithSameAccountId_Then_ReturnsFailure()
    {
      // Arrange.
      AccountingManager testObject = AccountingManagerFactory.Create(out IAccountingDataAccess accountingDataAccess);

      var newAccount = new NewAccountDto
      {
        Name = "SomeAccount",
        ParentAccountId = BaseAccountIds[0]
      };

      accountingDataAccess
        .AddAccount(Arg.Any<Account>())
        .Throws(new AccountException("Account already exists"));

      // Act.
      AddAccountResult result = await testObject.AddAccount(newAccount);

      // Assert.
      Assert.IsFalse(result.IsSuccess);
    }
  }
}

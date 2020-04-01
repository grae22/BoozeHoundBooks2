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
    private const char AccountIdSeparator = '.';

    private static readonly string[] BaseAccountNames =
    {
      "Funds",
      "Income",
      "Expense",
      "Debtor",
      "Creditor"
    };

    private static readonly string[] BaseAccountIds =
    {
      "funds",
      "income",
      "expense",
      "debtor",
      "creditor"
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
      foreach (var name in BaseAccountNames)
      {
        Assert.NotNull(accounts.SingleOrDefault(a => a.Name.Equals(name)));
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

      foreach (var name in BaseAccountNames)
      {
        await accountingDataAccess.AddAccount(
          new Account
          {
            Id = name.ToLower(),
            Name = name,
            Balance = 0m
          });
      }

      // Act.
      await testObject.Initialise();

      // Assert.
      IEnumerable<AccountDto> accounts = await testObject.GetAllAccounts();

      foreach (var name in BaseAccountNames)
      {
        Assert.NotNull(accounts.SingleOrDefault(a => a.Name.Equals(name)));
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
        .Throws(new AccountException("Account already exists", ""));

      // Act.
      AddAccountResult result = await testObject.AddAccount(newAccount);

      // Assert.
      Assert.IsFalse(result.IsSuccess);
    }

    [TestCase("")]
    [TestCase("   ")]
    [TestCase("Some.Name")]
    [TestCase(null)]
    public async Task Given_NewAccount_When_AddAccountCalledWithInvalidName_Then_ReturnsFailure(string accountName)
    {
      // Arrange.
      AccountingManager testObject = AccountingManagerFactory.Create(out IAccountingDataAccess accountingDataAccess);

      var newAccount = new NewAccountDto
      {
        Name = accountName,
        ParentAccountId = BaseAccountIds[0]
      };

      // Act.
      AddAccountResult result = await testObject.AddAccount(newAccount);

      // Assert.
      Assert.IsFalse(result.IsSuccess);
    }

    [TestCase(" Name")]
    [TestCase("Name ")]
    [TestCase(" Name ")]
    [TestCase("   Name   ")]
    public async Task Given_NewAccount_When_NameContainsLeadingOrTrailingWhitespace_Then_WhitespaceIsRemoved(string accountName)
    {
      // Arrange.
      AccountingManager testObject = AccountingManagerFactory.Create(out IAccountingDataAccess accountingDataAccess);

      accountingDataAccess
        .GetAccountsById(Arg.Any<string[]>())
        .Returns(new Dictionary<string, Account>
        {
          { BaseAccountIds[0], new Account() }
        });

      var newAccount = new NewAccountDto
      {
        Name = accountName,
        ParentAccountId = BaseAccountIds[0]
      };

      // Act.
      await testObject.AddAccount(newAccount);

      // Assert.
      string expectedId = $"{newAccount.ParentAccountId}{AccountIdSeparator}{accountName.Trim().ToLower()}";
      string expectedName = accountName.Trim();

      await accountingDataAccess
        .Received(1)
        .AddAccount(Arg.Is<Account>(a =>
          a.Id.Equals(expectedId) &&
          a.Name.Equals(expectedName)));
    }

    [Test]
    public async Task Given_NewAccount_When_Added_Then_AccountIdIsLowercase()
    {
      // Arrange.
      AccountingManager testObject = AccountingManagerFactory.Create(out IAccountingDataAccess accountingDataAccess);

      accountingDataAccess
        .GetAccountsById(Arg.Any<string[]>())
        .Returns(new Dictionary<string, Account>
        {
          { BaseAccountIds[0], new Account() }
        });

      var newAccount = new NewAccountDto
      {
        Name = "SomeAccount",
        ParentAccountId = BaseAccountIds[0]
      };

      // Act.
      await testObject.AddAccount(newAccount);

      // Assert.
      string expectedId = $"{newAccount.ParentAccountId}{AccountIdSeparator}{newAccount.Name}".ToLower();

      await accountingDataAccess
        .Received(1)
        .AddAccount(Arg.Is<Account>(a =>
          a.Id.Equals(expectedId)));
    }

    [TestCase("")]
    [TestCase("   ")]
    [TestCase(null)]
    public async Task Given_NewAccount_When_AddAccountCalledWithInvalidParentId_Then_ReturnsFailure(string parentId)
    {
      // Arrange.
      AccountingManager testObject = AccountingManagerFactory.Create(out IAccountingDataAccess accountingDataAccess);

      var newAccount = new NewAccountDto
      {
        Name = "SomeName",
        ParentAccountId = parentId
      };

      // Act.
      AddAccountResult result = await testObject.AddAccount(newAccount);

      // Assert.
      Assert.IsFalse(result.IsSuccess);
    }

    [Test]
    public async Task Given_NewAccount_When_ParentIdDoesNotExist_Then_ReturnsFailure()
    {
      // Arrange.
      AccountingManager testObject = AccountingManagerFactory.Create(out IAccountingDataAccess accountingDataAccess);

      var newAccount = new NewAccountDto
      {
        Name = "SomeName",
        ParentAccountId = "SomeInvalidParentId"
      };

      // Act.
      AddAccountResult result = await testObject.AddAccount(newAccount);

      // Assert.
      Assert.IsFalse(result.IsSuccess);
    }
  }
}

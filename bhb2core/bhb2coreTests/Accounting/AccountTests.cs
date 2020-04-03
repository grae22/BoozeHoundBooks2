using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using bhb2core.Accounting.ActionResults;
using bhb2core.Accounting.Dto;
using bhb2core.Accounting.Exceptions;
using bhb2core.Accounting.Interfaces;
using bhb2core.Accounting.Managers.AccountingManager;
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
    private const string FundsAccountId = "funds";
    private const string IncomeAccountId = "income";
    private const string ExpenseAccountId = "expense";
    private const string DebtorAccountId = "debtor";
    private const string CreditorAccountId = "creditor";
    private const string FundsAccountName = "Funds";
    private const string IncomeAccountName = "Income";
    private const string ExpenseAccountName = "Expense";
    private const string DebtorAccountName = "Debtor";
    private const string CreditorAccountName = "Creditor";

    private static readonly string[] BaseAccountNames =
    {
      FundsAccountName,
      IncomeAccountName,
      ExpenseAccountName,
      DebtorAccountName,
      CreditorAccountName
    };

    private static readonly string[] BaseAccountIds =
    {
      FundsAccountId,
      IncomeAccountId,
      ExpenseAccountId,
      DebtorAccountId,
      CreditorAccountId
    };

    [TestCase(FundsAccountId, FundsAccountName, true, false, false, false, false)]
    [TestCase(IncomeAccountId, IncomeAccountName, false, true, false, false, false)]
    [TestCase(ExpenseAccountId, ExpenseAccountName, false, false, true, false, false)]
    [TestCase(DebtorAccountId, DebtorAccountName, false, false, false, true, false)]
    [TestCase(CreditorAccountId, CreditorAccountName, false, false, false, false, true)]
    public async Task Given_NoAccountsAndAccountingManagerInitialised_When_AllAccountsRetrieved_Then_BaseAccountsAreReturned(
      string id,
      string name,
      bool isFunds,
      bool isIncome,
      bool isExpense,
      bool isDebtor,
      bool isCreditor)
    {
      // Arrange.
      AccountingManager testObject = AccountingManagerFactory.Create(
        out IAccountingDataAccess accountingDataAccess,
        useConcreteDataAccessMock: true);

      // Act.
      IEnumerable<AccountDto> accounts = await testObject.GetAllAccounts();

      // Assert.
      AccountDto account = accounts.SingleOrDefault(a => a.Id.Equals(id));

      Assert.NotNull(account);
      Assert.AreEqual(name, account.Name);
      Assert.IsNull(account.ParentAccountId);
      Assert.AreEqual(0m, account.Balance);
      Assert.AreEqual(isFunds, account.IsFunds);
      Assert.AreEqual(isIncome, account.IsIncome);
      Assert.AreEqual(isExpense, account.IsExpense);
      Assert.AreEqual(isDebtor, account.IsDebtor);
      Assert.AreEqual(isCreditor, account.IsCreditor);
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
        .GetAccountById(FundsAccountId)
        .Returns(
          GetAccountResult.CreateSuccess(new Account { Id = FundsAccountId }));

      var newAccount = new NewAccountDto
      {
        Name = accountName,
        ParentAccountId = FundsAccountId
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
        .GetAccountById(FundsAccountId)
        .Returns(
          GetAccountResult.CreateSuccess(new Account { Id = FundsAccountId }));

      var newAccount = new NewAccountDto
      {
        Name = "SomeAccount",
        ParentAccountId = FundsAccountId
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

    [Test]
    public async Task Given_NewAccount_When_Added_Then_ParentAccountIdIsCorrect()
    {
      // Arrange.
      AccountingManager testObject = AccountingManagerFactory.Create(out IAccountingDataAccess accountingDataAccess);

      accountingDataAccess
        .GetAccountById(FundsAccountId)
        .Returns(
          GetAccountResult.CreateSuccess(new Account { Id = FundsAccountId }));

      var newAccount = new NewAccountDto
      {
        Name = "SomeAccount",
        ParentAccountId = BaseAccountIds[0]
      };

      // Act.
      await testObject.AddAccount(newAccount);

      // Assert.
      string expectedId = newAccount.ParentAccountId;

      await accountingDataAccess
        .Received(1)
        .AddAccount(Arg.Is<Account>(a =>
          a.ParentAccountId.Equals(expectedId)));
    }

    [Test]
    public async Task Given_NewAccount_When_Added_Then_BalanceIsZero()
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
      await accountingDataAccess
        .Received()
        .AddAccount(Arg.Is<Account>(a => a.Balance == 0m));
    }

    [Test]
    public async Task Given_NewFundsAccount_When_Added_Then_AccountTypeIsFunds()
    {
      // Arrange.
      AccountingManager testObject = AccountingManagerFactory.Create(out IAccountingDataAccess accountingDataAccess);

      accountingDataAccess
        .GetAccountById(FundsAccountId)
        .Returns(
          GetAccountResult.CreateSuccess(new Account { IsFunds = true }));

      var newAccount = new NewAccountDto
      {
        Name = "SomeAccount",
        ParentAccountId = FundsAccountId
      };

      // Act.
      await testObject.AddAccount(newAccount);

      // Assert.
      await accountingDataAccess
        .Received(1)
        .AddAccount(Arg.Is<Account>(a =>
          a.Name.Equals(newAccount.Name) &&
          a.IsFunds));
    }

    [Test]
    public async Task Given_NewIncomeAccount_When_Added_Then_AccountTypeIsIncome()
    {
      // Arrange.
      AccountingManager testObject = AccountingManagerFactory.Create(out IAccountingDataAccess accountingDataAccess);

      accountingDataAccess
        .GetAccountById(IncomeAccountId)
        .Returns(
          GetAccountResult.CreateSuccess(new Account { IsIncome = true }));

      var newAccount = new NewAccountDto
      {
        Name = "SomeAccount",
        ParentAccountId = IncomeAccountId
      };

      // Act.
      await testObject.AddAccount(newAccount);

      // Assert.
      await accountingDataAccess
        .Received(1)
        .AddAccount(Arg.Is<Account>(a => 
          a.Name.Equals(newAccount.Name) &&
          a.IsIncome));
    }

    [Test]
    public async Task Given_NewExpenseAccount_When_Added_Then_AccountTypeIsExpense()
    {
      // Arrange.
      AccountingManager testObject = AccountingManagerFactory.Create(out IAccountingDataAccess accountingDataAccess);

      accountingDataAccess
        .GetAccountById(ExpenseAccountId)
        .Returns(
          GetAccountResult.CreateSuccess(new Account { IsExpense = true }));

      var newAccount = new NewAccountDto
      {
        Name = "SomeAccount",
        ParentAccountId = ExpenseAccountId
      };

      // Act.
      await testObject.AddAccount(newAccount);

      // Assert.
      await accountingDataAccess
        .Received(1)
        .AddAccount(Arg.Is<Account>(a =>
          a.Name.Equals(newAccount.Name) &&
          a.IsExpense));
    }

    [Test]
    public async Task Given_NewDebtorAccount_When_Added_Then_AccountTypeIsDebtor()
    {
      // Arrange.
      AccountingManager testObject = AccountingManagerFactory.Create(out IAccountingDataAccess accountingDataAccess);

      accountingDataAccess
        .GetAccountById(DebtorAccountId)
        .Returns(
          GetAccountResult.CreateSuccess(new Account { IsDebtor = true }));

      var newAccount = new NewAccountDto
      {
        Name = "SomeAccount",
        ParentAccountId = DebtorAccountId
      };

      // Act.
      await testObject.AddAccount(newAccount);

      // Assert.
      await accountingDataAccess
        .Received(1)
        .AddAccount(Arg.Is<Account>(a =>
          a.Name.Equals(newAccount.Name) &&
          a.IsDebtor));
    }

    [Test]
    public async Task Given_NewCreditorAccount_When_Added_Then_AccountTypeIsCreditor()
    {
      // Arrange.
      AccountingManager testObject = AccountingManagerFactory.Create(out IAccountingDataAccess accountingDataAccess);

      accountingDataAccess
        .GetAccountById(CreditorAccountId)
        .Returns(
          GetAccountResult.CreateSuccess(new Account { IsCreditor = true }));

      var newAccount = new NewAccountDto
      {
        Name = "SomeAccount",
        ParentAccountId = CreditorAccountId
      };

      // Act.
      await testObject.AddAccount(newAccount);

      // Assert.
      await accountingDataAccess
        .Received(1)
        .AddAccount(Arg.Is<Account>(a =>
          a.Name.Equals(newAccount.Name) &&
          a.IsCreditor));
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

    [Test]
    public async Task Given_BaseAccountsExist_When_RetrievingTransactionDebitAccounts_Then_CorrectAccountsAreReturned()
    {
      // Arrange.
      AccountingManager testObject = AccountingManagerFactory.Create(out IAccountingDataAccess accountingDataAccess);

      accountingDataAccess
        .GetAccounts(
          isFunds: true,
          isIncome: true,
          isDebtor: true,
          isCreditor: true)
        .Returns(new[]
        {
          new Account { Id = FundsAccountId },
          new Account { Id = IncomeAccountId },
          new Account { Id = DebtorAccountId },
          new Account { Id = CreditorAccountId },
        });

      // Act.
      IEnumerable<AccountDto> accounts = await testObject.GetTransactionDebitAccounts();

      // Assert.
      accounts.Single(a => a.Id.Equals(FundsAccountId));
      accounts.Single(a => a.Id.Equals(IncomeAccountId));
      accounts.Single(a => a.Id.Equals(DebtorAccountId));
      accounts.Single(a => a.Id.Equals(CreditorAccountId));

      Assert.Pass();
    }

    [Test]
    public async Task Given_BaseAccountsExist_When_RetrievingTransactionCreditAccounts_Then_CorrectAccountsAreReturned()
    {
      // Arrange.
      AccountingManager testObject = AccountingManagerFactory.Create(out IAccountingDataAccess accountingDataAccess);

      accountingDataAccess
        .GetAccounts(
          isFunds: true,
          isExpense: true,
          isDebtor: true,
          isCreditor: true)
        .Returns(new[]
        {
          new Account { Id = FundsAccountId },
          new Account { Id = ExpenseAccountId },
          new Account { Id = DebtorAccountId },
          new Account { Id = CreditorAccountId },
        });

      // Act.
      IEnumerable<AccountDto> accounts = await testObject.GetTransactionCreditAccounts();

      // Assert.
      accounts.Single(a => a.Id.Equals(FundsAccountId));
      accounts.Single(a => a.Id.Equals(ExpenseAccountId));
      accounts.Single(a => a.Id.Equals(DebtorAccountId));
      accounts.Single(a => a.Id.Equals(CreditorAccountId));

      Assert.Pass();
    }
  }
}

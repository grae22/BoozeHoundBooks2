using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using bhb2core.Accounting.DataAccess.ActionResults;
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
    private const char AccountQualifiedNameSeparator = '.';
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

    [Test]
    public void Given_NoBaseAccounts_When_Initialised_Then_FundsAccountCreated()
    {
      // Arrange.
      // Act.
      AccountingManagerFactory.Create(out IAccountingDataAccess accountingDataAccess);

      // Assert.
      accountingDataAccess
        .Received(1)
        .AddAccount(Arg.Is<Account>(a =>
          a.AccountType.IsFunds &&
          a.QualifiedName.Equals(FundsAccountName) &&
          a.Name.Equals(FundsAccountName) &&
          a.Balance == 0m));

      accountingDataAccess
        .Received(1)
        .AddAccount(Arg.Is<Account>(a =>
          a.QualifiedName.Equals(FundsAccountName) &&
          a.AccountTypesWithDebitPermission.Contains(AccountType.CreateFunds()) &&
          a.AccountTypesWithDebitPermission.Contains(AccountType.CreateExpense()) &&
          a.AccountTypesWithDebitPermission.Contains(AccountType.CreateDebtor()) &&
          a.AccountTypesWithDebitPermission.Contains(AccountType.CreateCreditor()) &&
          !a.AccountTypesWithDebitPermission.Contains(AccountType.CreateIncome())));

      accountingDataAccess
        .Received(1)
        .AddAccount(Arg.Is<Account>(a =>
          a.QualifiedName.Equals(FundsAccountName) &&
          a.AccountTypesWithCreditPermission.Contains(AccountType.CreateFunds()) &&
          a.AccountTypesWithCreditPermission.Contains(AccountType.CreateIncome()) &&
          a.AccountTypesWithCreditPermission.Contains(AccountType.CreateDebtor()) &&
          a.AccountTypesWithCreditPermission.Contains(AccountType.CreateCreditor()) &&
          !a.AccountTypesWithCreditPermission.Contains(AccountType.CreateExpense())));
    }

    [Test]
    public void Given_NoBaseAccounts_When_Initialised_Then_IncomeAccountCreated()
    {
      // Arrange.
      // Act.
      AccountingManagerFactory.Create(out IAccountingDataAccess accountingDataAccess);

      // Assert.
      accountingDataAccess
        .Received(1)
        .AddAccount(Arg.Is<Account>(a =>
          a.AccountType.IsIncome &&
          a.QualifiedName.Equals(IncomeAccountName) &&
          a.Name.Equals(IncomeAccountName) &&
          a.Balance == 0m));

      accountingDataAccess
        .Received(1)
        .AddAccount(Arg.Is<Account>(a =>
          a.QualifiedName.Equals(IncomeAccountName) &&
          a.AccountTypesWithDebitPermission.Contains(AccountType.CreateFunds()) &&
          !a.AccountTypesWithDebitPermission.Contains(AccountType.CreateIncome()) &&
          !a.AccountTypesWithDebitPermission.Contains(AccountType.CreateExpense()) &&
          !a.AccountTypesWithDebitPermission.Contains(AccountType.CreateDebtor()) &&
          !a.AccountTypesWithDebitPermission.Contains(AccountType.CreateCreditor())));

      accountingDataAccess
        .Received(1)
        .AddAccount(Arg.Is<Account>(a =>
          a.QualifiedName.Equals(IncomeAccountName) &&
          !a.AccountTypesWithCreditPermission.Contains(AccountType.CreateFunds()) &&
          !a.AccountTypesWithCreditPermission.Contains(AccountType.CreateIncome()) &&
          !a.AccountTypesWithCreditPermission.Contains(AccountType.CreateExpense()) &&
          !a.AccountTypesWithCreditPermission.Contains(AccountType.CreateDebtor()) &&
          !a.AccountTypesWithCreditPermission.Contains(AccountType.CreateCreditor())));
    }

    [Test]
    public void Given_NoBaseAccounts_When_Initialised_Then_ExpenseAccountCreated()
    {
      // Arrange.
      // Act.
      AccountingManagerFactory.Create(out IAccountingDataAccess accountingDataAccess);

      // Assert.
      accountingDataAccess
        .Received(1)
        .AddAccount(Arg.Is<Account>(a =>
          a.AccountType.IsExpense &&
          a.QualifiedName.Equals(ExpenseAccountName) &&
          a.Name.Equals(ExpenseAccountName) &&
          a.Balance == 0m));

      accountingDataAccess
        .Received(1)
        .AddAccount(Arg.Is<Account>(a =>
          a.QualifiedName.Equals(ExpenseAccountName) &&
          !a.AccountTypesWithDebitPermission.Contains(AccountType.CreateFunds()) &&
          !a.AccountTypesWithDebitPermission.Contains(AccountType.CreateIncome()) &&
          !a.AccountTypesWithDebitPermission.Contains(AccountType.CreateExpense()) &&
          !a.AccountTypesWithDebitPermission.Contains(AccountType.CreateDebtor()) &&
          !a.AccountTypesWithDebitPermission.Contains(AccountType.CreateCreditor())));

      accountingDataAccess
        .Received(1)
        .AddAccount(Arg.Is<Account>(a =>
          a.QualifiedName.Equals(ExpenseAccountName) &&
          a.AccountTypesWithCreditPermission.Contains(AccountType.CreateFunds()) &&
          !a.AccountTypesWithCreditPermission.Contains(AccountType.CreateIncome()) &&
          !a.AccountTypesWithCreditPermission.Contains(AccountType.CreateExpense()) &&
          !a.AccountTypesWithCreditPermission.Contains(AccountType.CreateDebtor()) &&
          !a.AccountTypesWithCreditPermission.Contains(AccountType.CreateCreditor())));
    }

    [Test]
    public void Given_NoBaseAccounts_When_Initialised_Then_DebtorAccountCreated()
    {
      // Arrange.
      // Act.
      AccountingManagerFactory.Create(out IAccountingDataAccess accountingDataAccess);

      // Assert.
      accountingDataAccess
        .Received(1)
        .AddAccount(Arg.Is<Account>(a =>
          a.AccountType.IsDebtor &&
          a.QualifiedName.Equals(DebtorAccountName) &&
          a.Name.Equals(DebtorAccountName) &&
          a.Balance == 0m));

      accountingDataAccess
        .Received(1)
        .AddAccount(Arg.Is<Account>(a =>
          a.QualifiedName.Equals(DebtorAccountName) &&
          a.AccountTypesWithDebitPermission.Contains(AccountType.CreateFunds()) &&
          !a.AccountTypesWithDebitPermission.Contains(AccountType.CreateIncome()) &&
          !a.AccountTypesWithDebitPermission.Contains(AccountType.CreateExpense()) &&
          !a.AccountTypesWithDebitPermission.Contains(AccountType.CreateDebtor()) &&
          !a.AccountTypesWithDebitPermission.Contains(AccountType.CreateCreditor())));

      accountingDataAccess
        .Received(1)
        .AddAccount(Arg.Is<Account>(a =>
          a.QualifiedName.Equals(DebtorAccountName) &&
          a.AccountTypesWithCreditPermission.Contains(AccountType.CreateFunds()) &&
          !a.AccountTypesWithCreditPermission.Contains(AccountType.CreateIncome()) &&
          !a.AccountTypesWithCreditPermission.Contains(AccountType.CreateExpense()) &&
          !a.AccountTypesWithCreditPermission.Contains(AccountType.CreateDebtor()) &&
          !a.AccountTypesWithCreditPermission.Contains(AccountType.CreateCreditor())));
    }

    [Test]
    public void Given_NoBaseAccounts_When_Initialised_Then_CreditorAccountCreated()
    {
      // Arrange.
      // Act.
      AccountingManagerFactory.Create(out IAccountingDataAccess accountingDataAccess);

      // Assert.
      accountingDataAccess
        .Received(1)
        .AddAccount(Arg.Is<Account>(a =>
          a.AccountType.IsCreditor &&
          a.QualifiedName.Equals(CreditorAccountName) &&
          a.Name.Equals(CreditorAccountName) &&
          a.Balance == 0m));

      accountingDataAccess
        .Received(1)
        .AddAccount(Arg.Is<Account>(a =>
          a.QualifiedName.Equals(CreditorAccountName) &&
          a.AccountTypesWithDebitPermission.Contains(AccountType.CreateFunds()) &&
          !a.AccountTypesWithDebitPermission.Contains(AccountType.CreateIncome()) &&
          !a.AccountTypesWithDebitPermission.Contains(AccountType.CreateExpense()) &&
          !a.AccountTypesWithDebitPermission.Contains(AccountType.CreateDebtor()) &&
          !a.AccountTypesWithDebitPermission.Contains(AccountType.CreateCreditor())));

      accountingDataAccess
        .Received(1)
        .AddAccount(Arg.Is<Account>(a =>
          a.QualifiedName.Equals(CreditorAccountName) &&
          a.AccountTypesWithCreditPermission.Contains(AccountType.CreateFunds()) &&
          !a.AccountTypesWithCreditPermission.Contains(AccountType.CreateIncome()) &&
          !a.AccountTypesWithCreditPermission.Contains(AccountType.CreateExpense()) &&
          !a.AccountTypesWithCreditPermission.Contains(AccountType.CreateDebtor()) &&
          !a.AccountTypesWithCreditPermission.Contains(AccountType.CreateCreditor())));
    }

    [Test]
    public async Task Given_BaseAccountsAlreadyExist_When_AccountingManagerInitialised_Then_BaseAccountsAreNotDuplicated()
    {
      // Arrange.
      AccountingManager testObject = AccountingManagerFactory.Create(out IAccountingDataAccess accountingDataAccess);

      // Act.
      await testObject.Initialise();

      // Assert.
      foreach (var name in BaseAccountNames)
      {
        await accountingDataAccess
          .Received(1)
          .AddAccount(Arg.Is<Account>(a => a.Name.Equals(name)));
      }
    }

    [Test]
    public async Task Given_ExistingAccount_When_AddAccountCalledWithSameAccountDetails_Then_ReturnsFailure()
    {
      // Arrange.
      AccountingManager testObject = AccountingManagerFactory.Create(out IAccountingDataAccess accountingDataAccess);

      var newAccount = new NewAccountDto
      {
        Name = "SomeAccount",
        ParentAccountQualifiedName = BaseAccountNames[0]
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
        ParentAccountQualifiedName = BaseAccountNames[0]
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

      //AccountingManagerFactory.ConfigureDataAccessWithBaseAccounts(accountingDataAccess);

      var newAccount = new NewAccountDto
      {
        Name = accountName,
        ParentAccountQualifiedName = FundsAccountName
      };

      // Act.
      await testObject.AddAccount(newAccount);

      // Assert.
      string expectedQualifiedName = $"{newAccount.ParentAccountQualifiedName}{AccountQualifiedNameSeparator}{accountName.Trim()}";
      string expectedName = accountName.Trim();

      await accountingDataAccess
        .Received(1)
        .AddAccount(Arg.Is<Account>(a =>
          a.QualifiedName.Equals(expectedQualifiedName) &&
          a.Name.Equals(expectedName)));
    }

    [Test]
    public async Task Given_NewAccount_When_Added_Then_AccountQualifiedNameIsCorrect()
    {
      // Arrange.
      AccountingManager testObject = AccountingManagerFactory.Create(out IAccountingDataAccess accountingDataAccess);

      var newAccount = new NewAccountDto
      {
        Name = "SomeAccount",
        ParentAccountQualifiedName = FundsAccountName
      };

      // Act.
      await testObject.AddAccount(newAccount);

      // Assert.
      string expectedQualifiedName = $"{newAccount.ParentAccountQualifiedName}{AccountQualifiedNameSeparator}{newAccount.Name}";

      await accountingDataAccess
        .Received(1)
        .AddAccount(Arg.Is<Account>(a =>
          a.QualifiedName.Equals(expectedQualifiedName, StringComparison.Ordinal)));
    }

    [Test]
    public async Task Given_NewAccount_When_Added_Then_ParentAccountQualifiedNameIsCorrect()
    {
      // Arrange.
      AccountingManager testObject = AccountingManagerFactory.Create(out IAccountingDataAccess accountingDataAccess);

      var newAccount = new NewAccountDto
      {
        Name = "SomeAccount",
        ParentAccountQualifiedName = BaseAccountNames[0]
      };

      // Act.
      await testObject.AddAccount(newAccount);

      // Assert.
      string expectedQualifiedName = newAccount.ParentAccountQualifiedName;

      await accountingDataAccess
        .Received(1)
        .AddAccount(Arg.Is<Account>(a =>
          a.ParentAccountQualifiedName.Equals(expectedQualifiedName)));
    }

    [Test]
    public async Task Given_NewAccount_When_Added_Then_BalanceIsZero()
    {
      // Arrange.
      AccountingManager testObject = AccountingManagerFactory.Create(out IAccountingDataAccess accountingDataAccess);

      accountingDataAccess
        .GetAccounts(Arg.Any<string[]>())
        .Returns(new Dictionary<string, Account>
        {
          { BaseAccountNames[0], new Account() }
        });

      var newAccount = new NewAccountDto
      {
        Name = "SomeAccount",
        ParentAccountQualifiedName = BaseAccountNames[0]
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

      var newAccount = new NewAccountDto
      {
        Name = "SomeAccount",
        ParentAccountQualifiedName = FundsAccountName
      };

      // Act.
      await testObject.AddAccount(newAccount);

      // Assert.
      await accountingDataAccess
        .Received(1)
        .AddAccount(Arg.Is<Account>(a =>
          a.Name.Equals(newAccount.Name) &&
          a.AccountType.IsFunds));
    }

    [Test]
    public async Task Given_NewIncomeAccount_When_Added_Then_AccountTypeIsIncome()
    {
      // Arrange.
      AccountingManager testObject = AccountingManagerFactory.Create(out IAccountingDataAccess accountingDataAccess);

      var newAccount = new NewAccountDto
      {
        Name = "SomeAccount",
        ParentAccountQualifiedName = IncomeAccountName
      };

      // Act.
      await testObject.AddAccount(newAccount);

      // Assert.
      await accountingDataAccess
        .Received(1)
        .AddAccount(Arg.Is<Account>(a => 
          a.Name.Equals(newAccount.Name) &&
          a.AccountType.IsIncome));
    }

    [Test]
    public async Task Given_NewExpenseAccount_When_Added_Then_AccountTypeIsExpense()
    {
      // Arrange.
      AccountingManager testObject = AccountingManagerFactory.Create(out IAccountingDataAccess accountingDataAccess);

      var newAccount = new NewAccountDto
      {
        Name = "SomeAccount",
        ParentAccountQualifiedName = ExpenseAccountName
      };

      // Act.
      await testObject.AddAccount(newAccount);

      // Assert.
      await accountingDataAccess
        .Received(1)
        .AddAccount(Arg.Is<Account>(a =>
          a.Name.Equals(newAccount.Name) &&
          a.AccountType.IsExpense));
    }

    [Test]
    public async Task Given_NewDebtorAccount_When_Added_Then_AccountTypeIsDebtor()
    {
      // Arrange.
      AccountingManager testObject = AccountingManagerFactory.Create(out IAccountingDataAccess accountingDataAccess);

      var newAccount = new NewAccountDto
      {
        Name = "SomeAccount",
        ParentAccountQualifiedName = DebtorAccountName
      };

      // Act.
      await testObject.AddAccount(newAccount);

      // Assert.
      await accountingDataAccess
        .Received(1)
        .AddAccount(Arg.Is<Account>(a =>
          a.Name.Equals(newAccount.Name) &&
          a.AccountType.IsDebtor));
    }

    [Test]
    public async Task Given_NewCreditorAccount_When_Added_Then_AccountTypeIsCreditor()
    {
      // Arrange.
      AccountingManager testObject = AccountingManagerFactory.Create(out IAccountingDataAccess accountingDataAccess);

      var newAccount = new NewAccountDto
      {
        Name = "SomeAccount",
        ParentAccountQualifiedName = CreditorAccountName
      };

      // Act.
      await testObject.AddAccount(newAccount);

      // Assert.
      await accountingDataAccess
        .Received(1)
        .AddAccount(Arg.Is<Account>(a =>
          a.Name.Equals(newAccount.Name) &&
          a.AccountType.IsCreditor));
    }

    [Test]
    public async Task Given_NewAccount_When_Added_Then_DebitWhitelistAccountTypesAreSameAsParents()
    {
      // Arrange.
      AccountingManager testObject = AccountingManagerFactory.Create(out IAccountingDataAccess accountingDataAccess);

      var newAccount = new NewAccountDto
      {
        Name = "SomeAccount",
        ParentAccountQualifiedName = FundsAccountName
      };

      // Act.
      await testObject.AddAccount(newAccount);

      // Assert.
      await accountingDataAccess
        .Received(1)
        .AddAccount(Arg.Is<Account>(a =>
          a.Name.Equals(newAccount.Name) &&
          a.AccountTypesWithDebitPermission.Contains(AccountType.CreateFunds()) &&
          a.AccountTypesWithDebitPermission.Contains(AccountType.CreateExpense()) &&
          a.AccountTypesWithDebitPermission.Contains(AccountType.CreateDebtor()) &&
          a.AccountTypesWithDebitPermission.Contains(AccountType.CreateCreditor())));

      await accountingDataAccess
        .Received(0)
        .AddAccount(Arg.Is<Account>(a =>
          a.Name.Equals(newAccount.Name) &&
          a.AccountTypesWithDebitPermission.Contains(AccountType.CreateIncome())));
    }

    [Test]
    public async Task Given_NewAccount_When_Added_Then_CreditWhitelistAccountTypesAreSameAsParents()
    {
      // Arrange.
      AccountingManager testObject = AccountingManagerFactory.Create(out IAccountingDataAccess accountingDataAccess);

      var newAccount = new NewAccountDto
      {
        Name = "SomeAccount",
        ParentAccountQualifiedName = FundsAccountName
      };

      // Act.
      await testObject.AddAccount(newAccount);

      // Assert.
      await accountingDataAccess
        .Received(1)
        .AddAccount(Arg.Is<Account>(a =>
          a.Name.Equals(newAccount.Name) &&
          a.AccountTypesWithCreditPermission.Contains(AccountType.CreateFunds()) &&
          a.AccountTypesWithCreditPermission.Contains(AccountType.CreateIncome()) &&
          a.AccountTypesWithCreditPermission.Contains(AccountType.CreateDebtor()) &&
          a.AccountTypesWithCreditPermission.Contains(AccountType.CreateCreditor())));

      await accountingDataAccess
        .Received(0)
        .AddAccount(Arg.Is<Account>(a =>
          a.Name.Equals(newAccount.Name) &&
          a.AccountTypesWithCreditPermission.Contains(AccountType.CreateExpense())));
    }

    [TestCase("")]
    [TestCase("   ")]
    [TestCase(null)]
    public async Task Given_NewAccount_When_AddAccountCalledWithInvalidParentQualifiedName_Then_ReturnsFailure(
      string parentQualifiedName)
    {
      // Arrange.
      AccountingManager testObject = AccountingManagerFactory.Create(out IAccountingDataAccess accountingDataAccess);

      var newAccount = new NewAccountDto
      {
        Name = "SomeName",
        ParentAccountQualifiedName = parentQualifiedName
      };

      // Act.
      AddAccountResult result = await testObject.AddAccount(newAccount);

      // Assert.
      Assert.IsFalse(result.IsSuccess);
    }

    [Test]
    public async Task Given_NewAccount_When_ParentAccountDoesNotExist_Then_ReturnsFailure()
    {
      // Arrange.
      AccountingManager testObject = AccountingManagerFactory.Create(out IAccountingDataAccess accountingDataAccess);

      var newAccount = new NewAccountDto
      {
        Name = "SomeName",
        ParentAccountQualifiedName = "SomeInvalidParent"
      };

      // Act.
      AddAccountResult result = await testObject.AddAccount(newAccount);

      // Assert.
      Assert.IsFalse(result.IsSuccess);
    }

    [Test]
    public async Task Given_AccountsExist_When_RetrievingTransactionDebitAccounts_Then_CorrectAccountsAreReturned()
    {
      // Arrange.
      AccountingManager testObject = AccountingManagerFactory.Create(out IAccountingDataAccess accountingDataAccess);

      string someNonBaseAccount = $"{FundsAccountName}{AccountQualifiedNameSeparator}SomeAccount";

      accountingDataAccess
        .GetAccounts(
          isFunds: true,
          isIncome: true,
          isDebtor: true,
          isCreditor: true)
        .Returns(new[]
        {
          new Account { QualifiedName = FundsAccountName },
          new Account { QualifiedName = IncomeAccountName },
          new Account { QualifiedName = DebtorAccountName },
          new Account { QualifiedName = CreditorAccountName },
          new Account { QualifiedName = someNonBaseAccount, ParentAccountQualifiedName = FundsAccountName }
        });

      // Act.
      IEnumerable<AccountDto> accounts = await testObject.GetTransactionDebitAccounts();

      // Assert.
      Assert.IsNull(accounts.SingleOrDefault(a => a.QualifiedName.Equals(FundsAccountName)));

      accounts.Single(a => a.QualifiedName.Equals(IncomeAccountName));
      accounts.Single(a => a.QualifiedName.Equals(DebtorAccountName));
      accounts.Single(a => a.QualifiedName.Equals(CreditorAccountName));
      accounts.Single(a => a.QualifiedName.Equals(someNonBaseAccount));

      Assert.Pass();
    }

    [Test]
    public async Task Given_AccountsExist_When_RetrievingTransactionCreditAccounts_Then_CorrectAccountsAreReturned()
    {
      // Arrange.
      AccountingManager testObject = AccountingManagerFactory.Create(out IAccountingDataAccess accountingDataAccess);

      string someNonBaseAccount = $"{FundsAccountName}{AccountQualifiedNameSeparator}SomeAccount";

      accountingDataAccess
        .GetAccounts(
          isFunds: true,
          isExpense: true,
          isDebtor: true,
          isCreditor: true)
        .Returns(new[]
        {
          new Account { QualifiedName = FundsAccountName },
          new Account { QualifiedName = ExpenseAccountName },
          new Account { QualifiedName = DebtorAccountName },
          new Account { QualifiedName = CreditorAccountName },
          new Account { QualifiedName = someNonBaseAccount, ParentAccountQualifiedName = FundsAccountName }
        });

      // Act.
      IEnumerable<AccountDto> accounts = await testObject.GetTransactionCreditAccounts();

      // Assert.
      Assert.IsNull(accounts.SingleOrDefault(a => a.QualifiedName.Equals(FundsAccountName)));
      accounts.Single(a => a.QualifiedName.Equals(ExpenseAccountName));
      accounts.Single(a => a.QualifiedName.Equals(DebtorAccountName));
      accounts.Single(a => a.QualifiedName.Equals(CreditorAccountName));
      accounts.Single(a => a.QualifiedName.Equals(someNonBaseAccount));

      Assert.Pass();
    }
  }
}

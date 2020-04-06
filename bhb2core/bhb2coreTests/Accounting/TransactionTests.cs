using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using bhb2core.Accounting.DataAccess.ActionResults;
using bhb2core.Accounting.Dto;
using bhb2core.Accounting.Interfaces;
using bhb2core.Accounting.Managers.AccountingManager;
using bhb2core.Accounting.Managers.AccountingManager.ActionResults;
using bhb2core.Accounting.Models;

using bhb2coreTests.Accounting.TestUtils;

using NSubstitute;

using NUnit.Framework;

namespace bhb2coreTests.Accounting
{
  [TestFixture]
  public class TransactionTests
  {
    [Test]
    public async Task Given_Transaction_When_ProcessedSuccessfully_Then_ResultIsSuccess()
    {
      // Arrange.
      const string debitAccountName = "Funds";
      const string creditAccountName = "Expense";
      const decimal amount = 123.45m;

      AccountingManager testObject = AccountingManagerFactory.Create(out IAccountingDataAccess accountingDataAccess);

      AccountingManagerFactory.ConfigureDataAccessWithBaseAccounts(accountingDataAccess);

      Account debitAccount = accountingDataAccess.GetAccount(debitAccountName).Result.Result;
      Account creditAccount = accountingDataAccess.GetAccount(creditAccountName).Result.Result;

      accountingDataAccess
        .GetAccounts(Arg.Any<IEnumerable<string>>())
        .Returns(
          new Dictionary<string, Account>
          {
            { debitAccountName, debitAccount },
            { creditAccountName, creditAccount }
          });

      accountingDataAccess
        .GetParentAccountsOrdered(debitAccountName)
        .Returns(GetResult<IEnumerable<Account>>.CreateSuccess(new Account[] {}));

      accountingDataAccess
        .GetParentAccountsOrdered(creditAccountName)
        .Returns(GetResult<IEnumerable<Account>>.CreateSuccess(new Account[] {}));

      var transaction = new TransactionDto
      {
        DebitAccountQualifiedName = debitAccountName,
        CreditAccountQualifiedName = creditAccountName,
        Amount = amount
      };

      // Act.
      ProcessTransactionResult result = await testObject.ProcessTransaction(transaction);

      // Assert.
      Assert.IsTrue(result.IsSuccess);
    }

    [Test]
    public async Task Given_Transaction_When_ProcessedSuccessfully_Then_ResultAccountBalancesAreCorrect()
    {
      // Arrange.
      const string debitAccountName = "Funds";
      const string creditAccountName = "Expense";
      const decimal amount = 123.45m;

      AccountingManager testObject = AccountingManagerFactory.Create(out IAccountingDataAccess accountingDataAccess);

      AccountingManagerFactory.ConfigureDataAccessWithBaseAccounts(accountingDataAccess);

      Account debitAccount = accountingDataAccess.GetAccount(debitAccountName).Result.Result;
      Account creditAccount = accountingDataAccess.GetAccount(creditAccountName).Result.Result;

      accountingDataAccess
        .GetAccounts(Arg.Any<IEnumerable<string>>())
        .Returns(
          new Dictionary<string, Account>
          {
            { debitAccountName, debitAccount },
            { creditAccountName, creditAccount }
          });

      accountingDataAccess
        .GetParentAccountsOrdered(debitAccountName)
        .Returns(GetResult<IEnumerable<Account>>.CreateSuccess(new Account[] { }));

      accountingDataAccess
        .GetParentAccountsOrdered(creditAccountName)
        .Returns(GetResult<IEnumerable<Account>>.CreateSuccess(new Account[] { }));

      var transaction = new TransactionDto
      {
        DebitAccountQualifiedName = debitAccountName,
        CreditAccountQualifiedName = creditAccountName,
        Amount = amount
      };

      // Act.
      ProcessTransactionResult result = await testObject.ProcessTransaction(transaction);

      // Assert.
      AccountDto resultDebitAccount = result.UpdatedAccounts.First(a => a.QualifiedName.Equals(debitAccountName));
      AccountDto resultCreditAccount = result.UpdatedAccounts.First(a => a.QualifiedName.Equals(creditAccountName));

      Assert.AreEqual(-amount, resultDebitAccount.Balance);
      Assert.AreEqual(amount, resultCreditAccount.Balance);
    }

    [Test]
    public async Task Given_Transaction_When_TransactionProcessed_Then_AccountBalancesAreUpdatedCorrectly()
    {
      // Arrange.
      const string debitAccountName = "Funds";
      const string creditAccountName = "Expense";
      const decimal amount = 123.45m;

      AccountingManager testObject = AccountingManagerFactory.Create(out IAccountingDataAccess accountingDataAccess);

      AccountingManagerFactory.ConfigureDataAccessWithBaseAccounts(accountingDataAccess);

      Account debitAccount = accountingDataAccess.GetAccount(debitAccountName).Result.Result;
      Account creditAccount = accountingDataAccess.GetAccount(creditAccountName).Result.Result;

      accountingDataAccess
        .GetAccounts(Arg.Any<IEnumerable<string>>())
        .Returns(
          new Dictionary<string, Account>
          {
            { debitAccountName, debitAccount },
            { creditAccountName, creditAccount }
          });

      var transaction = new TransactionDto
      {
        DebitAccountQualifiedName = debitAccountName,
        CreditAccountQualifiedName = creditAccountName,
        Amount = amount
      };

      // Act.
      await testObject.ProcessTransaction(transaction);

      // Assert.
      decimal expectedDebitAccountBalance = -transaction.Amount;
      decimal expectedCreditAccountBalance = transaction.Amount;

      await accountingDataAccess
        .Received(1)
        .UpdateAccountBalances(
          Arg.Is<IReadOnlyDictionary<string, decimal>>(balancesByAccount =>
            balancesByAccount.Single(x =>
              x.Key.Equals(debitAccountName)).Value == expectedDebitAccountBalance));

      await accountingDataAccess
        .Received(1)
        .UpdateAccountBalances(
          Arg.Is<IReadOnlyDictionary<string, decimal>>(balancesByAccount =>
            balancesByAccount.Single(x =>
              x.Key.Equals(creditAccountName)).Value == expectedCreditAccountBalance));
    }

    [Test]
    public async Task Given_Transaction_When_TransactionProcessed_Then_ResultParentAccountBalancesAreUpdatedCorrectly()
    {
      // Arrange.
      const string debitAccountName = "Cash";
      const string creditAccountName = "Food";
      const string debitParentAccountName = "Funds";
      const string creditParentAccountName = "Expense";
      const decimal amount = 123.45m;

      AccountingManager testObject = AccountingManagerFactory.Create(out IAccountingDataAccess accountingDataAccess);

      AccountingManagerFactory.ConfigureDataAccessWithBaseAccounts(accountingDataAccess);

      Account parentDebitAccount = accountingDataAccess.GetAccount(debitParentAccountName).Result.Result;
      Account parentCreditAccount = accountingDataAccess.GetAccount(creditParentAccountName).Result.Result;

      var debitAccount = new Account
      {
        AccountType = AccountType.CreateFunds(),
        QualifiedName = debitAccountName,
        ParentAccountQualifiedName = debitParentAccountName,
        AccountTypesWithDebitPermission = parentDebitAccount.AccountTypesWithDebitPermission,
        AccountTypesWithCreditPermission = parentDebitAccount.AccountTypesWithCreditPermission
      };

      var creditAccount = new Account
      {
        AccountType = AccountType.CreateExpense(),
        QualifiedName = creditAccountName,
        ParentAccountQualifiedName = creditParentAccountName,
        AccountTypesWithDebitPermission = parentCreditAccount.AccountTypesWithDebitPermission,
        AccountTypesWithCreditPermission = parentCreditAccount.AccountTypesWithCreditPermission
      };

      accountingDataAccess
        .GetAccounts(Arg.Any<IEnumerable<string>>())
        .Returns(
          new Dictionary<string, Account>
          {
            { debitAccountName, debitAccount },
            { creditAccountName, creditAccount }
          });

      accountingDataAccess
        .GetParentAccountsOrdered(debitAccountName)
        .Returns(GetResult<IEnumerable<Account>>.CreateSuccess(new[]
        {
          parentDebitAccount
        }));

      accountingDataAccess
        .GetParentAccountsOrdered(creditAccountName)
        .Returns(GetResult<IEnumerable<Account>>.CreateSuccess(new[]
        {
          parentCreditAccount
        }));

      var transaction = new TransactionDto
      {
        DebitAccountQualifiedName = debitAccountName,
        CreditAccountQualifiedName = creditAccountName,
        Amount = amount
      };

      // Act.
      ProcessTransactionResult result = await testObject.ProcessTransaction(transaction);

      // Assert.
      AccountDto resultDebitAccount = result.UpdatedAccounts.First(a => a.QualifiedName.Equals(debitParentAccountName));
      AccountDto resultCreditAccount = result.UpdatedAccounts.First(a => a.QualifiedName.Equals(creditParentAccountName));

      Assert.AreEqual(-amount, resultDebitAccount.Balance);
      Assert.AreEqual(amount, resultCreditAccount.Balance);
    }

    [Test]
    public async Task Given_Transaction_When_TransactionProcessed_Then_ParentAccountBalancesAreUpdatedCorrectly()
    {
      // Arrange.
      const string debitAccountName = "Cash";
      const string creditAccountName = "Food";
      const string debitParentAccountName = "Funds";
      const string creditParentAccountName = "Expense";
      const decimal amount = 123.45m;

      AccountingManager testObject = AccountingManagerFactory.Create(out IAccountingDataAccess accountingDataAccess);

      AccountingManagerFactory.ConfigureDataAccessWithBaseAccounts(accountingDataAccess);

      Account parentDebitAccount = accountingDataAccess.GetAccount(debitParentAccountName).Result.Result;
      Account parentCreditAccount = accountingDataAccess.GetAccount(creditParentAccountName).Result.Result;

      var debitAccount = new Account
      {
        AccountType = AccountType.CreateFunds(),
        QualifiedName = debitAccountName,
        ParentAccountQualifiedName = debitParentAccountName,
        AccountTypesWithDebitPermission = parentDebitAccount.AccountTypesWithDebitPermission,
        AccountTypesWithCreditPermission = parentDebitAccount.AccountTypesWithCreditPermission
      };

      var creditAccount = new Account
      {
        AccountType = AccountType.CreateExpense(),
        QualifiedName = creditAccountName,
        ParentAccountQualifiedName = creditParentAccountName,
        AccountTypesWithDebitPermission = parentCreditAccount.AccountTypesWithDebitPermission,
        AccountTypesWithCreditPermission = parentCreditAccount.AccountTypesWithCreditPermission
      };

      accountingDataAccess
        .GetAccounts(Arg.Any<IEnumerable<string>>())
        .Returns(
          new Dictionary<string, Account>
          {
            { debitAccountName, debitAccount },
            { creditAccountName, creditAccount }
          });

      accountingDataAccess
        .GetParentAccountsOrdered(debitAccountName)
        .Returns(GetResult<IEnumerable<Account>>.CreateSuccess(new[]
        {
          parentDebitAccount
        }));

      accountingDataAccess
        .GetParentAccountsOrdered(creditAccountName)
        .Returns(GetResult<IEnumerable<Account>>.CreateSuccess(new[]
        {
          parentCreditAccount
        }));

      var transaction = new TransactionDto
      {
        DebitAccountQualifiedName = debitAccountName,
        CreditAccountQualifiedName = creditAccountName,
        Amount = amount
      };

      // Act.
      await testObject.ProcessTransaction(transaction);

      // Assert.
      decimal expectedDebitAccountBalance = -transaction.Amount;
      decimal expectedCreditAccountBalance = transaction.Amount;

      await accountingDataAccess
        .Received(1)
        .UpdateAccountBalances(
          Arg.Is<IReadOnlyDictionary<string, decimal>>(balancesByAccount =>
            balancesByAccount.Single(x =>
              x.Key.Equals(debitParentAccountName)).Value == expectedDebitAccountBalance));

      await accountingDataAccess
        .Received(1)
        .UpdateAccountBalances(
          Arg.Is<IReadOnlyDictionary<string, decimal>>(balancesByAccount =>
            balancesByAccount.Single(x =>
              x.Key.Equals(creditParentAccountName)).Value == expectedCreditAccountBalance));
    }
  }
}

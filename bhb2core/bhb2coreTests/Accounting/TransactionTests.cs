using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using bhb2core.Accounting.Dto;
using bhb2core.Accounting.Interfaces;
using bhb2core.Accounting.Managers.AccountingManager;
using bhb2core.Accounting.Managers.AccountingManager.ActionResults;
using bhb2core.Accounting.Models;
using bhb2core.Common.ActionResults;

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
        .Returns(GetResult<IReadOnlyDictionary<string, Account>>.CreateSuccess(
          new Dictionary<string, Account>
          {
            { debitAccountName, debitAccount },
            { creditAccountName, creditAccount }
          }));

      accountingDataAccess
        .GetParentAccountsOrdered(debitAccountName)
        .Returns(GetResult<IEnumerable<Account>>.CreateSuccess(new Account[0]));

      accountingDataAccess
        .GetParentAccountsOrdered(creditAccountName)
        .Returns(GetResult<IEnumerable<Account>>.CreateSuccess(new Account[0]));

      accountingDataAccess
        .UpdateAccountBalances(default)
        .ReturnsForAnyArgs(ActionResult.CreateSuccess());

      accountingDataAccess
        .AddTransaction(default)
        .ReturnsForAnyArgs(ActionResult.CreateSuccess());

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
        .Returns(GetResult<IReadOnlyDictionary<string, Account>>.CreateSuccess(
          new Dictionary<string, Account>
          {
            { debitAccountName, debitAccount },
            { creditAccountName, creditAccount }
          }));

      accountingDataAccess
        .GetParentAccountsOrdered(debitAccountName)
        .Returns(GetResult<IEnumerable<Account>>.CreateSuccess(new Account[0]));

      accountingDataAccess
        .GetParentAccountsOrdered(creditAccountName)
        .Returns(GetResult<IEnumerable<Account>>.CreateSuccess(new Account[0]));

      accountingDataAccess
        .UpdateAccountBalances(default)
        .ReturnsForAnyArgs(ActionResult.CreateSuccess());

      accountingDataAccess
        .AddTransaction(default)
        .ReturnsForAnyArgs(ActionResult.CreateSuccess());

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
        .Returns(GetResult<IReadOnlyDictionary<string, Account>>.CreateSuccess(
          new Dictionary<string, Account>
          {
            { debitAccountName, debitAccount },
            { creditAccountName, creditAccount }
          }));

      accountingDataAccess
        .GetParentAccountsOrdered(debitAccountName)
        .Returns(GetResult<IEnumerable<Account>>.CreateSuccess(new Account[0]));

      accountingDataAccess
        .GetParentAccountsOrdered(creditAccountName)
        .Returns(GetResult<IEnumerable<Account>>.CreateSuccess(new Account[0]));

      accountingDataAccess
        .AddTransaction(default)
        .ReturnsForAnyArgs(ActionResult.CreateSuccess());

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
        .Returns(GetResult<IReadOnlyDictionary<string, Account>>.CreateSuccess(
          new Dictionary<string, Account>
          {
            { debitAccountName, debitAccount },
            { creditAccountName, creditAccount }
          }));

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

      accountingDataAccess
        .UpdateAccountBalances(default)
        .ReturnsForAnyArgs(ActionResult.CreateSuccess());

      accountingDataAccess
        .AddTransaction(default)
        .ReturnsForAnyArgs(ActionResult.CreateSuccess());

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
        .Returns(GetResult<IReadOnlyDictionary<string, Account>>.CreateSuccess(
          new Dictionary<string, Account>
          {
            { debitAccountName, debitAccount },
            { creditAccountName, creditAccount }
          }));

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

      accountingDataAccess
        .AddTransaction(default)
        .ReturnsForAnyArgs(ActionResult.CreateSuccess());

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

    [Test]
    public async Task Given_Transaction_When_AccountsShareParent_Then_ParentAccountBalanceShouldNotBeUpdated()
    {
      // Arrange.
      const string debitAccountName = "Bank";
      const string creditAccountName = "Cash";
      const string parentAccountName = "Funds";
      const decimal amount = 123.45m;

      AccountingManager testObject = AccountingManagerFactory.Create(out IAccountingDataAccess accountingDataAccess);

      AccountingManagerFactory.ConfigureDataAccessWithBaseAccounts(accountingDataAccess);

      Account parentAccount = accountingDataAccess.GetAccount(parentAccountName).Result.Result;

      var debitAccount = new Account
      {
        AccountType = AccountType.CreateFunds(),
        QualifiedName = debitAccountName,
        ParentAccountQualifiedName = parentAccountName,
        AccountTypesWithDebitPermission = parentAccount.AccountTypesWithDebitPermission,
        AccountTypesWithCreditPermission = parentAccount.AccountTypesWithCreditPermission
      };

      var creditAccount = new Account
      {
        AccountType = AccountType.CreateExpense(),
        QualifiedName = creditAccountName,
        ParentAccountQualifiedName = parentAccountName,
        AccountTypesWithDebitPermission = parentAccount.AccountTypesWithDebitPermission,
        AccountTypesWithCreditPermission = parentAccount.AccountTypesWithCreditPermission
      };

      accountingDataAccess
        .GetAccounts(Arg.Any<IEnumerable<string>>())
        .Returns(GetResult<IReadOnlyDictionary<string, Account>>.CreateSuccess(
          new Dictionary<string, Account>
          {
            { debitAccountName, debitAccount },
            { creditAccountName, creditAccount }
          }));

      accountingDataAccess
        .GetParentAccountsOrdered(debitAccountName)
        .Returns(GetResult<IEnumerable<Account>>.CreateSuccess(new[]
        {
          parentAccount
        }));

      accountingDataAccess
        .GetParentAccountsOrdered(creditAccountName)
        .Returns(GetResult<IEnumerable<Account>>.CreateSuccess(new[]
        {
          parentAccount
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
      await accountingDataAccess
        .Received(0)
        .UpdateAccountBalances(
          Arg.Is<IReadOnlyDictionary<string, decimal>>(balancesByAccount =>
            balancesByAccount.Any(x =>
              x.Key.Equals(parentAccountName))));
    }

    [Test]
    public async Task Given_TransactionWhichDebitsNonLeafAccount_When_Processed_Then_ResultIsFailure()
    {
      // Arrange.
      const string creditAccountName = "Food";
      const string debitParentAccountName = "Funds";
      const string creditParentAccountName = "Expense";
      const decimal amount = 123.45m;

      AccountingManager testObject = AccountingManagerFactory.Create(out IAccountingDataAccess accountingDataAccess);

      AccountingManagerFactory.ConfigureDataAccessWithBaseAccounts(accountingDataAccess);

      Account parentDebitAccount = accountingDataAccess.GetAccount(debitParentAccountName).Result.Result;
      Account parentCreditAccount = accountingDataAccess.GetAccount(creditParentAccountName).Result.Result;

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
        .Returns(GetResult<IReadOnlyDictionary<string, Account>>.CreateSuccess(
          new Dictionary<string, Account>
          {
            { debitParentAccountName, parentDebitAccount },
            { creditAccountName, creditAccount }
          }));

      accountingDataAccess
        .GetParentAccountsOrdered(debitParentAccountName)
        .Returns(GetResult<IEnumerable<Account>>.CreateSuccess(new Account[0]));

      accountingDataAccess
        .GetParentAccountsOrdered(creditAccountName)
        .Returns(GetResult<IEnumerable<Account>>.CreateSuccess(new[]
        {
          parentCreditAccount
        }));

      accountingDataAccess
        .IsParentAccount(debitParentAccountName)
        .Returns(true);

      var transaction = new TransactionDto
      {
        DebitAccountQualifiedName = debitParentAccountName,
        CreditAccountQualifiedName = creditAccountName,
        Amount = amount
      };

      // Act.
      ProcessTransactionResult result = await testObject.ProcessTransaction(transaction);

      // Assert.
      Assert.IsFalse(result.IsSuccess);
    }

    [Test]
    public async Task Given_TransactionWhichCreditsNonLeafAccount_When_Processed_Then_ResultIsFailure()
    {
      // Arrange.
      const string debitAccountName = "Cash";
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

      accountingDataAccess
        .GetAccounts(Arg.Any<IEnumerable<string>>())
        .Returns(GetResult<IReadOnlyDictionary<string, Account>>.CreateSuccess(
          new Dictionary<string, Account>
          {
            { debitAccountName, debitAccount },
            { creditParentAccountName, parentCreditAccount }
          }));

      accountingDataAccess
        .GetParentAccountsOrdered(debitAccountName)
        .Returns(GetResult<IEnumerable<Account>>.CreateSuccess(new[]
        {
          parentDebitAccount
        }));

      accountingDataAccess
        .GetParentAccountsOrdered(creditParentAccountName)
        .Returns(GetResult<IEnumerable<Account>>.CreateSuccess(new Account[0]));

      accountingDataAccess
        .IsParentAccount(creditParentAccountName)
        .Returns(true);

      var transaction = new TransactionDto
      {
        DebitAccountQualifiedName = debitAccountName,
        CreditAccountQualifiedName = creditParentAccountName,
        Amount = amount
      };

      // Act.
      ProcessTransactionResult result = await testObject.ProcessTransaction(transaction);

      // Assert.
      Assert.IsFalse(result.IsSuccess);
    }

    [Test]
    public async Task Given_Transaction_When_Processed_Then_WrittenToTransactionLog()
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
        .Returns(GetResult<IReadOnlyDictionary<string, Account>>.CreateSuccess(
          new Dictionary<string, Account>
          {
            { debitAccountName, debitAccount },
            { creditAccountName, creditAccount }
          }));

      accountingDataAccess
        .GetParentAccountsOrdered(debitAccountName)
        .Returns(GetResult<IEnumerable<Account>>.CreateSuccess(new Account[0]));

      accountingDataAccess
        .GetParentAccountsOrdered(creditAccountName)
        .Returns(GetResult<IEnumerable<Account>>.CreateSuccess(new Account[0]));

      accountingDataAccess
        .UpdateAccountBalances(default)
        .ReturnsForAnyArgs(ActionResult.CreateSuccess());

      accountingDataAccess
        .AddTransaction(default)
        .ReturnsForAnyArgs(ActionResult.CreateSuccess());

      var transaction = new TransactionDto
      {
        DebitAccountQualifiedName = debitAccountName,
        CreditAccountQualifiedName = creditAccountName,
        Amount = amount
      };

      // Act.
      await testObject.ProcessTransaction(transaction);

      // Assert.
      await accountingDataAccess
        .Received(1)
        .AddTransaction(Arg.Is<Transaction>(t =>
          t.DebitAccountQualifiedName.Equals(transaction.DebitAccountQualifiedName) &&
          t.CreditAccountQualifiedName.Equals(transaction.CreditAccountQualifiedName) &&
          t.Date == transaction.Date &&
          t.Amount == transaction.Amount));
    }

    [Test]
    public async Task Given_Transaction_When_WriteToTransactionLogFails_Then_ResultIsFailure()
    {
      // Arrange.
      const string debitAccountName = "Funds";
      const string creditAccountName = "Expense";
      const decimal amount = 123.45m;

      AccountingManager testObject = AccountingManagerFactory.Create(out IAccountingDataAccess accountingDataAccess);

      AccountingManagerFactory.ConfigureDataAccessWithBaseAccounts(accountingDataAccess);

      var transaction = new TransactionDto
      {
        DebitAccountQualifiedName = debitAccountName,
        CreditAccountQualifiedName = creditAccountName,
        Amount = amount
      };

      // Act.
      ProcessTransactionResult result = await testObject.ProcessTransaction(transaction);

      // Assert.
      Assert.IsFalse(result.IsSuccess);
    }

    [Test]
    public async Task Given_TransactionExists_When_SameIdempotencyIdReceived_Then_ResultIsFailure()
    {
      // Arrange.
      const string debitAccountName = "Funds";
      const string creditAccountName = "Expense";
      const decimal amount = 123.45m;

      var idemptotencyId = Guid.NewGuid();

      AccountingManager testObject = AccountingManagerFactory.Create(out IAccountingDataAccess accountingDataAccess);

      AccountingManagerFactory.ConfigureDataAccessWithBaseAccounts(accountingDataAccess);

      accountingDataAccess
        .DoesTransactionExist(idemptotencyId)
        .Returns(true);

      var transaction = new TransactionDto
      {
        IdempotencyId = idemptotencyId,
        DebitAccountQualifiedName = debitAccountName,
        CreditAccountQualifiedName = creditAccountName,
        Amount = amount
      };

      // Act.
      ProcessTransactionResult result = await testObject.ProcessTransaction(transaction);

      // Assert.
      Assert.IsFalse(result.IsSuccess);
      StringAssert.Contains("already exists", result.FailureMessage);
    }
  }
}

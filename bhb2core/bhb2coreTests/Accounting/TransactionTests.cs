using System.Threading.Tasks;

using bhb2core.Accounting.Dto;
using bhb2core.Accounting.Interfaces;
using bhb2core.Accounting.Managers.AccountingManager;
using bhb2core.Accounting.Models;

using bhb2coreTests.Accounting.TestUtils;

using NUnit.Framework;

namespace bhb2coreTests.Accounting
{
  [TestFixture]
  public class TransactionTests
  {
    [Test]
    public async Task Given_SufficientFunds_When_TransactionProcessed_Then_AccountBalancesAreUpdatedCorrectly()
    {
      // Arrange.
      AccountingManager testObject = AccountingManagerFactory.Create(
        out IAccountingDataAccess accountingDataAccess,
        useConcreteDataAccessMock: true);

      var debitAccount = new Account
      {
        Id = "Funds.Cash",
        Name = "Cash",
        Balance = 150m
      };

      var creditAccount = new Account
      {
        Id = "Expense.Vehicle.Petrol",
        Name = "Petrol",
        Balance = 0m
      };

      await accountingDataAccess.AddAccount(debitAccount);
      await accountingDataAccess.AddAccount(creditAccount);

      var transaction = new TransactionDto
      {
        DebitAccountId = debitAccount.Id,
        CreditAccountId = creditAccount.Id,
        Amount = 123.45m
      };

      decimal expectedDebitAccountBalance = debitAccount.Balance - transaction.Amount;
      decimal expectedCreditAccountBalance = creditAccount.Balance + transaction.Amount;

      // Act.
      await testObject.ProcessTransaction(transaction);

      // Assert.
      Assert.AreEqual(expectedDebitAccountBalance, debitAccount.Balance);
      Assert.AreEqual(expectedCreditAccountBalance, creditAccount.Balance);
    }
  }
}

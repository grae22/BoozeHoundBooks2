using System.Collections.Generic;
using System.Threading.Tasks;

using AutoMapper;

using bhb2core;
using bhb2core.Accounting;
using bhb2core.Accounting.Dto;
using bhb2core.Accounting.Interfaces;
using bhb2core.Accounting.Models;

using NSubstitute;

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
      Bhb2Core.Initialise(out IMapper mapper);

      var accountingDataAccess = Substitute.For<IAccountingDataAccess>();
      var transactionEngine = new TransactionEngine(accountingDataAccess);
      var testObject = new AccountingManager(transactionEngine, mapper);

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

      accountingDataAccess
        .GetAccountsById(Arg.Any<string[]>())
        .Returns(
          new Dictionary<string, Account>
          {
            { debitAccount.Id, debitAccount },
            { creditAccount.Id, creditAccount }
          });

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
      await accountingDataAccess
        .Received(1)
        .UpdateAccountBalances(
          Arg.Is<IReadOnlyDictionary<string, decimal>>(
            x => x[debitAccount.Id] == expectedDebitAccountBalance));

      await accountingDataAccess
        .Received(1)
        .UpdateAccountBalances(
          Arg.Is<IReadOnlyDictionary<string, decimal>>(
            x => x[creditAccount.Id] == expectedCreditAccountBalance));
    }
  }
}

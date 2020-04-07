using bhb2core;
using bhb2core.Accounting.Engines.AccountingEngine;
using bhb2core.Accounting.Interfaces;
using bhb2core.Accounting.Managers.AccountingManager;
using bhb2core.Accounting.Models;
using bhb2core.Common.ActionResults;
using bhb2core.Utils.Logging;
using bhb2core.Utils.Mapping;

using NSubstitute;

namespace bhb2coreTests.Accounting.TestUtils
{
  internal static class AccountingManagerFactory
  {
    private const string FundsAccountName = "Funds";
    private const string IncomeAccountName = "Income";
    private const string ExpenseAccountName = "Expense";
    private const string DebtorAccountName = "Debtor";
    private const string CreditorAccountName = "Creditor";

    public static AccountingManager Create(
      out IAccountingDataAccess accountingDataAccess,
      in bool performInitialisation = true)
    {
      Bhb2Core.Initialise(
        out ILogger logger,
        out IMapper mapper,
        out IAccountingManager ignoredAccountingManager);

      accountingDataAccess = Substitute.For<IAccountingDataAccess>();

      var transactionEngine = new AccountingEngine(accountingDataAccess, logger);

      var accountingManager = new AccountingManager(
        transactionEngine,
        accountingDataAccess,
        mapper,
        logger);

      if (performInitialisation)
      {
        accountingDataAccess
          .AddAccount(Arg.Is<Account>(a => a.Name.Equals(FundsAccountName)))
          .Returns(ActionResult.CreateSuccess());

        accountingDataAccess
          .AddAccount(Arg.Is<Account>(a => a.Name.Equals(IncomeAccountName)))
          .Returns(ActionResult.CreateSuccess());

        accountingDataAccess
          .AddAccount(Arg.Is<Account>(a => a.Name.Equals(ExpenseAccountName)))
          .Returns(ActionResult.CreateSuccess());

        accountingDataAccess
          .AddAccount(Arg.Is<Account>(a => a.Name.Equals(DebtorAccountName)))
          .Returns(ActionResult.CreateSuccess());

        accountingDataAccess
          .AddAccount(Arg.Is<Account>(a => a.Name.Equals(CreditorAccountName)))
          .Returns(ActionResult.CreateSuccess());

        accountingManager
          .Initialise()
          .GetAwaiter()
          .GetResult();

        ConfigureDataAccessWithBaseAccounts(accountingDataAccess);
      }

      return accountingManager;
    }

    public static void ConfigureDataAccessWithBaseAccounts(in IAccountingDataAccess dataAccess)
    {
      dataAccess
        .GetAccount(FundsAccountName)
        .Returns(
          GetResult<Account>.CreateSuccess(new Account
          {
            AccountType = AccountType.CreateFunds(),
            QualifiedName = FundsAccountName,
            Name = FundsAccountName,
            ParentAccountQualifiedName = null,
            AccountTypesWithDebitPermission = new[]
            {
              AccountType.CreateFunds(),
              AccountType.CreateExpense(),
              AccountType.CreateDebtor(),
              AccountType.CreateCreditor()
            },
            AccountTypesWithCreditPermission = new[]
            {
              AccountType.CreateFunds(),
              AccountType.CreateIncome(),
              AccountType.CreateDebtor(),
              AccountType.CreateCreditor()
            }
          }));

      dataAccess
        .GetAccount(IncomeAccountName)
        .Returns(
          GetResult<Account>.CreateSuccess(new Account
          {
            AccountType = AccountType.CreateIncome(),
            QualifiedName = IncomeAccountName,
            Name = IncomeAccountName,
            ParentAccountQualifiedName = null,
            AccountTypesWithDebitPermission = new[]
            {
              AccountType.CreateFunds()
            },
            AccountTypesWithCreditPermission = new AccountType[] { }
          }));

      dataAccess
        .GetAccount(ExpenseAccountName)
        .Returns(
          GetResult<Account>.CreateSuccess(new Account
          {
            AccountType = AccountType.CreateExpense(),
            QualifiedName = ExpenseAccountName,
            Name = ExpenseAccountName,
            ParentAccountQualifiedName = null,
            AccountTypesWithDebitPermission = new AccountType[] { },
            AccountTypesWithCreditPermission = new[]
            {
              AccountType.CreateFunds()
            }
          }));

      dataAccess
        .GetAccount(DebtorAccountName)
        .Returns(
          GetResult<Account>.CreateSuccess(new Account
          {
            AccountType = AccountType.CreateDebtor(),
            QualifiedName = DebtorAccountName,
            Name = DebtorAccountName,
            ParentAccountQualifiedName = null,
            AccountTypesWithDebitPermission = new[]
            {
              AccountType.CreateFunds()
            },
            AccountTypesWithCreditPermission = new[]
            {
              AccountType.CreateFunds()
            }
          }));

      dataAccess
        .GetAccount(CreditorAccountName)
        .Returns(
          GetResult<Account>.CreateSuccess(new Account
          {
            AccountType = AccountType.CreateCreditor(),
            QualifiedName = CreditorAccountName,
            Name = CreditorAccountName,
            ParentAccountQualifiedName = null,
            AccountTypesWithDebitPermission = new[]
            {
              AccountType.CreateFunds()
            },
            AccountTypesWithCreditPermission = new[]
            {
              AccountType.CreateFunds()
            }
          }));
    }
  }
}

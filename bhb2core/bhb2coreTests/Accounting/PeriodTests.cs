using System;
using System.Threading.Tasks;

using bhb2core.Accounting.Dto;
using bhb2core.Accounting.Interfaces;
using bhb2core.Accounting.Managers;
using bhb2core.Accounting.Models;
using bhb2core.Common.ActionResults;

using bhb2coreTests.Accounting.TestUtils;

using NSubstitute;

using NUnit.Framework;

namespace bhb2coreTests.Accounting
{
  [TestFixture]
  public class PeriodTests
  {
    [Test]
    public async Task Given_NoExistingPeriods_When_Added_Then_ResultIsSuccess()
    {
      // Arrange.
      AccountingManager testObject = AccountingManagerFactory.Create(out IAccountingDataAccess accountingDataAccess);

      accountingDataAccess
        .GetLastPeriod()
        .Returns(GetResult<Period>.CreateFailure("No periods found"));

      accountingDataAccess
        .AddPeriod(Arg.Any<Period>())
        .Returns(ActionResult.CreateSuccess());

      var period = new PeriodDto(DateTime.Now, DateTime.Now);

      // Act.
      ActionResult result = await testObject.AddPeriod(period);

      // Assert.
      Assert.IsTrue(result.IsSuccess);
    }

    [Test]
    public async Task Given_PeriodWithEndBeforeStart_When_Adding_Then_ResultIsFailure()
    {
      // Arrange.
      AccountingManager testObject = AccountingManagerFactory.Create(out IAccountingDataAccess accountingDataAccess);

      accountingDataAccess
        .GetLastPeriod()
        .Returns(GetResult<Period>.CreateSuccess(null));

      accountingDataAccess.AddPeriod(Arg.Any<Period>()).Returns(ActionResult.CreateSuccess());

      var period = new PeriodDto(DateTime.Now, DateTime.Now.AddDays(-1));

      // Act.
      ActionResult result = await testObject.AddPeriod(period);

      // Assert.
      Assert.IsFalse(result.IsSuccess);
    }

    [Test]
    public async Task Given_ExistingPeriods_When_NewPeriodDoesNotStartAfterLastExistingPeriod_Then_ResultIsFailure()
    {
      // Arrange.
      var lastPeriod = new Period(
        new DateTime(2020, 1, 1),
        new DateTime(2020, 1, 31));

      AccountingManager testObject = AccountingManagerFactory.Create(out IAccountingDataAccess accountingDataAccess);

      accountingDataAccess
        .GetLastPeriod()
        .Returns(GetResult<Period>.CreateSuccess(lastPeriod));

      var period = new PeriodDto(
        lastPeriod.End.AddDays(2),
        lastPeriod.End.AddDays(30));

      // Act.
      ActionResult result = await testObject.AddPeriod(period);

      // Assert.
      Assert.IsFalse(result.IsSuccess);
    }
  }
}

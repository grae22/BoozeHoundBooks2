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
    public void Given_StartingUp_When_NoPeriodsExist_Then_AddPeriodForCurrentMonth()
    {
      // Arrange.
      DateTime now = DateTime.Now;

      var expectedPeriodStart = new DateTime(
        now.Year,
        now.Month,
        1);

      var expectedPeriodEnd = new DateTime(
        now.Year,
        now.Month,
        DateTime.DaysInMonth(now.Year, now.Month));

      // Act.
      AccountingManagerFactory.Create(out IAccountingDataAccess accountingDataAccess);

      // Assert.
      accountingDataAccess
        .Received(1)
        .AddPeriod(Arg.Is<Period>(p =>
          p.Start == expectedPeriodStart &&
          p.End == expectedPeriodEnd));
    }

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

    [Test]
    public async Task Given_ExistingPeriodWhichIsLastPeriod_When_EndDateUpdated_Then_ResultIsSuccess()
    {
      // Arrange.
      var period = new Period(
        new DateTime(2020, 1, 1),
        new DateTime(2020, 1, 31));

      AccountingManager testObject = AccountingManagerFactory.Create(out IAccountingDataAccess accountingDataAccess);

      accountingDataAccess
        .GetLastPeriod()
        .Returns(GetResult<Period>.CreateSuccess(period));

      var updateEndDate = new UpdatePeriodEndDateDto(
        period.Start,
        period.End.AddDays(1));

      accountingDataAccess
        .UpdateLastPeriodEndDate(updateEndDate.NewEnd)
        .Returns(ActionResult.CreateSuccess());

      // Act.
      ActionResult result = await testObject.UpdatePeriodEndDate(updateEndDate);

      // Assert.
      Assert.IsTrue(result.IsSuccess);
    }

    [Test]
    public async Task Given_ExistingPeriodWhichIsLastPeriod_When_EndDateUpdatedWithDateNotInLastPeriod_Then_ResultIsFailure()
    {
      // Arrange.
      var period = new Period(
        new DateTime(2020, 1, 1),
        new DateTime(2020, 1, 31));

      AccountingManager testObject = AccountingManagerFactory.Create(out IAccountingDataAccess accountingDataAccess);

      accountingDataAccess
        .GetLastPeriod()
        .Returns(GetResult<Period>.CreateSuccess(period));

      var updateEndDate = new UpdatePeriodEndDateDto(
        period.Start.AddDays(-1),
        period.End.AddDays(1));

      accountingDataAccess
        .UpdateLastPeriodEndDate(updateEndDate.NewEnd)
        .Returns(ActionResult.CreateSuccess());

      // Act.
      ActionResult result = await testObject.UpdatePeriodEndDate(updateEndDate);

      // Assert.
      Assert.IsFalse(result.IsSuccess);
    }

    [Test]
    public async Task Given_ExistingPeriodWhichIsNotLastPeriod_When_EndDateUpdated_Then_ResultIsFailure()
    {
      // Arrange.
      var period = new Period(
        new DateTime(2020, 1, 1),
        new DateTime(2020, 1, 31));

      var lastPeriod = new Period(
        new DateTime(2020, 2, 1),
        new DateTime(2020, 2, 29));

      AccountingManager testObject = AccountingManagerFactory.Create(out IAccountingDataAccess accountingDataAccess);

      accountingDataAccess
        .GetLastPeriod()
        .Returns(GetResult<Period>.CreateSuccess(lastPeriod));

      var updateEndDate = new UpdatePeriodEndDateDto(
        period.Start,
        period.End.AddDays(1));

      accountingDataAccess
        .UpdateLastPeriodEndDate(updateEndDate.NewEnd)
        .Returns(ActionResult.CreateSuccess());

      // Act.
      ActionResult result = await testObject.UpdatePeriodEndDate(updateEndDate);

      // Assert.
      Assert.IsFalse(result.IsSuccess);
    }

    [Test]
    public async Task Given_ExistingLastPeriod_When_EndDateUpdatedWithDateBeforeStart_Then_ResultIsFailure()
    {
      // Arrange.
      var lastPeriod = new Period(
        new DateTime(2020, 2, 1),
        new DateTime(2020, 2, 29));

      AccountingManager testObject = AccountingManagerFactory.Create(out IAccountingDataAccess accountingDataAccess);

      accountingDataAccess
        .GetLastPeriod()
        .Returns(GetResult<Period>.CreateSuccess(lastPeriod));

      var updateEndDate = new UpdatePeriodEndDateDto(
        lastPeriod.Start,
        lastPeriod.Start.AddDays(-1));

      accountingDataAccess
        .UpdateLastPeriodEndDate(updateEndDate.NewEnd)
        .Returns(ActionResult.CreateSuccess());

      // Act.
      ActionResult result = await testObject.UpdatePeriodEndDate(updateEndDate);

      // Assert.
      Assert.IsFalse(result.IsSuccess);
    }
  }
}

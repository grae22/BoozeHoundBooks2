using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using bhb2core.Accounting.Dto;
using bhb2core.Accounting.Interfaces;
using bhb2core.Common.ActionResults;

using bhb2desktop.Extensions;

namespace bhb2desktop
{
  public partial class AddPeriodDialog : Form
  {
    private readonly IAccountingManager _accountingManager;
    private readonly SynchronizationContext _synchronizationContext;

    private PeriodDto _newPeriod;

    public AddPeriodDialog(IAccountingManager accountingManager)
    {
      _accountingManager = accountingManager ?? throw new ArgumentNullException(nameof(accountingManager));

      _synchronizationContext = SynchronizationContext.Current;

      InitializeComponent();
    }

    private void Dialog_OnLoad(object sender, EventArgs args)
    {
      Task.Run(async () => await PopulatePeriodDates());
    }

    private void OkButton_OnClick(object sender, EventArgs args)
    {
      Task.Run(async () => await AddPeriodAndCloseDialogOnSuccess());
    }

    private async Task PopulatePeriodDates()
    {
      GetResult<IEnumerable<PeriodDto>> getPeriodsResult = await _accountingManager.GetAllPeriods();

      if (!getPeriodsResult.IsSuccess)
      {
        this.ShowErrorMessage("Failed to retrieve periods.", getPeriodsResult.FailureMessage);
        Close();
        return;
      }

      PeriodDto lastPeriod = getPeriodsResult.Result.LastOrDefault();

      DateTime? periodStart = lastPeriod?.End.AddDays(1);

      if (!periodStart.HasValue)
      {
        DateTime now = DateTime.Now;

        periodStart = new DateTime(
          now.Year,
          now.Month,
          1);
      }

      DateTime? periodEnd = lastPeriod?.End.AddMonths(1);

      if (periodEnd.HasValue)
      {
        bool doesPreviousPeriodEndOnLastDayOfMonth =
          lastPeriod.End.Day == DateTime.DaysInMonth(lastPeriod.End.Year, lastPeriod.End.Month);

        if (doesPreviousPeriodEndOnLastDayOfMonth)
        {
          periodEnd = new DateTime(
            periodEnd.Value.Year,
            periodEnd.Value.Month,
            DateTime.DaysInMonth(periodEnd.Value.Year, periodEnd.Value.Month));
        }
      }
      else
      {
        DateTime now = DateTime.Now;

        periodEnd = new DateTime(
          now.Year,
          now.Month,
          DateTime.DaysInMonth(now.Year, now.Month));
      }

      _newPeriod = new PeriodDto(periodStart.Value, periodEnd.Value);

      _synchronizationContext.Post(
        p =>
        {
          var period = (PeriodDto)p;

          _startDateValueLabel.Text = $"{period.Start:yyyy-MM-dd}";
          _endDatePicker.Value = period.End;
        },
        _newPeriod);
    }

    private async Task AddPeriodAndCloseDialogOnSuccess()
    {
      ActionResult result = await _accountingManager.AddPeriod(_newPeriod);

      if (!result.IsSuccess)
      {
        this.ShowErrorMessage("Failed to add period.", result.FailureMessage);
        return;
      }

      _synchronizationContext.Post(
        x =>
        {
          DialogResult = DialogResult.OK;
          Close();
        },
        null);
    }
  }
}

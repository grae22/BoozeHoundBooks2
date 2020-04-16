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
  public partial class ChangeLastPeriodEndDateDialog : Form
  {
    private readonly IAccountingManager _accountingManager;
    private readonly SynchronizationContext _synchronizationContext;

    public ChangeLastPeriodEndDateDialog(IAccountingManager accountingManager)
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
      Task.Run(async () => await UpdateLastPeriodEndDateAndCloseDialogOnSuccess(_endDatePicker.Value));
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

      if (lastPeriod == null)
      {
        this.ShowErrorMessage("No periods found.");
        Close();
        return;
      }

      _synchronizationContext.Post(
        p =>
        {
          var period = (PeriodDto)p;

          _startDateValueLabel.Text = $"{period.Start:yyyy-MM-dd}";
          _endDatePicker.Value = period.End;
        },
        lastPeriod);
    }

    private async Task UpdateLastPeriodEndDateAndCloseDialogOnSuccess(DateTime newEnd)
    {
      var dto = new UpdateLastPeriodEndDateDto(newEnd);

      ActionResult result = await _accountingManager.UpdateLastPeriodEndDate(dto);

      if (!result.IsSuccess)
      {
        this.ShowErrorMessage("Failed to update last period's end date.", result.FailureMessage);
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

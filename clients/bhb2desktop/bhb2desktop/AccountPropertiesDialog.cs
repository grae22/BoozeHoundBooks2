using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using bhb2core.Accounting.Dto;
using bhb2core.Accounting.Interfaces;
using bhb2core.Accounting.Managers.AccountingManager.ActionResults;

using bhb2desktop.Extensions;

namespace bhb2desktop
{
  public partial class AccountPropertiesDialog : Form
  {
    private readonly IAccountingManager _accountingManager;

    public AccountPropertiesDialog(in IAccountingManager accountingManager)
    {
      _accountingManager = accountingManager ?? throw new ArgumentNullException(nameof(accountingManager));

      InitializeComponent();

      PopulateParentComboBox();
    }

    private void PopulateParentComboBox()
    {
      IEnumerable<AccountDto> accounts = _accountingManager.GetAllAccounts().Result;

      accounts
        .ToList()
        .ForEach(a => _parentComboBox.Items.Add(a.Id));

      if (_parentComboBox.Items.Count == 0)
      {
        this.ShowErrorMessage("No accounts found, cannot populate parents.");

        Close();
      }

      _parentComboBox.SelectedIndex = 0;
    }

    private void OkButton_OnClick(object sender, EventArgs args)
    {
      var account = new NewAccountDto
      {
        Name = _nameTextBox.Text,
        ParentAccountId = _parentComboBox.Text
      };

      AddAccountResult result = _accountingManager.AddAccount(account).Result;

      if (!result.IsSuccess)
      {
        this.ShowErrorMessage($"Failed to add the account.{Environment.NewLine}{Environment.NewLine}{result.FailureMessage}");
        return;
      }

      Close();
    }
  }
}

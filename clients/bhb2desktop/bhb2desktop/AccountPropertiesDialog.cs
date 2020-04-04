using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using bhb2core.Accounting.Dto;
using bhb2core.Accounting.Interfaces;

using bhb2desktop.Extensions;

namespace bhb2desktop
{
  public partial class AccountPropertiesDialog : Form
  {
    public AccountDto Account { get; private set; }

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
        .ForEach(a =>
          _parentComboBox.Items.Add(a.QualifiedName));

      if (_parentComboBox.Items.Count == 0)
      {
        this.ShowErrorMessage("No accounts found, cannot populate parents.");

        DialogResult = DialogResult.Cancel;

        Close();
      }

      _parentComboBox.SelectedIndex = 0;
    }

    private void OkButton_OnClick(object sender, EventArgs args)
    {
      Account = new AccountDto
      {
        Name = _nameTextBox.Text,
        ParentAccountQualifiedName = _parentComboBox.Text
      };

      DialogResult = DialogResult.OK;

      Close();
    }
  }
}

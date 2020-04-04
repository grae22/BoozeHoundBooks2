using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using bhb2core.Accounting.Dto;
using bhb2core.Accounting.Interfaces;

using bhb2desktop.Extensions;

namespace bhb2desktop
{
  public partial class TransactionPropertiesDialog : Form
  {
    public TransactionDto Transaction { get; private set; }

    private readonly IAccountingManager _accountingManager;

    public TransactionPropertiesDialog(IAccountingManager accountingManager)
    {
      _accountingManager = accountingManager ?? throw new ArgumentNullException(nameof(accountingManager));

      InitializeComponent();

      PopulateDebitAccountComboBox();
      PopulateCreditAccountComboBox();
    }

    private void PopulateDebitAccountComboBox()
    {
      IEnumerable<AccountDto> accounts = _accountingManager.GetTransactionDebitAccounts().Result;

      accounts
        .ToList()
        .ForEach(a =>
          _debitAccountComboBox.Items.Add(a.QualifiedName));

      if (_debitAccountComboBox.Items.Count == 0)
      {
        this.ShowErrorMessage("No accounts found, cannot populate debit accounts.");

        DialogResult = DialogResult.Cancel;

        Close();
      }

      _debitAccountComboBox.SelectedIndex = 0;
    }

    private void PopulateCreditAccountComboBox()
    {
      IEnumerable<AccountDto> accounts = _accountingManager.GetTransactionCreditAccounts().Result;

      accounts
        .ToList()
        .ForEach(a =>
          _creditAccountComboBox.Items.Add(a.QualifiedName));

      if (_creditAccountComboBox.Items.Count == 0)
      {
        this.ShowErrorMessage("No accounts found, cannot populate credit accounts.");

        DialogResult = DialogResult.Cancel;

        Close();
      }

      _creditAccountComboBox.SelectedIndex = 0;
    }

    private void OkButton_OnClick(object sender, EventArgs args)
    {
      Transaction = new TransactionDto
      {
        DebitAccountQualifiedName = _debitAccountComboBox.Text,
        CreditAccountQualifiedName = _creditAccountComboBox.Text,
        Date = _datePicker.Value,
        Amount = decimal.Parse(_amountTextBox.Text)
      };

      DialogResult = DialogResult.OK;

      Close();
    }
  }
}

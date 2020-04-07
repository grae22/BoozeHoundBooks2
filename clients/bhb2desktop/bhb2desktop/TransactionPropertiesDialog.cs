using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using bhb2core.Accounting.Dto;
using bhb2core.Accounting.Interfaces;
using bhb2core.Common.ActionResults;

using bhb2desktop.Extensions;

namespace bhb2desktop
{
  public partial class TransactionPropertiesDialog : Form
  {
    public TransactionDto Transaction { get; private set; }

    private readonly IAccountingManager _accountingManager;
    private IEnumerable<AccountDto> _debitAccounts;
    private IEnumerable<AccountDto> _creditAccounts;

    public TransactionPropertiesDialog(IAccountingManager accountingManager)
    {
      _accountingManager = accountingManager ?? throw new ArgumentNullException(nameof(accountingManager));

      InitializeComponent();
    }

    private void PopulateDebitAccountComboBox()
    {
      _debitAccountComboBox.Items.Clear();

      _debitAccounts
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
      _creditAccountComboBox.Items.Clear();

      AccountDto debitAccount =
        _debitAccounts
          .First(a =>
            a.QualifiedName.Equals(_debitAccountComboBox.Text, StringComparison.Ordinal));

      IEnumerable<AccountDto> accountsWithoutDebitPermission =
        _creditAccounts
          .ToList()
          .Where(a => !debitAccount.AccountTypesWithDebitPermission.Contains(a.AccountType));

      _creditAccounts.ToList()
        .ForEach(
          a =>
          {
            bool isNotDebitAccount = !a.QualifiedName.Equals(debitAccount.QualifiedName, StringComparison.Ordinal);
            bool hasPermissionToDebit = !accountsWithoutDebitPermission.Contains(a);

            if (isNotDebitAccount && hasPermissionToDebit)
            {
              _creditAccountComboBox.Items.Add(a.QualifiedName);
            }
          });

      if (_creditAccountComboBox.Items.Count == 0)
      {
        this.ShowErrorMessage("No accounts found, cannot populate credit accounts.");

        DialogResult = DialogResult.Cancel;

        Close();
      }

      _creditAccountComboBox.SelectedIndex = 0;
    }

    private void Dialog_OnLoad(object sender, EventArgs args)
    {
      // Get debit accounts.
      GetResult<IEnumerable<AccountDto>> getAccountsResult = _accountingManager.GetTransactionDebitAccounts().Result;

      if (!getAccountsResult.IsSuccess)
      {
        this.ShowErrorMessage("Failed to retrieve debit accounts.", getAccountsResult.FailureMessage);

        DialogResult = DialogResult.Cancel;
        Close();
      }

      _debitAccounts = getAccountsResult.Result;

      // Get credit accounts.
      getAccountsResult = _accountingManager.GetTransactionCreditAccounts().Result;

      if (!getAccountsResult.IsSuccess)
      {
        this.ShowErrorMessage("Failed to retrieve credit accounts.", getAccountsResult.FailureMessage);

        DialogResult = DialogResult.Cancel;
        Close();
      }

      _creditAccounts = getAccountsResult.Result;

      // Populate drop-downs.
      PopulateDebitAccountComboBox();
      PopulateCreditAccountComboBox();
    }

    private void DebitAccount_OnSelectedChangeCommitted(object sender, EventArgs args)
    {
      PopulateCreditAccountComboBox();
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

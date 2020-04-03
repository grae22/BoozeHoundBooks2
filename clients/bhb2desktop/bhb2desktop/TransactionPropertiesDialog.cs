﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using bhb2core.Accounting.Dto;
using bhb2core.Accounting.Interfaces;

using bhb2desktop.Extensions;
using bhb2desktop.Utils;

namespace bhb2desktop
{
  public partial class TransactionPropertiesDialog : Form
  {
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
          _debitAccountComboBox.Items.Add(
            AccountUtils.GetFullyQualifiedAccountName(
              a.Id,
              accounts,
              '.')));

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
          _creditAccountComboBox.Items.Add(
            AccountUtils.GetFullyQualifiedAccountName(
              a.Id,
              accounts,
              '.')));

      if (_creditAccountComboBox.Items.Count == 0)
      {
        this.ShowErrorMessage("No accounts found, cannot populate credit accounts.");

        DialogResult = DialogResult.Cancel;

        Close();
      }

      _creditAccountComboBox.SelectedIndex = 0;
    }
  }
}
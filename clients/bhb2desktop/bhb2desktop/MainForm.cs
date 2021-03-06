﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using bhb2core.Accounting.Dto;
using bhb2core.Accounting.Interfaces;
using bhb2core.Accounting.Managers.ActionResults;
using bhb2core.Common.ActionResults;
using bhb2core.Utils.Logging;

using bhb2desktop.Extensions;

namespace bhb2desktop
{
  public partial class MainForm : Form
  {
    private readonly SynchronizationContext _synchronizationContext;
    private readonly ILogger _logger;
    private readonly IAccountingManager _accountingManager;
    private readonly ConcurrentDictionary<string, TreeNode> _accountTreeNodesByAccountQualifiedName = new ConcurrentDictionary<string, TreeNode>();

    public MainForm(
      IAccountingManager accountingManager,
      ILogger logger)
    {
      _accountingManager = accountingManager ?? throw new ArgumentNullException(nameof(accountingManager));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      _synchronizationContext = SynchronizationContext.Current;

      InitializeComponent();
    }

    private async Task PopulateAccountsTree()
    {
      _logger.LogVerbose("Populating accounts tree...");

      GetResult<IEnumerable<AccountDto>> getAccountsResult = await _accountingManager.GetAllAccounts();

      if (!getAccountsResult.IsSuccess)
      {
        this.ShowErrorMessage("Failed to retrieve accounts.", getAccountsResult.FailureMessage);
        return;
      }

      IEnumerable<AccountDto> accounts = getAccountsResult.Result;

      _synchronizationContext.Post(
        accountsListAsObject =>
        {
          var accountsList = (List<AccountDto>)accountsListAsObject;

          _accountsTree.Nodes.Clear();
          _accountTreeNodesByAccountQualifiedName.Clear();

          while (_accountTreeNodesByAccountQualifiedName.Count < accountsList.Count)
          {
            foreach (var account in accountsList)
            {
              if (!account.HasParent)
              {
                TreeNode newBaseNode = _accountsTree.Nodes.Add(FormatAccountTreeNodeText(account));

                _accountTreeNodesByAccountQualifiedName.TryAdd(account.QualifiedName, newBaseNode);

                continue;
              }

              if (!_accountTreeNodesByAccountQualifiedName.TryGetValue(
                account.ParentAccountQualifiedName,
                out TreeNode parentNode))
              {
                _logger.LogError($"Failed to find parent account \"{account.ParentAccountQualifiedName}\".");
                continue;
              }

              TreeNode newNode = parentNode.Nodes.Add(FormatAccountTreeNodeText(account));

              _accountTreeNodesByAccountQualifiedName.TryAdd(account.QualifiedName, newNode);
            }
          }

          _accountsTree.ExpandAll();
        },
        accounts.ToList());
    }

    private void UpdateAccountTreeBalances(in IEnumerable<AccountDto> accounts)
    {
      _synchronizationContext.Post(
        accountsAsObject =>
        {
          foreach (var account in (IEnumerable<AccountDto>)accountsAsObject)
          {
            if (_accountTreeNodesByAccountQualifiedName.TryGetValue(account.QualifiedName, out TreeNode node))
            {
              node.Text = FormatAccountTreeNodeText(account);
            }
          }
        },
        accounts);
    }

    private async Task PopulateTransactionGrid()
    {
      _synchronizationContext.Post(
        tg =>
          ((DataGridView)tg).Rows.Clear(),
        _transactionGrid);

      GetResult<IEnumerable<TransactionDto>> getResult = await _accountingManager.GetTransactions();

      if (!getResult.IsSuccess)
      {
        this.ShowErrorMessage($"Failed to retrieve transactions: \"{getResult.FailureMessage}\".");
        return;
      }

      foreach (var transaction in getResult.Result)
      {
        AddTransactionToGrid(transaction);
      }
    }

    private void AddTransactionToGrid(in TransactionDto transaction)
    {
      _synchronizationContext.Post(
        t =>
        {
          var tran = (TransactionDto)t;

          _transactionGrid.Rows.Add(
            tran.Date,
            tran.IsCommitted,
            tran.Amount,
            string.Empty,
            tran.DebitAccountQualifiedName,
            tran.CreditAccountQualifiedName,
            string.Empty);
        },
        transaction);
    }

    private static string FormatAccountTreeNodeText(in AccountDto account)
    {
      return $"{account.Name}  ( {account.Balance:N} )";
    }

    private void Form_OnLoad(object sender, EventArgs args)
    {
      Task.Run(async () =>
      {
        await PopulateAccountsTree();
        await PopulateTransactionGrid();
      });
    }

    private void AddAccount_OnClick(object sender, EventArgs args)
    {
      using var dlg = new AccountPropertiesDialog(_accountingManager);

      dlg.ShowDialog(this);

      if (dlg.DialogResult != DialogResult.OK)
      {
        return;
      }

      var account = new NewAccountDto
      {
        Name = dlg.Account.Name,
        ParentAccountQualifiedName = dlg.Account.ParentAccountQualifiedName
      };

      Task.Run(async () =>
      {
        ActionResult result = _accountingManager.AddAccount(account).Result;

        if (!result.IsSuccess)
        {
          this.ShowErrorMessage(
            $"Failed to add the account.{Environment.NewLine}{Environment.NewLine}{result.FailureMessage}");

          //dlg.ShowDialog();

          return;
        }

        await PopulateAccountsTree();
      });
    }

    private void AddPeriod_OnClick(object sender, EventArgs args)
    {
      using var dlg = new AddPeriodDialog(_accountingManager);

      dlg.ShowDialog(this);
    }

    private void ChangeLastPeriodEndDate_OnClick(object sender, EventArgs args)
    {
      using var dlg = new ChangeLastPeriodEndDateDialog(_accountingManager);

      dlg.ShowDialog(this);
    }

    private void NewTransaction_OnClick(object sender, EventArgs args)
    {
      using var dlg = new TransactionPropertiesDialog(_accountingManager);

      dlg.ShowDialog();

      if (dlg.DialogResult != DialogResult.OK)
      {
        return;
      }

      Task.Run(async () =>
      {
        ProcessTransactionResult result = await _accountingManager.ProcessTransaction(dlg.Transaction);

        if (!result.IsSuccess)
        {
          this.ShowErrorMessage($"Failed to process transaction: \"{result.FailureMessage}\".");

          return;
        }

        UpdateAccountTreeBalances(result.UpdatedAccounts);
        AddTransactionToGrid(dlg.Transaction);
      });
    }
  }
}

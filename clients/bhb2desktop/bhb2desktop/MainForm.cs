using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using bhb2core;
using bhb2core.Accounting.Dto;
using bhb2core.Accounting.Interfaces;
using bhb2core.Utils.Logging;
using bhb2core.Utils.Mapping;

namespace bhb2desktop
{
  public partial class MainForm : Form
  {
    private readonly SynchronizationContext _synchronizationContext;
    private readonly ILogger _logger;
    private readonly IAccountingManager _accountingManager;

    public MainForm()
    {
      _synchronizationContext = SynchronizationContext.Current;

      InitializeComponent();

      Bhb2Core.Initialise(
        out _logger,
        out IMapper mapper,
        out _accountingManager);

      Task.Run(async () => await PopulateAccountsTree());
    }

    private async Task PopulateAccountsTree()
    {
      _logger.LogVerbose("Populating accounts tree...");

      IEnumerable<AccountDto> accounts = await _accountingManager.GetAllAccounts();

      _synchronizationContext.Post(
        accountsList =>
        {
          _accountsTree.Nodes.Clear();

          ((List<AccountDto>)accountsList)
            .ForEach(a =>
            {
              if (a.ParentAccountId == null)
              {
                _accountsTree.Nodes.Add($"{a.Name}  ( {a.Balance:N} )");
              }
              else
              {
                _accountsTree.Nodes.Add($"{a.ParentAccountId}.{a.Name}  ( {a.Balance:N} )");
              }
            });
        },
        accounts.ToList());
    }

    private void AddAccount_OnClick(object sender, EventArgs args)
    {
      using var dlg = new AccountPropertiesDialog(_accountingManager);

      dlg.ShowDialog(this);

      Task.Run(async () => await PopulateAccountsTree());
    }
  }
}

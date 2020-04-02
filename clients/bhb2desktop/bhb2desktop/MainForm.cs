using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using bhb2core.Accounting.Dto;
using bhb2core.Accounting.Interfaces;
using bhb2core.Utils.Logging;

namespace bhb2desktop
{
  public partial class MainForm : Form
  {
    private readonly SynchronizationContext _synchronizationContext;
    private readonly ILogger _logger;
    private readonly IAccountingManager _accountingManager;

    public MainForm(
      IAccountingManager accountingManager,
      ILogger logger)
    {
      _accountingManager = accountingManager ?? throw new ArgumentNullException(nameof(accountingManager));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      _synchronizationContext = SynchronizationContext.Current;

      InitializeComponent();

      Task.Run(async () => await PopulateAccountsTree());
    }

    private async Task PopulateAccountsTree()
    {
      _logger.LogVerbose("Populating accounts tree...");

      IEnumerable<AccountDto> accounts = await _accountingManager.GetAllAccounts();

      _synchronizationContext.Post(
        accountsListAsObject =>
        {
          var accountsList = (List<AccountDto>)accountsListAsObject;

          _accountsTree.Nodes.Clear();

          var nodesByAccountId = new Dictionary<string, TreeNode>();

          while (nodesByAccountId.Count < accountsList.Count)
          {
            foreach (var account in accountsList)
            {
              if (account.ParentAccountId == null)
              {
                TreeNode newBaseNode = _accountsTree.Nodes.Add($"{account.Name}  ( {account.Balance:N} )");

                nodesByAccountId.Add(account.Id, newBaseNode);

                continue;
              }

              if (!nodesByAccountId.TryGetValue(account.ParentAccountId, out TreeNode parentNode))
              {
                continue;
              }

              TreeNode newNode = parentNode.Nodes.Add($"{account.Name}  ( {account.Balance:N} )");

              nodesByAccountId.Add(account.Id, newNode);
            }
          }

          _accountsTree.ExpandAll();
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

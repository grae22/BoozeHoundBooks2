using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace bhb2desktop
{
  partial class MainForm
  {
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private IContainer _components = null;
    private MenuStrip _menuStrip;
    private ToolStripMenuItem _accountsMenuItem;
    private ToolStripMenuItem _addAccountMenuItem;
    private ToolStripMenuItem _periodsMenuItem;
    private ToolStripMenuItem _changeLastPeriodEndDateMenuItem;
    private ToolStripMenuItem _addPeriodMenuItem;
    private ToolStripMenuItem _newTransactionMenuItem;
    private Panel _mainPanel;
    private SplitContainer _mainSplitContainer;
    private TreeView _accountsTree;
    private DataGridView _transactionGrid;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (_components != null))
      {
        _components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      _components = new Container();

      AutoScaleMode = AutoScaleMode.Font;
      ClientSize = new Size(800, 450);
      Text = "Booze Hound Books 2";

      Load += Form_OnLoad;

      _menuStrip = new MenuStrip
      {
      };

      _accountsMenuItem = new ToolStripMenuItem
      {
        Text = "Accounts"
      };

      _addAccountMenuItem = new ToolStripMenuItem
      {
        Text = "Add"
      };

      _addAccountMenuItem.Click += AddAccount_OnClick;

      _accountsMenuItem.DropDownItems.Add(_addAccountMenuItem);

      _periodsMenuItem = new ToolStripMenuItem
      {
        Text = "Periods"
      };

      _addPeriodMenuItem = new ToolStripMenuItem
      {
        Text = "Add period"
      };

      _addPeriodMenuItem.Click += AddPeriod_OnClick;

      _changeLastPeriodEndDateMenuItem = new ToolStripMenuItem
      {
        Text = "Change last period end date"
      };

      _changeLastPeriodEndDateMenuItem.Click += ChangeLastPeriodEndDate_OnClick;

      _periodsMenuItem.DropDownItems.Add(_addPeriodMenuItem);
      _periodsMenuItem.DropDownItems.Add(_changeLastPeriodEndDateMenuItem);

      _newTransactionMenuItem = new ToolStripMenuItem
      {
        Text = "New Transaction",
        Alignment = ToolStripItemAlignment.Right
      };

      _newTransactionMenuItem.Click += NewTransaction_OnClick;

      _menuStrip.Items.Add(_accountsMenuItem);
      _menuStrip.Items.Add(_periodsMenuItem);
      _menuStrip.Items.Add(_newTransactionMenuItem);

      _mainPanel = new Panel
      {
        AutoSize = true,
        AutoSizeMode = AutoSizeMode.GrowAndShrink,
        Dock = DockStyle.Fill,
        Padding = new Padding(top: 30, bottom: 0, left: 0, right: 0)
      };

      _mainSplitContainer = new SplitContainer
      {
        AutoSize = true,
        Dock = DockStyle.Fill,
        Orientation = Orientation.Vertical
      };

      _accountsTree = new TreeView
      {
        Dock = DockStyle.Fill
      };

      _transactionGrid = new DataGridView
      {
        AutoSize = true,
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
        ColumnCount = 7,
        Dock = DockStyle.Fill,
        ReadOnly = true
      };

      _transactionGrid.Columns[0].Name = "Date";
      _transactionGrid.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
      _transactionGrid.Columns[0].DefaultCellStyle.Format = "yyyy-MM-dd";
      _transactionGrid.Columns[0].FillWeight = 0.1f;

      _transactionGrid.Columns[1].Name = "Committed";
      _transactionGrid.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
      _transactionGrid.Columns[1].FillWeight = 0.05f;

      _transactionGrid.Columns[2].Name = "Amount";
      _transactionGrid.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
      _transactionGrid.Columns[2].DefaultCellStyle.Format = "N";
      _transactionGrid.Columns[2].FillWeight = 0.10f;

      _transactionGrid.Columns[3].Name = "Allocations";
      _transactionGrid.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
      _transactionGrid.Columns[3].FillWeight = 0.15f;

      _transactionGrid.Columns[4].Name = "Debit Account";
      _transactionGrid.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
      _transactionGrid.Columns[4].FillWeight = 0.2f;

      _transactionGrid.Columns[5].Name = "Credit Account";
      _transactionGrid.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
      _transactionGrid.Columns[5].FillWeight = 0.2f;

      _transactionGrid.Columns[6].Name = "Description";
      _transactionGrid.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
      _transactionGrid.Columns[6].FillWeight = 0.2f;

      SuspendLayout();

      Controls.Add(_menuStrip);
      Controls.Add(_mainPanel);

      _mainPanel.Controls.Add(_mainSplitContainer);

      _mainSplitContainer.Panel1.Controls.Add(_accountsTree);
      _mainSplitContainer.Panel2.Controls.Add(_transactionGrid);

      ResumeLayout(true);
    }

    #endregion
  }
}


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
    private ToolStripMenuItem _accountMenuItem;
    private ToolStripMenuItem _addAccountMenuItem;
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

      _accountMenuItem = new ToolStripMenuItem
      {
        Text = "Accounts"
      };

      _addAccountMenuItem = new ToolStripMenuItem
      {
        Text = "Add"
      };

      _addAccountMenuItem.Click += AddAccount_OnClick;

      _accountMenuItem.DropDownItems.Add(_addAccountMenuItem);

      _newTransactionMenuItem = new ToolStripMenuItem
      {
        Text = "New Transaction",
        Alignment = ToolStripItemAlignment.Right
      };

      _newTransactionMenuItem.Click += NewTransaction_OnClick;

      _menuStrip.Items.Add(_accountMenuItem);
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
        ColumnCount = 7,
        Dock = DockStyle.Fill,
        ReadOnly = true
      };

      _transactionGrid.Columns[0].Name = "Date";
      _transactionGrid.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
      _transactionGrid.Columns[0].DefaultCellStyle.Format = "yyyy-MM-dd";

      _transactionGrid.Columns[1].Name = "Committed";
      _transactionGrid.Columns[1].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

      _transactionGrid.Columns[2].Name = "Amount";
      _transactionGrid.Columns[2].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
      _transactionGrid.Columns[2].DefaultCellStyle.Format = "N";

      _transactionGrid.Columns[3].Name = "Allocations";
      _transactionGrid.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

      _transactionGrid.Columns[4].Name = "Debit Account";
      _transactionGrid.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

      _transactionGrid.Columns[5].Name = "Credit Account";
      _transactionGrid.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

      _transactionGrid.Columns[6].Name = "Description";
      _transactionGrid.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

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


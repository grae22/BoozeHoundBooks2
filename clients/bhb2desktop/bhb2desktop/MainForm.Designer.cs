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
    private TreeView _accountsTree;

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

      _accountsTree = new TreeView
      {
        Dock = DockStyle.Fill
      };
      
      SuspendLayout();

      Controls.Add(_menuStrip);
      Controls.Add(_mainPanel);

      _mainPanel.Controls.Add(_accountsTree);

      ResumeLayout(true);
    }

    #endregion
  }
}


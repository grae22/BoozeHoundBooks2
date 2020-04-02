using System.ComponentModel;
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
    private ToolStripMenuItem _addAccountItem;
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
      _components = new System.ComponentModel.Container();

      AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      ClientSize = new System.Drawing.Size(800, 450);
      Text = "Booze Hound Books 2";

      _menuStrip = new System.Windows.Forms.MenuStrip
      {
      };

      _accountMenuItem = new System.Windows.Forms.ToolStripMenuItem
      {
        Text = "Accounts"
      };

      _addAccountItem = new System.Windows.Forms.ToolStripMenuItem
      {
        Text = "Add"
      };

      _addAccountItem.Click += AddAccount_OnClick;

      _accountMenuItem.DropDownItems.Add(_addAccountItem);

      _menuStrip.Items.Add(_accountMenuItem);
      _menuStrip.Items.Add(_accountMenuItem);

      _mainPanel = new System.Windows.Forms.Panel
      {
        AutoSize = true,
        AutoSizeMode = AutoSizeMode.GrowAndShrink,
        Dock = DockStyle.Fill,
        Padding = new Padding(top: 30, bottom: 0, left: 0, right: 0)
      };

      _accountsTree = new System.Windows.Forms.TreeView
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


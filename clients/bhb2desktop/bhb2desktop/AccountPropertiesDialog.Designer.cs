using System.Drawing;
using System.Windows.Forms;

namespace bhb2desktop
{
  partial class AccountPropertiesDialog
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;
    private System.Windows.Forms.TableLayoutPanel _mainPanel;
    private System.Windows.Forms.Label _nameLabel;
    private System.Windows.Forms.TextBox _nameTextBox;
    private System.Windows.Forms.Label _parentLabel;
    private System.Windows.Forms.ComboBox _parentComboBox;
    private System.Windows.Forms.FlowLayoutPanel _buttonsPanel;
    private System.Windows.Forms.Button _okButton;
    private System.Windows.Forms.Button _cancelButton;
    private System.Windows.Forms.Panel _dummyPanel;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(400, 400);
      this.Text = "Account";
      this.FormBorderStyle = FormBorderStyle.FixedDialog;
      this.AutoSize = true;
      this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.ControlBox = false;

      _mainPanel = new TableLayoutPanel
      {
        AutoSizeMode = AutoSizeMode.GrowAndShrink,
        AutoSize = true,
        ColumnCount = 2,
        Dock = DockStyle.Fill,
        Padding = new Padding(top: 5, bottom: 0, left: 0, right: 0)
      };

      _nameLabel = new Label
      {
        Text = "Name",
        TextAlign = ContentAlignment.MiddleLeft
      };

      _nameTextBox = new TextBox
      {
      };

      _parentLabel = new Label
      {
        Text = "Parent",
        TextAlign = ContentAlignment.MiddleLeft
      };

      _parentComboBox = new ComboBox
      {
        DropDownStyle = ComboBoxStyle.DropDownList,
        Sorted = true
      };

      _buttonsPanel = new FlowLayoutPanel
      {
        FlowDirection = FlowDirection.RightToLeft,
        AutoSize = true,
        AutoSizeMode = AutoSizeMode.GrowAndShrink,
        Dock = DockStyle.Fill,
        Padding = new Padding(top: 10, bottom: 0, left: 0, right: 0)
      };

      _okButton = new Button
      {
        Text = "OK"
      };

      _okButton.Click += OkButton_OnClick;

      _cancelButton = new Button
      {
        Text = "Cancel",
        DialogResult = DialogResult.Cancel
      };

      _dummyPanel = new Panel
      {
        AutoSize = true,
        AutoSizeMode = AutoSizeMode.GrowAndShrink
      };

      SuspendLayout();

      this.Controls.Add(_mainPanel);

      _mainPanel.Controls.Add(_nameLabel);
      _mainPanel.Controls.Add(_nameTextBox);

      _mainPanel.Controls.Add(_parentLabel);
      _mainPanel.Controls.Add(_parentComboBox);

      _buttonsPanel.Controls.Add(_okButton);
      _buttonsPanel.Controls.Add(_cancelButton);
      _mainPanel.Controls.Add(_dummyPanel);
      _mainPanel.Controls.Add(_buttonsPanel);

      ResumeLayout(true);
    }

    #endregion
  }
}
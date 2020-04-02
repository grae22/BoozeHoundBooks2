using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace bhb2desktop
{
  partial class TransactionPropertiesDialog
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private IContainer _components = null;
    private TableLayoutPanel _mainPanel;
    private Label _debitAccountLabel;
    private ComboBox _debitAccountComboBox;
    private Label _creditAccountLabel;
    private ComboBox _creditAccountComboBox;
    private Label _dateLabel;
    private DateTimePicker _datePicker;
    private Label _amountLabel;
    private TextBox _amountTextBox;
    private FlowLayoutPanel _buttonsPanel;
    private Button _okButton;
    private Button _cancelButton;
    private Panel _dummyPanel;

    /// <summary>
    /// Clean up any resources being used.
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
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      _components = new System.ComponentModel.Container();

      AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      ClientSize = new System.Drawing.Size(400, 400);
      Text = "Transaction";
      FormBorderStyle = FormBorderStyle.FixedDialog;
      AutoSize = true;
      AutoSizeMode = AutoSizeMode.GrowAndShrink;
      ControlBox = false;
      StartPosition = FormStartPosition.CenterParent;
      ShowInTaskbar = false;

      _mainPanel = new TableLayoutPanel
      {
        AutoSizeMode = AutoSizeMode.GrowAndShrink,
        AutoSize = true,
        ColumnCount = 2,
        Dock = DockStyle.Fill,
        Padding = new Padding(top: 5, bottom: 0, left: 0, right: 0)
      };

      _debitAccountLabel = new Label
      {
        Text = "Debit",
        TextAlign = ContentAlignment.MiddleLeft
      };

      _debitAccountComboBox = new ComboBox
      {
        DropDownStyle = ComboBoxStyle.DropDownList,
        Sorted = true
      };

      _creditAccountLabel = new Label
      {
        Text = "Credit",
        TextAlign = ContentAlignment.MiddleLeft
      };

      _creditAccountComboBox = new ComboBox
      {
        DropDownStyle = ComboBoxStyle.DropDownList,
        Sorted = true
      };

      _dateLabel = new Label
      {
        Text = "Date",
        TextAlign = ContentAlignment.MiddleLeft
      };

      _datePicker = new DateTimePicker
      {
        AutoSize = true,
        CustomFormat = "yyyy-MM-dd",
        Format = DateTimePickerFormat.Custom
      };

      _amountLabel = new Label
      {
        Text = "Amount",
        TextAlign = ContentAlignment.MiddleLeft
      };

      _amountTextBox = new TextBox
      {
        TextAlign = HorizontalAlignment.Center
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

      //_okButton.Click += OkButton_OnClick;

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

      _mainPanel.Controls.Add(_debitAccountLabel);
      _mainPanel.Controls.Add(_debitAccountComboBox);

      _mainPanel.Controls.Add(_creditAccountLabel);
      _mainPanel.Controls.Add(_creditAccountComboBox);

      _mainPanel.Controls.Add(_dateLabel);
      _mainPanel.Controls.Add(_datePicker);

      _mainPanel.Controls.Add(_amountLabel);
      _mainPanel.Controls.Add(_amountTextBox);

      _buttonsPanel.Controls.Add(_okButton);
      _buttonsPanel.Controls.Add(_cancelButton);
      _mainPanel.Controls.Add(_dummyPanel);
      _mainPanel.Controls.Add(_buttonsPanel);

      ResumeLayout(true);
    }

    #endregion
  }
}
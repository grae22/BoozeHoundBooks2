using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace bhb2desktop
{
  partial class ChangeLastPeriodEndDateDialog
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private IContainer _components = null;
    private TableLayoutPanel _mainPanel;
    private Label _startDateLabel;
    private Label _startDateValueLabel;
    private Label _endDateLabel;
    private DateTimePicker _endDatePicker;
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
      Text = "Last Period";
      FormBorderStyle = FormBorderStyle.FixedDialog;
      AutoSize = true;
      AutoSizeMode = AutoSizeMode.GrowAndShrink;
      ControlBox = false;
      StartPosition = FormStartPosition.CenterParent;
      ShowInTaskbar = false;

      Load += Dialog_OnLoad;

      _mainPanel = new TableLayoutPanel
      {
        AutoSizeMode = AutoSizeMode.GrowAndShrink,
        AutoSize = true,
        ColumnCount = 2,
        Dock = DockStyle.Fill,
        Padding = new Padding(top: 5, bottom: 0, left: 0, right: 0)
      };

      _startDateLabel = new Label
      {
        Text = "Start date",
        TextAlign = ContentAlignment.MiddleLeft
      };

      _startDateValueLabel = new Label
      {
        Text = string.Empty,
        TextAlign = ContentAlignment.MiddleLeft
      };

      _endDateLabel = new Label
      {
        Text = "End date",
        TextAlign = ContentAlignment.MiddleLeft
      };

      _endDatePicker = new DateTimePicker
      {
        AutoSize = true,
        CustomFormat = "yyyy-MM-dd",
        Format = DateTimePickerFormat.Custom
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

      CancelButton = _cancelButton;

      SuspendLayout();

      this.Controls.Add(_mainPanel);

      _mainPanel.Controls.Add(_startDateLabel);
      _mainPanel.Controls.Add(_startDateValueLabel);

      _mainPanel.Controls.Add(_endDateLabel);
      _mainPanel.Controls.Add(_endDatePicker);

      _buttonsPanel.Controls.Add(_okButton);
      _buttonsPanel.Controls.Add(_cancelButton);
      _mainPanel.Controls.Add(_dummyPanel);
      _mainPanel.Controls.Add(_buttonsPanel);

      ResumeLayout(true);
    }

    #endregion
  }
}
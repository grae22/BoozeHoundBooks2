using System;
using System.Windows.Forms;

namespace bhb2desktop.Extensions
{
  internal static class FormExtensions
  {
    public static void ShowErrorMessage(
      this Form form,
      string message)
    {
      MessageBox.Show(
        message,
        "Error",
        MessageBoxButtons.OK,
        MessageBoxIcon.Error);
    }

    public static void ShowErrorMessage(
      this Form form,
      string message,
      string details)
    {
      MessageBox.Show(
        message,
        $"Error{Environment.NewLine}{Environment.NewLine}{details}",
        MessageBoxButtons.OK,
        MessageBoxIcon.Error);
    }
  }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

using bhb2core;
using bhb2core.Common.ActionResults;
using bhb2core.Utils.Configuration;

namespace bhb2desktop
{
  static class Program
  {
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    public static async Task Main()
    {
      Application.SetHighDpiMode(HighDpiMode.SystemAware);
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);

      IConfiguration configuration = ConfigurationBuilder.Build(
        new Dictionary<string, string>
        {
          { "dataFilename", @"c:\dev\prj\other\boozehoundbooks2\clients\bhb2desktop\bhb2desktop\bin\test.bhb2" }
        });

      ActionResult result = await Bhb2Core.Initialise(configuration);

      if (!result.IsSuccess)
      {
        MessageBox.Show(
          $"Core initialisation failed.{Environment.NewLine}{Environment.NewLine}{result.FailureMessage}",
          "Error",
          MessageBoxButtons.OK,
          MessageBoxIcon.Error);

        return;
      }

      Application.Run(
        new MainForm(
          Bhb2Core.AccountingManager,
          Bhb2Core.Logger));
    }
  }
}

using System;
using System.Windows.Forms;

using bhb2core;
using bhb2core.Accounting.Interfaces;
using bhb2core.Utils.Logging;
using bhb2core.Utils.Mapping;

namespace bhb2desktop
{
  static class Program
  {
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      Bhb2Core.Initialise(
        out ILogger _logger,
        out IMapper mapper,
        out IAccountingManager _accountingManager);

      Application.SetHighDpiMode(HighDpiMode.SystemAware);
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new MainForm(_accountingManager, _logger));
    }
  }
}

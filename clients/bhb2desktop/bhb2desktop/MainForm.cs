using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

using bhb2core;
using bhb2core.Accounting.Dto;
using bhb2core.Accounting.Interfaces;
using bhb2core.Utils.Logging;
using bhb2core.Utils.Mapping;

namespace bhb2desktop
{
  public partial class MainForm : Form
  {
    public MainForm()
    {
      InitializeComponent();

      Bhb2Core.Initialise(
        out ILogger logger,
        out IMapper mapper,
        out IAccountingManager accountingManager);

      IEnumerable<AccountDto> accounts =
        accountingManager
          .GetAllAccounts()
          .GetAwaiter()
          .GetResult();

      accounts
        .ToList()
        .ForEach(a => Debug.WriteLine(a.Id));
    }
  }
}

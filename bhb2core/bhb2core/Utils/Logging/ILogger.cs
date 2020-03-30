using System;
using System.Runtime.CompilerServices;

namespace bhb2core.Utils.Logging
{
  public interface ILogger
  {
    void LogVerbose(
      in string message,
      [CallerMemberName] in string callerMemberName = "",
      [CallerFilePath] in string callerFilePath = "");

    void LogInformation(
      in string message,
      [CallerMemberName] in string callerMemberName = "",
      [CallerFilePath] in string callerFilePath = "");

    void LogWarning(
      in string message,
      in Exception exception = null,
      [CallerMemberName] in string callerMemberName = "",
      [CallerFilePath] in string callerFilePath = "");

    void LogError(
      in string message,
      in Exception exception = null,
      [CallerMemberName] in string callerMemberName = "",
      [CallerFilePath] in string callerFilePath = "");
  }
}

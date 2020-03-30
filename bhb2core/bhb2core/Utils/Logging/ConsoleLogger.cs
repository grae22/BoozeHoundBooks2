using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace bhb2core.Utils.Logging
{
  internal class ConsoleLogger : ILogger
  {
    public void LogVerbose(
      in string message,
      [CallerMemberName] in string callerMemberName = "",
      [CallerFilePath] in string callerFilePath = "")
    {
      Debug.WriteLine(
        BuildLogMessage(
          "Verbose",
          message,
          callerMemberName,
          callerFilePath,
          null));
    }

    public void LogInformation(
      in string message,
      [CallerMemberName] in string callerMemberName = "",
      [CallerFilePath] in string callerFilePath = "")
    {
      Debug.WriteLine(
        BuildLogMessage(
          "Info",
          message,
          callerMemberName,
          callerFilePath,
          null));
    }

    public void LogWarning(
      in string message,
      in Exception exception = null,
      [CallerMemberName] in string callerMemberName = "",
      [CallerFilePath] in string callerFilePath = "")
    {
      Debug.WriteLine(
        BuildLogMessage(
          "Warning",
          message,
          callerMemberName,
          callerFilePath,
          exception));
    }

    public void LogError(
      in string message,
      in Exception exception = null,
      [CallerMemberName] in string callerMemberName = "",
      [CallerFilePath] in string callerFilePath = "")
    {
      Debug.WriteLine(
        BuildLogMessage(
          "Error",
          message,
          callerMemberName,
          callerFilePath,
          exception));
    }

    private static string BuildLogMessage(
      in string type,
      in string message,
      in string callerMemberName,
      in string callerFilePath,
      in Exception exception)
    {
      string filename = Path.GetFileName(callerFilePath);
      string exceptionDetails = string.Empty;

      if (exception != null)
      {
        exceptionDetails =
          $"_{Environment.NewLine}Exception: \"{exception.Message}\"{Environment.NewLine}{exception.StackTrace}";
      }

      return $"{DateTime.Now:s} | {type,-7} | {message} | {callerMemberName} | {filename}{exceptionDetails}";
    }
  }
}

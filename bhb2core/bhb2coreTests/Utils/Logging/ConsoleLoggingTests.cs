using System;

using bhb2core.Utils.Logging;

using NUnit.Framework;

namespace bhb2coreTests.Utils.Logging
{
  [TestFixture]
  public class ConsoleLoggingTests
  {
    [Test]
    public void Given_Logging_When_Logged_Then_DetailsLoggedInCorrectFormat()
    {
      // Arrange.
      var testObject = new ConsoleLogger();

      // Act.
      testObject.LogVerbose("Testing 123...");
      testObject.LogInformation("Testing 123...");
      testObject.LogWarning("Testing 123...", new Exception("SomeException"));
      testObject.LogError("Testing 123...", new Exception("SomeException"));

      // Assert.
      // View the debug output.
    }
  }
}

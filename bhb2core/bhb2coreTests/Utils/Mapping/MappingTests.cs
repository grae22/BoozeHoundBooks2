using bhb2core.Utils.Logging;
using bhb2core.Utils.Mapping;

using NSubstitute;

using NUnit.Framework;

namespace bhb2coreTests.Utils.Mapping
{
  [TestFixture]
  public class MappingTests
  {
    [Test]
    public void Given_NoMapper_When_MapperCreated_Then_MapperCreatedSuccessfully()
    {
      // Arrange.
      var logger = Substitute.For<ILogger>();

      // Act.
      Mapper.CreateAndInitialiseMappings(logger);

      // Assert.
      Mapper.VerifyConfiguration();
    }
  }
}

using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GameOfLife.UnitTest;

public class AppModuleTest
{
    [Fact]
    public void AddApplicationDependencies_ShouldRegisterDependencies()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ConnectionStrings:Mongo", "mongodb://localhost:27017" } //the driver does not try connect at initialization :)
            })
            .Build();

        // Act
        var result = AppModule.AddApplicationDependencies(services, configuration);

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
    }
}

using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using FluentAssertions;
using GameOfLife.Persistence.Mapping;

namespace GameOfLife.UnitTest.Persistence.Mapping;

public class LowerCaseElementNameConventionTest
{
    [Fact]
    public void Apply_ShouldConvertElementNameToLowerCase()
    {
        // Arrange
        var convention = new LowerCaseElementNameConvention();
        var classMap = new BsonClassMap<TestClass>();
        var memberMap = new BsonMemberMap(classMap, typeof(TestClass).GetProperty(nameof(TestClass.TestProperty)));
        memberMap.SetElementName("TestProperty");

        // Act
        convention.Apply(memberMap);

        // Assert
        memberMap.ElementName.Should().Be("testProperty");
    }

    [Fact]
    public void Enable_ShouldRegisterConvention()
    {
        // Arrange
        var conventionName = "LowerCaseElementName";

        // Act
        LowerCaseElementNameConvention.Enable();

        // Assert
        var registeredConventions = ConventionRegistry.Lookup(typeof(object));
        registeredConventions.Conventions.Should().ContainSingle(c => c.Name == conventionName);
    }

    private class TestClass
    {
        public string TestProperty { get; set; }
    }
}

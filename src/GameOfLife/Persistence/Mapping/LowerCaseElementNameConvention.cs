using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;

namespace GameOfLife.Persistence.Mapping;

/// <summary>
/// A convention that converts the member names of a class to lower case when serializing to BSON.
/// </summary>
public class LowerCaseElementNameConvention : IMemberMapConvention, IEnableableConfiguration
{
    private const string ConventionName = "LowerCaseElementName";

    public string Name => ConventionName;

    public void Apply(BsonMemberMap memberMap)
    {
        var elementName = string.Create(memberMap.ElementName.Length, memberMap.ElementName, (chars, state) =>
        {
            chars[0] = char.ToLowerInvariant(state[0]);

            for (var i = 1; i < state.Length; i++)
                chars[i] = state[i];
        });
        
        memberMap.SetElementName(elementName);
    }

    public static void Enable()
    {
        var pack = new ConventionPack
        {
            new LowerCaseElementNameConvention()
        };

        ConventionRegistry.Register(ConventionName, pack, t => true);
    }
}

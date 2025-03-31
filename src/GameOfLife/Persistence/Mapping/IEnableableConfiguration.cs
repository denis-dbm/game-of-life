namespace GameOfLife.Persistence.Mapping;

/// <summary>
/// Marks configurations (conventions, serializers, and so on) that can be enabled on the MongoDB Driver.
/// </summary>
public interface IEnableableConfiguration
{
    static abstract void Enable();
}

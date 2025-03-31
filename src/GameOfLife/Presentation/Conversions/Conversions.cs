using System.Diagnostics.CodeAnalysis;

namespace GameOfLife.Presentation.Conversions;

/// <summary>
/// This interface is responsible for converting between the presentation model and the domain model.
/// </summary>
/// <typeparam name="TPresentation"></typeparam>
/// <typeparam name="TModel"></typeparam>
public interface IInputConverter<in TPresentation, TModel>
{
    bool TryInput(TPresentation presentation, [NotNullWhen(true)]out TModel? model, [NotNullWhen(false)]out string? errorMessage);
}

/// <summary>
/// This interface is responsible for converting between the domain model and the presentation model.
/// </summary>
/// <typeparam name="TModel"></typeparam>
/// <typeparam name="TPresentation"></typeparam>
public interface IOutputConverter<in TModel, out TPresentation>
{
    TPresentation View(TModel model);
}
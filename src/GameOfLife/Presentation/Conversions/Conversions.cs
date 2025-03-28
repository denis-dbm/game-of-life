using System.Diagnostics.CodeAnalysis;

namespace GameOfLife.Presentation.Conversions;

public interface IInputConverter<in TPresentation, TModel>
{
    bool TryInput(TPresentation presentation, [NotNullWhen(true)]out TModel? model, [NotNullWhen(false)]out string? errorMessage);
}

public interface IOutputConverter<in TModel, out TPresentation>
{
    TPresentation View(TModel model);
}
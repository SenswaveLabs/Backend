namespace Senswave.Devices.Application.Widgets.Features.CreateWidget;

public class CreateWidgetErrors
{
    public static Error OperationNotFound =>
        Error.NotFound("OperationNotFound", "Operation with provided id not found");

    public static Error WidgetNameAlreadyUsed =>
        Error.Conflict("WidgetNameAlreadyUsed", "Widget with provided name has already exists for operation");

    public static Error FailedToSaveWidget =>
        Error.Conflict("FailedToSaveWidget", "Failed to save widget to database.");
}
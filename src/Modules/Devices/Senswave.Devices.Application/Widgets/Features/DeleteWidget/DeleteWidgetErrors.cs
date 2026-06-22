namespace Senswave.Devices.Application.Widgets.Features.DeleteWidget;

public static class DeleteWidgetErrors
{
    public static Error NoAccess => Error.Failure("NoAccess", "User does not have access to delete widget");
    public static Error FailedToRemoveWidget => Error.NotFound("FailedToRemoveWidget", "Failed to remove widget.");
}
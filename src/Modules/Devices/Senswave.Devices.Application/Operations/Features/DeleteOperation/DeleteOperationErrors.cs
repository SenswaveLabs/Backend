namespace Senswave.Devices.Application.Operations.Features.DeleteOperation;

public static class DeleteOperationErrors
{
    public static Error OperationNotFound =>
        Error.NotFound("OperationNotFound", "Operation with provided Id not found in the database");

    public static Error WidgetConflict =>
        Error.Conflict("WidgetConflict", "Can not delete operation because there is widget that uses it");

    public static Error DeviceTileConfilict =>
        Error.Conflict("DeviceTileConfilict", "Can not delete operation because there is device tile that uses it.");
}
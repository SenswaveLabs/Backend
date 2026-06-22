namespace Senswave.Devices.Domain.Widgets.Types.Base;

internal class BaseWidgetErrors
{
    internal static Error OperationTypeNotSupported = Error.Validation("OperationTypeNotSupported", "Operation type is not supported by this widget.");

    internal static Error WidgetIsNotEnabled = Error.Validation("WidgetIsNotEnabled", "Widget is not enabled for making actions.");

    internal static Error ValueIsNotCompliantWithOperation = Error.Validation("ValueNotCompliant", "Provided value is not compliant with operation.");

    internal static Error ActionIsNotSupported = Error.Failure("ActionIsNotSupported", "This widget can not execute any action.");
}

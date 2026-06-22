using Senswave.Devices.Domain.Operations.Enums;
using Senswave.Devices.Domain.Operations.Types;
using Senswave.Devices.Domain.Widgets.Entities;
using Senswave.Devices.Domain.Widgets.Models;

namespace Senswave.Devices.Domain.Widgets.Types;

public interface IWidget
{
    /// <summary>
    /// Operation instance for accessing operation related data and methods.
    /// </summary>
    IOperation Operation { get; }

    /// <summary>
    /// List of allowed Operation Types for this widget.
    /// </summary>
    List<OperationType> AllowedOperationTypes { get; }

    /// <summary>
    /// Validates widget and configuration.
    /// </summary>
    /// <returns></returns>
    Task<Result> Validate();

    /// <summary>
    /// Method to preprocess incoming value.
    /// </summary>
    /// <param name="incomingValue"></param>
    /// <returns></returns>
    Result<JsonValue> PreprocessAction(JsonValue incomingValue);

    /// <summary>
    /// Method to convert the widget to a <see cref="DisplayWidgetModel"/> for display purposes.
    /// </summary>
    /// <returns></returns>
    Task<DisplayWidgetModel> ToDisplay();

    /// <summary>
    /// Method to convert the widget to a <see cref="Widget"/> entity.
    /// Important! This removes unwanted fields in json configuration!.
    /// </summary>
    /// <returns> Widget Entity </returns>
    Widget AsWidget();
}

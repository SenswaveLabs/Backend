using FluentValidation;
using Senswave.Devices.Domain.Operations.Enums;
using Senswave.Devices.Domain.Operations.Extensions;
using Senswave.Devices.Domain.Operations.Types;
using Senswave.Devices.Domain.Widgets.Entities;
using Senswave.Devices.Domain.Widgets.Enums;
using Senswave.Devices.Domain.Widgets.Extensions;
using Senswave.Devices.Domain.Widgets.Models;

namespace Senswave.Devices.Domain.Widgets.Types.Base;

public abstract class BaseWidget(Widget widget, IOperation operation) : IWidget
{
    public IOperation Operation => operation;

    public Guid Id => widget.Id;

    public string Name => widget.Name;

    public WidgetType Type => widget.Type;

    public bool Enabled => widget.Enabled;

    public DateTime CreatedAtUtc => widget.CreatedAtUtc;

    public DateTime UpdatedAtUtc => widget.UpdatedAtUtc;

    public abstract List<OperationType> AllowedOperationTypes { get; }

    public abstract IWidgetConfiguration Configuration { get; }

    public abstract Task<Result> Validate();

    public abstract Result<JsonValue> PreprocessAction(JsonValue incomingValue);

    public virtual Task<DisplayWidgetModel> ToDisplay() => Task.FromResult(new DisplayWidgetModel()
    {
        Id = Id,
        Name = Name,
        Enabled = Enabled,
        Type = Type.FromWidgetType(),
        Configuration = [],
        CreatedAtUtc = CreatedAtUtc,
        UpdatedAtUtc = UpdatedAtUtc,
    });

    public virtual Widget AsWidget() => widget;

    #region Protected

    protected async Task<Result> ValidateInternal<T, TValidator>()
       where TValidator : AbstractValidator<T>, new()
       where T : BaseWidget
    {
        if (!AllowedOperationTypes.Contains(Operation.Type))
            return Result.Failure(BaseWidgetErrors.OperationTypeNotSupported);

        var validator = new TValidator();

        var validationResult = await validator.ValidateAsync((T)this);

        if (!validationResult.IsValid)
            return validationResult.ToResult();

        return Result.Success();
    }

    #endregion
}

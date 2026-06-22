using Senswave.Devices.Domain.Dashboards.Entities;
using Senswave.Devices.Domain.Dashboards.Models;
using Senswave.Devices.Domain.Operations.Extensions;
using Senswave.Devices.Domain.Widgets.Factory;
using System.Text.Json;

namespace Senswave.Devices.Domain.Dashboards.Types.Gird;

public class GridDashboard(
    Dashboard dashboard,
    GridDashboardConfiguration configuration,
    WidgetFactory widgetFactory,
    bool dashboardInitialization) : BaseDashboard(dashboard), IDashboard
{
    public GridDashboardConfiguration Configuration => configuration;

    public override async Task<Result<DisplayDashboardModel>> ToDisplay(CancellationToken cancellationToken)
    {
        var widgetIds = Configuration.PositionedWidgets.Select(pw => pw.WidgetId).ToList();

        var widgets = await widgetFactory.Create(widgetIds, cancellationToken);

        var displayWidgets = await Task.WhenAll(widgets.Data.Select(w => w.ToDisplay()));

        var calculatedWidgets = JsonNode.Parse(JsonSerializer.Serialize(displayWidgets))?.AsArray() ?? [];

        var positionetWidgets = JsonNode.Parse(JsonSerializer.Serialize(Configuration.PositionedWidgets))?.AsArray() ?? [];

        var configuration = new JsonObject()
        {
            ["rows"] = Configuration.Rows,
            ["columns"] = Configuration.Columns,
            ["positionedWidgets"] = positionetWidgets,
            ["calculatedWidgets"] = calculatedWidgets
        };

        var displayModel = new DisplayDashboardModel
        {
            Id = Id,
            Name = Name,
            Type = Type,
            Configuration = configuration,
        };

        return Result<DisplayDashboardModel>.Success(displayModel);
    }

    public override Dashboard ToDashboard()
    {
        var widgets = Configuration.PositionedWidgets.Select(pw => new JsonObject
        {
            ["widgetId"] = pw.WidgetId,
            ["row"] = pw.Row,
            ["column"] = pw.Column,
            ["rowSpan"] = pw.RowSpan,
            ["columnSpan"] = pw.ColumnSpan
        });

        var widgetArray = JsonArray.Parse(JsonSerializer.Serialize(widgets));

        var data = new JsonObject
        {
            ["rows"] = Configuration.Rows,
            ["columns"] = Configuration.Columns,
            ["positionedWidgets"] = widgetArray
        };

        _dashboard.Configuration = data;

        return _dashboard;
    }

    public override async Task<Result> Validate()
    {
        var validator = new GridDashboardValidator();

        var result = await validator.ValidateAsync(this);

        if (!result.IsValid)
            return result.ToResult();

        if (dashboardInitialization)
        {
            if (Configuration.PositionedWidgets.Count != 0)
                return Result.Failure(Error.Validation("PositionedWidgetsShouldBeEmpty"));
        }
        else
        {
            if (Configuration.PositionedWidgets.Count > 0)
            {
                var validationResult = ValidateGrid(Configuration.Rows, Configuration.Columns, Configuration.PositionedWidgets);

                if (!validationResult.IsSuccess)
                    return Result.Failure(validationResult.Errors);
            }
        }

        return Result.Success();
    }

    public override Result AddWidget(PositionedWidget widget)
    {
        if (Configuration.PositionedWidgets.Any(x => x.WidgetId == widget.WidgetId))
            return Result.Failure(Error.Conflict("WidgetAlreadyFoundOnDasboard", "Widget is already placed on this dashboard."));

        Configuration.PositionedWidgets.Add(widget);

        var validation = ValidateGrid(configuration.Rows, configuration.Columns, configuration.PositionedWidgets);

        if (validation.IsFailure)
            return Result.Failure(Error.Validation("FailedToSetDashboard"), validation.Errors);

        return Result.Success();
    }

    public override Result RemoveWidget(Guid widgetId)
    {
        if (!Configuration.PositionedWidgets.Any(x => x.WidgetId == widgetId))
            return Result.Failure(Error.Failure("WidgetNotFoundOnDashboard", "Widget was not found on this dashboard."));

        Configuration.PositionedWidgets.RemoveAll(x => x.WidgetId == widgetId);

        return Result.Success();
    }

    #region Private

    private static Result ValidateGrid(int rows, int columns, List<PositionedWidget> widgets)
    {
        bool[,] grid = new bool[rows, columns];

        foreach (var widget in widgets)
        {
            int startRow = widget.Row;
            int endRow = widget.Row + widget.RowSpan - 1;
            int startColumn = widget.Column;
            int endColumn = widget.Column + widget.ColumnSpan - 1;

            if (startRow < 0 || endRow >= rows || startColumn < 0 || endColumn >= columns)
                return Result.Failure(Error.Failure("WidgetOutOfDashboardBonds", "Widget position is outside dashboard boundaries."));

            for (int i = startRow; i <= endRow; i++)
            {
                for (int j = startColumn; j <= endColumn; j++)
                {
                    if (grid[i, j])
                    {
                        return Result.Failure(Error.Failure("OverlappingWidget", "Widget position overlaps with an existing widget."));
                    }

                    grid[i, j] = true;
                }
            }
        }

        return Result.Success();
    }

    #endregion
}

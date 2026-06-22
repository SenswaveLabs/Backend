using Senswave.Devices.Domain.Dashboards.Entities;
using Senswave.Devices.Domain.Dashboards.Factories;
using Senswave.Devices.Domain.Dashboards.Repositories;
using Senswave.Devices.Domain.Dashboards.Services;
using Senswave.Devices.Domain.Devices.Options;

namespace Senswave.Devices.Application.Dashboards.Features.CreateDashboard;

public class CreateDashboardHandler(
    DashboardFactory factory,
    IDashboardAccessService accessService,
    IDashboardsQueryRespository queryRepository,
    IOptions<DevicesOptions> options,
    IDashboardCommandRepository commandRepository,
    ILogger<CreateDashboardHandler> logger) : ICommandHandler<CreateDashboardCommand, Dashboard>
{
    public async Task<Result<Dashboard>> Handle(CreateDashboardCommand request, CancellationToken cancellationToken)
    {
        var canManage = await accessService.CanManageDevice(request.UserId, request.DeviceId, cancellationToken);

        if (!canManage)
            return Result<Dashboard>.Failure(canManage.Errors);

        var dashboardExists = await queryRepository.NameUsed(request.DeviceId, request.Name, cancellationToken);

        if (dashboardExists)
        {
            logger.LogWarning("[UserId: {UserId}][DeviceId: {DeviceId}] Dashboard with name '{DashboardName}' already exists.", request.UserId, request.DeviceId, request.Name);
            return Result<Dashboard>.Failure(CreateDashboardErrors.DashboardAlreadyCreated);
        }

        //TODO: Redis lock for device

        var operationsCount = await queryRepository.CountDashboardsByDevice(request.DeviceId, cancellationToken);

        if (options.Value.Limits.DashboardsPerDevice <= operationsCount)
        {
            logger.LogWarning("[UserId: {UserId}][DeviceId: {DeviceId}] Dashboards limit reached for device.", request.UserId, request.DeviceId);
            return Result<Dashboard>.Failure(CreateDashboardErrors.DashboardsLimitReached);
        }

        var initializationResult = await factory.Initialize(request.DeviceId, request.Name, request.Icon, request.Type, request.Configuration);

        if (initializationResult.IsFailure)
        {
            logger.LogError("[UserId: {UserId}][DeviceId: {DeviceId}] Failed to initialize dashboard creation",
                request.UserId,
                request.DeviceId);
            return Result<Dashboard>.Failure(initializationResult.Errors);
        }

        var dashboard = initializationResult.Data
            .ToDashboard();

        var result = await commandRepository.CreateDashboard(dashboard, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogError("[UserId: {UserId}][DeviceId: {DeviceId}] Failed to create dashboard.",
                request.UserId,
                request.DeviceId);
            return Result<Dashboard>.Failure(result.Errors);
        }

        logger.LogInformation("[UserId: {UserId}][DeviceId: {DeviceId}] Dashboard created successfully.",
            request.UserId,
            request.DeviceId);
        return Result<Dashboard>.Success(dashboard);
    }
}
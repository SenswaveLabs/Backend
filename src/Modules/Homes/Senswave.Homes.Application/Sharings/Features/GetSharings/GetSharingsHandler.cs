using MassTransit.Initializers;
using Senswave.Homes.Domain.Homes.Services;
using Senswave.Homes.Domain.Sharings.Repositories;
using Senswave.Integration.User;

namespace Senswave.Homes.Application.Sharings.Features.GetSharings;

public class GetSharingsHandler(
    IHomeAccessService accessService,
    IHomeSharingQueryRepository sharingRepository,
    IRequestClient<EmailRequest> userClient,
    ILogger<GetSharingsHandler> logger) : IQueryHandler<GetSharingsQuery, IList<SharingModel>>
{
    public async Task<Result<IList<SharingModel>>> Handle(GetSharingsQuery request, CancellationToken cancellationToken)
    {
        var isOwner = await accessService.IsOwner(request.UserId, request.HomeId, cancellationToken);

        if (!isOwner)
            return Result<IList<SharingModel>>.Failure(isOwner.Errors);

        var sharingUsers = await sharingRepository.GetSharingUsers(request.HomeId, cancellationToken);

        var ids = sharingUsers
            .Select(x => x.UserId)
            .ToList();

        var emailRequests = new EmailRequest
        {
            UserIds = ids
        };

        var response = await userClient.GetResponse<EmailResponse>(emailRequests, cancellationToken);

        var idToEmails = response.Message.UserEmails;

        var homeSharingDtos = new List<SharingModel>();

        foreach (var homeSharing in sharingUsers)
        {
            var email = "Unknown";

            if (idToEmails.ContainsKey(homeSharing.UserId))
                email = idToEmails[homeSharing.UserId];

            homeSharingDtos.Add(new SharingModel
            {
                SharingId = homeSharing.Id,
                SharingType = homeSharing.SharingType,
                FriendEmail = email
            });
        }

        logger.LogInformation("[Home: {HomeId}] Retrieved {Count} sharings.",
            request.HomeId,
            homeSharingDtos.Count);

        return Result<IList<SharingModel>>.Success(homeSharingDtos);
    }
}
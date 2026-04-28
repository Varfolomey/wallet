using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MNR.SDK.Commons.MediatR.Behaviours;

public class UserScopeBehaviour<TRequest, TResponse>(ILogger<UserScopeBehaviour<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var userId = Activity.Current?.GetBaggageItem("user.id");
        var clientId = Activity.Current?.GetBaggageItem("client.id");
        var requestId = Activity.Current?.GetBaggageItem("request.trace.id");

        if (string.IsNullOrWhiteSpace(userId))
            return await next();
        
        using var _ = logger.BeginScope($"ReqId: {requestId}; UserId: {userId}; ClientId: {clientId}");
        return await next();
    }
}
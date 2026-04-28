using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MNR.SDK.Commons.MediatR.Behaviours;

public class UserScopeBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<UserScopeBehaviour<TRequest, TResponse>> _logger;

    public UserScopeBehaviour(ILogger<UserScopeBehaviour<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request, CancellationToken token, RequestHandlerDelegate<TResponse> next)
    {
        var userId = Activity.Current?.GetBaggageItem("user.id");
        var clientId = Activity.Current?.GetBaggageItem("client.id");
        var requestId = Activity.Current?.GetBaggageItem("request.trace.id");

        if (string.IsNullOrWhiteSpace(userId))
            return await next();
        
        using var _ = _logger.BeginScope($"ReqId: {requestId}; UserId: {userId}; ClientId: {clientId}");
        return await next();
    }
}
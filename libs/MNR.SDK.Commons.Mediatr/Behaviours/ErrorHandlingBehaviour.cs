using System.Diagnostics;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using MNR.SDK.Commons.Models;

namespace MNR.SDK.Commons.MediatR.Behaviours;

public class ErrorHandlingBehaviour<TRequest, TResponse>(ILogger<ErrorHandlingBehaviour<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private const string FieldUserId = "user.id";
    private const string FieldClientId = "client.id";
    private const string FieldRequestId = "request.trace.id";

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var userId = Activity.Current?.GetBaggageItem(FieldUserId);
        var clientId = Activity.Current?.GetBaggageItem(FieldClientId);
        var requestId = Activity.Current?.GetBaggageItem(FieldRequestId);

        using var scope = logger.BeginScope($"Request={requestId};ClientId={clientId},UserId={userId}");
        try
        {
            return await next();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Could not process request of {Type}", request.GetType().Name);
            var response = Result.Internal("Internal server error").Adapt<TResponse>();
            return response;
        }
    }
}
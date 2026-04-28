using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MNR.SDK.Commons.MediatR.Behaviours;

namespace MNR.SDK.Commons.MediatR;

public static class DependencyInjection
{
    public static IServiceCollection AddMediatr(
        this IServiceCollection services,
        params Type[] lookupTypes)
    {
        services.AddMediatR(lookupTypes ?? throw new ArgumentNullException(nameof(lookupTypes)));

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ErrorHandlingBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UserScopeBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehaviour<,>));

        return services;
    }
}
using FluentValidation;
using Mapster;
using MediatR;
using MNR.SDK.Commons.Models;

namespace MNR.SDK.Commons.MediatR.Behaviours;

public class RequestValidationBehaviour<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);

        var failures = validators
            .Select(s => s.Validate(context))
            .SelectMany(result => result.Errors)
            .Where(f => f != null)
            .Select(e => e.ErrorMessage)
            .ToList();

        return failures.Count != 0
            ? Task.FromResult(Result.Bad(string.Join(". ", failures)).Adapt<TResponse>())
            : next();
    }
}
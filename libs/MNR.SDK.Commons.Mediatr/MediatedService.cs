using System.Runtime.CompilerServices;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;
using MNR.SDK.Commons.Models;

namespace MNR.SDK.Commons.MediatR;

public class MediatedService(IMediator mediator, ILogger logger)
{
    public async ValueTask<Result> Mediate<TReq, TBReq>(
        TReq request,
        CancellationToken token = default,
        [CallerMemberName] string caller = null)
        where TBReq : IRequest<Result>
    {
        var businessRequest = request.Adapt<TBReq>();
        try
        {
            var businessResponse = await mediator.Send(businessRequest, token);
            if (!businessResponse.Success)
                logger.LogWarning("Error: Request {Name} ended with {Message}", caller, businessResponse.Message);

            return businessResponse;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Could not perform call {Name}. Something went wrong", caller);
            throw;
        }
    }

    public async ValueTask<Result<TRes>> Mediate<TReq, TBReq, TBRes, TRes>(
        TReq request,
        CancellationToken token = default,
        [CallerMemberName] string caller = null)
        where TBReq : IRequest<Result<TBRes>>
    {
        var businessRequest = request.Adapt<TBReq>();
        try
        {
            var businessResponse = await mediator.Send(businessRequest, token);
            if (businessResponse.Success)
                return Result<TRes>.Ok(businessResponse.Data.Adapt<TRes>(), businessResponse.Message);

            logger.LogWarning("Error: Request {Name} ended with {Message}", caller, businessResponse.Message);
            return Result<TRes>.Failed(businessResponse);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Could not perform call {Name}. Something went wrong", caller);
            throw;
        }
    }

    public async ValueTask<ResultList<TRes>> MediateList<TReq, TBReq, TBRes, TRes>(
        TReq request,
        CancellationToken token = default,
        [CallerMemberName] string caller = null)
        where TBReq : IRequest<ResultList<TBRes>>
    {
        var businessRequest = request.Adapt<TBReq>();
        try
        {
            var businessResponse = await mediator.Send(businessRequest, token);
            if (businessResponse.Success)
                return ResultList<TRes>.Ok(
                    businessResponse.Data?.Select(e => e.Adapt<TRes>()) ?? new List<TRes>(),
                    businessResponse.Message);

            logger.LogWarning("Error: Request {Name} ended with {Message}", caller, businessResponse.Message);
            return ResultList<TRes>.Failed(businessResponse);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Could not perform call {Name}. Something went wrong", caller);
            throw;
        }
    }

    public async ValueTask<PagedResult<TRes>> MediatePagedList<TReq, TBReq, TBRes, TRes>(
        TReq request,
        CancellationToken token = default,
        [CallerMemberName] string caller = null)
        where TBReq : IRequest<PagedResult<TBRes>>
    {
        var businessRequest = request.Adapt<TBReq>();
        try
        {
            var businessResponse = await mediator.Send(businessRequest, token);
            if (businessResponse.Success)
                return PagedResult<TRes>.Ok(
                    businessResponse.Data?.Select(e => e.Adapt<TRes>()) ?? new List<TRes>(),
                    businessResponse.Count,
                    businessResponse.Page,
                    businessResponse.Total);

            logger.LogWarning("Error: Request {Name} ended with {Message}", caller, businessResponse.Message);
            return PagedResult<TRes>.Failed(businessResponse);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Could not perform call {Name}. Something went wrong", caller);
            throw;
        }
    }
}
using Mapster;

namespace MNR.SDK.Commons.Models.Extensions;

public static class ExpressionExtensions
{
    public static async Task<Result> OnSuccess(this Task<Result> sourceCall, Func<Task<Result>> success)
    {
        var res = await sourceCall;
        if (!res.Success)
            return res;
        return await success();
    }

    public static async Task<Result> OnSuccess<T>(this Task<Result> sourceCall, Func<Task<Result<T>>> success)
    {
        var res = await sourceCall;
        return !res.Success ? res : (await success()).Adapt<Result>();
    }

    public static async Task<Result<T>> OnSuccess<T, TK>(
        this Task<Result<T>> sourceCall,
        Func<Task<Result<TK>>> success)
    {
        var res = await sourceCall;
        return !res.Success ? res : (await success()).Adapt<Result<T>>();
    }

    public static async Task<Result<TK>> OnSuccessNext<TK>(
        this Task<Result> sourceCall,
        Func<Task<Result<TK>>> success)
    {
        var res = await sourceCall;
        return !res.Success ? Result<TK>.Failed(res.Message, res.StatusCode) : await success();
    }

    public static async Task<Result<TK>> OnSuccessNext<T, TK>(
        this Task<Result<T>> sourceCall,
        Func<Task<Result<TK>>> success)
    {
        var res = await sourceCall;
        return !res.Success ? Result<TK>.Failed(res.Message, res.StatusCode) : await success();
    }

    public static async Task<Result> OnFailure(this Task<Result> sourceCall, Func<Task<Result>> failure)
    {
        var res = await sourceCall;
        if (res.Success)
            return res;
        return await failure();
    }

    public static async Task<Result> OnFailure<T>(this Task<Result> sourceCall, Func<Task<Result<T>>> failure)
    {
        var res = await sourceCall;
        return res.Success ? res : (await failure()).Adapt<Result>();
    }

    public static async Task<Result<T>> OnFailure<T, TK>(
        this Task<Result<T>> sourceCall,
        Func<Task<Result<TK>>> failure)
    {
        var res = await sourceCall;
        return res.Success ? res : (await failure()).Adapt<Result<T>>();
    }

    public static async Task<Result> OnFailureNext(
        this Task<Result> sourceCall,
        Func<Task<Result>> failure)
    {
        var res = await sourceCall;
        return !res.Success ? res : await failure();
    }

    public static async Task<Result<TK>> OnFailureNext<TK>(
        this Task<Result> sourceCall,
        Func<Task<Result<TK>>> failure)
    {
        var res = await sourceCall;
        return !res.Success ? Result<TK>.Failed(res.Message, res.StatusCode) : await failure();
    }

    public static async Task<Result<TK>> OnFailureNext<T, TK>(
        this Task<Result<T>> sourceCall,
        Func<Task<Result<TK>>> failure)
    {
        var res = await sourceCall;
        return !res.Success ? Result<TK>.Failed(res.Message, res.StatusCode) : await failure();
    }

    // ValueTasks
    public static async ValueTask<Result> OnSuccess(
        this ValueTask<Result> sourceCall, Func<ValueTask<Result>> success)
    {
        var res = await sourceCall;
        if (!res.Success)
            return res;
        return await success();
    }

    public static async ValueTask<Result> OnSuccess<T>(
        this ValueTask<Result> sourceCall, Func<ValueTask<Result<T>>> success)
    {
        var res = await sourceCall;
        return !res.Success ? res : (await success()).Adapt<Result>();
    }

    public static async ValueTask<Result<T>> OnSuccess<T, TK>(
        this ValueTask<Result<T>> sourceCall,
        Func<ValueTask<Result<TK>>> success)
    {
        var res = await sourceCall;
        return !res.Success ? res : (await success()).Adapt<Result<T>>();
    }

    public static async ValueTask<Result<TK>> OnSuccessNext<TK>(
        this ValueTask<Result> sourceCall,
        Func<ValueTask<Result<TK>>> success)
    {
        var res = await sourceCall;
        return !res.Success ? Result<TK>.Failed(res.Message, res.StatusCode) : await success();
    }

    public static async ValueTask<Result<TK>> OnSuccessNext<T, TK>(
        this ValueTask<Result<T>> sourceCall,
        Func<ValueTask<Result<TK>>> success)
    {
        var res = await sourceCall;
        return !res.Success ? Result<TK>.Failed(res.Message, res.StatusCode) : await success();
    }

    public static async ValueTask<Result> OnFailure(
        this ValueTask<Result> sourceCall, Func<ValueTask<Result>> failure)
    {
        var res = await sourceCall;
        if (res.Success)
            return res;
        return await failure();
    }

    public static async ValueTask<Result> OnFailure<T>(
        this ValueTask<Result> sourceCall, Func<ValueTask<Result<T>>> failure)
    {
        var res = await sourceCall;
        return res.Success ? res : (await failure()).Adapt<Result>();
    }

    public static async ValueTask<Result<T>> OnFailure<T, TK>(
        this ValueTask<Result<T>> sourceCall,
        Func<ValueTask<Result<TK>>> failure)
    {
        var res = await sourceCall;
        return res.Success ? res : (await failure()).Adapt<Result<T>>();
    }

    public static async ValueTask<Result> OnFailureNext(
        this ValueTask<Result> sourceCall,
        Func<ValueTask<Result>> failure)
    {
        var res = await sourceCall;
        return !res.Success ? res : await failure();
    }

    public static async ValueTask<Result<TK>> OnFailureNext<TK>(
        this ValueTask<Result> sourceCall,
        Func<ValueTask<Result<TK>>> failure)
    {
        var res = await sourceCall;
        return !res.Success ? Result<TK>.Failed(res.Message, res.StatusCode) : await failure();
    }

    public static async ValueTask<Result<TK>> OnFailureNext<T, TK>(
        this ValueTask<Result<T>> sourceCall,
        Func<ValueTask<Result<TK>>> failure)
    {
        var res = await sourceCall;
        return !res.Success ? Result<TK>.Failed(res.Message, res.StatusCode) : await failure();
    }
}
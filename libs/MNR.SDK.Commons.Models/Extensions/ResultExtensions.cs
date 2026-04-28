namespace MNR.SDK.Commons.Models.Extensions;

public static class ResultExtensions
{
    public static async Task<Result> OnSuccess(this Task<Result> result, Task<Result> next)
    {
        var res = await result;
        if (res.Success)
            return await next;
        return res;
    }

    public static async Task<Result> OnFailure(this Task<Result> result, Task<Result> next)
    {
        var res = await result;
        if (!res.Success)
            return await next;
        return res;
    }

    public static async Task<Result> Next(this Task<Result> result, Task<Result> success, Task<Result> failure)
    {
        var res = await result;
        return await (res.Success ? success : failure);
    }

    public static async Task<Result<T>> Next<T>(
        this Task<Result> result, Task<Result<T>> success, Task<Result<T>> failure)
    {
        var res = await result;
        return await (res.Success ? success : failure);
    }

    public static async Task<Result<T>> OnSuccess<T>(this Task<Result> result, Task<Result<T>> next)
    {
        var res = await result;
        if (res.Success)
            return await next;
        return Result<T>.Failed(res.Message, res.StatusCode);
    }

    public static async Task<Result<T>> OnFailure<T>(this Task<Result> result, Task<Result<T>> next)
    {
        var res = await result;
        if (!res.Success)
            return await next;
        return Result<T>.Failed(res.Message, res.StatusCode);
    }
}
using System.Runtime.Serialization;
using MNR.SDK.Commons.Models.Enums;

namespace MNR.SDK.Commons.Models;

[DataContract]
public class Result : IResult
{
    [DataMember(Order = 1)] public bool Success { get; set; }
    [DataMember(Order = 2)] public string Message { get; set; }
    [DataMember(Order = 3)] public int StatusCode { get; set; }
    [DataMember(Order = 4)] public string Url { get; set; }
    [DataMember(Order = 5)] public Severity Severity { get; set; }

    public static Result Ok(string message = null)
    {
        return New(message, 200, true);
    }

    public static Result Created(string message = null)
    {
        return New(message, 201, true);
    }

    public static Result Updated(string message = null)
    {
        return New(message, 204, true);
    }

    public static Result Bad(string message)
    {
        return New(message, 400);
    }

    public static Result UnAuthorized(string message)
    {
        return New(message, 401);
    }

    public static Result Forbidden(string message)
    {
        return New(message, 403);
    }

    public static Result NotFound(string message)
    {
        return New(message, 404);
    }

    public static Result Failed(string message, int code = 500)
    {
        return New(message, code);
    }

    public static Result Failed(IResult result)
    {
        return New(result.Message, result.StatusCode);
    }

    public static Result Internal(string message)
    {
        return New(message);
    }

    public static Result Code(string message, int code)
    {
        return New(message);
    }

    private static Result New(
        string message,
        int code = 500,
        bool success = false)
    {
        return new() {Message = message, StatusCode = code, Success = success};
    }
}

[DataContract]
public class Result<T> : IResult<T>
{
    [DataMember(Order = 3)] public T Data { get; set; }
    [DataMember(Order = 1)] public bool Success { get; set; }
    [DataMember(Order = 2)] public string Message { get; set; }
    [DataMember(Order = 4)] public int StatusCode { get; set; }
    [DataMember(Order = 5)] public string Url { get; set; }
    [DataMember(Order = 6)] public Severity Severity { get; set; }

    public static Result<T> Ok(T data, string message = null)
    {
        return New(message, 200, data, true);
    }

    public static Result<T> Created(T data, string message = null)
    {
        return New(message, 201, data, true);
    }

    public static Result<T> Updated(T data = default, string message = null)
    {
        return New(message, 204, data, true);
    }

    public static Result<T> Bad(string message)
    {
        return New(message, 400);
    }

    public static Result<T> UnAuthorized(string message)
    {
        return New(message, 401);
    }

    public static Result<T> Forbidden(string message)
    {
        return New(message, 403);
    }

    public static Result<T> NotFound(string message)
    {
        return New(message, 404);
    }

    public static Result<T> Failed(string message, int code = 500, T data = default)
    {
        return New(message, code, data);
    }

    public static Result<T> Failed(IResult result)
    {
        return New(result.Message, result.StatusCode);
    }
    
    public static Result<T> Failed(Result<T> result)
    {
        return New(result.Message, result.StatusCode, result.Data);
    }

    public static Result<T> Internal(string message)
    {
        return New(message);
    }

    public static Result<T> Code(string message, int code)
    {
        return New(message);
    }

    public static Result<T> Code(T data, string message, int code)
    {
        return New(message, code, data);
    }

    private static Result<T> New(
        string message,
        int code = 500,
        T data = default,
        bool success = false)
    {
        return new() {Message = message, StatusCode = code, Success = success, Data = data};
    }
}
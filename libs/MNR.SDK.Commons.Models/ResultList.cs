using System.Runtime.Serialization;
using MNR.SDK.Commons.Models.Enums;

namespace MNR.SDK.Commons.Models;

[DataContract]
public class ResultList<T> : IResult<T>
{
    [DataMember(Order = 3)] public IEnumerable<T> Data { get; set; }
    [DataMember(Order = 1)] public bool Success { get; set; }
    [DataMember(Order = 2)] public string Message { get; set; }
    [DataMember(Order = 4)] public int StatusCode { get; set; }
    [DataMember(Order = 5)] public string Url { get; set; }
    [DataMember(Order = 6)] public Severity Severity { get; set; }

    public static ResultList<T> Ok(IEnumerable<T> data, string message = null)
    {
        return New(message, 200, data, true);
    }

    public static ResultList<T> Created(IEnumerable<T> data, string message = null)
    {
        return New(message, 201, data, true);
    }

    public static ResultList<T> Updated(IEnumerable<T> data = default, string message = null)
    {
        return New(message, 204, data, true);
    }

    public static ResultList<T> Bad(string message, Severity severity, int code = 400)
    {
        return New(message, code, severity: severity);
    }

    public static ResultList<T> UnAuthorized(string message)
    {
        return New(message, 401, severity: Severity.Error);
    }

    public static ResultList<T> Forbidden(string message)
    {
        return New(message, 403, severity: Severity.Error);
    }

    public static ResultList<T> NotFound(string message, Severity severity)
    {
        return New(message, 404);
    }

    public static ResultList<T> Failed(string message, Severity severity, int code = 500)
    {
        return New(message, code, severity: severity);
    }

    public static ResultList<T> Failed(IResult result)
    {
        return New(result.Message, result.StatusCode, severity: result.Severity);
    }

    public static ResultList<T> Internal(string message)
    {
        return New(message, 500, severity: Severity.Critical);
    }
    
    public static ResultList<T> Code(string message, int code)
    {
        return New(message, code);
    }

    public static ResultList<T> Code(IEnumerable<T> data, string message, int code)
    {
        return New(message, code, data);
    }


    private static ResultList<T> New(
        string message,
        int code = 500,
        IEnumerable<T> data = default,
        bool success = false,
        Severity severity = Severity.Info)
    {
        return new() { Message = message, StatusCode = code, Success = success, Data = data, Severity = severity};
    }
}
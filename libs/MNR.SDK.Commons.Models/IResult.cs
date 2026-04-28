using MNR.SDK.Commons.Models.Enums;

namespace MNR.SDK.Commons.Models;

public interface IResult
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public int StatusCode { get; set; }
    public string Url { get; set; }
    public Severity Severity { get; set; }
}

public interface IResult<T> : IResult
{
}
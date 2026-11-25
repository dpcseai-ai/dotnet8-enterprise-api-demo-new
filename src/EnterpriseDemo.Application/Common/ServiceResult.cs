namespace EnterpriseDemo.Application.Common;

public sealed record ServiceResult<T>(T? Value, string? Error, int StatusCode)
{
    public bool Succeeded => Error is null;
    public static ServiceResult<T> Success(T value, int statusCode = 200) => new(value, null, statusCode);
    public static ServiceResult<T> Failure(string error, int statusCode) => new(default, error, statusCode);
}

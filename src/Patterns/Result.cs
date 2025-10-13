using System.Diagnostics.CodeAnalysis;

namespace Nkraft.CrossUtility.Patterns;

public enum ErrorCode
{
	Unknown = 0,

	General = 1,

	InvalidState = 2,

	InvalidParameter = 3,

	NotSupported = 4,

	Cancelled = 5,
}

public interface IResult
{
	public bool IsSuccess { get; }

	public bool IsFailure { get; }

	public ErrorCode ErrorCode { get; }

	string? ErrorMessage { get; }
}

public sealed class Result<T> : IResult
{
	public T? Value { get; }

	public bool IsSuccess { get; }

	public bool IsFailure => IsSuccess == false;

	public string? ErrorMessage { get; }

	public ErrorCode ErrorCode { get; }

	internal Result(T value)
	{
		Value = value;
		IsSuccess = true;
		ErrorCode = ErrorCode.Unknown;
		ErrorMessage = null;
	}

	internal Result(ErrorCode errorCode, string errorMessage)
	{
		Value = default;
		IsSuccess = false;
		ErrorMessage = errorMessage;
		ErrorCode = errorCode;
	}

	internal Result(ErrorCode errorCode, string format, params object[] args)
	{
		Value = default;
		IsSuccess = false;
		ErrorMessage = string.Format(format, args);
		ErrorCode = errorCode;
	}

	public bool TryGetValue([NotNullWhen(true)] out T? value)
	{
		if (IsSuccess && Value is T outValue)
		{
			value = outValue;
			return true;
		}
		value = default;
		return false;
	}
}

public static class Result
{
	private readonly struct Unit
	{
		internal static readonly Unit Instance = new();
	}

	public static IResult Ok()
		=> new Result<Unit>(value: Unit.Instance);

	public static IResult Fail(ErrorCode errorCode, string errorMessage)
		=> new Result<Unit>(errorCode, errorMessage);

	public static IResult Fail(ErrorCode errorCode, string format, params object[] args)
		=> new Result<Unit>(errorCode, format, args);

	public static Result<TValue> Ok<TValue>(TValue value)
		=> new(value);

	public static Result<TValue> Fail<TValue>(ErrorCode errorCode, string errorMessage)
		=> new(errorCode, errorMessage);

	public static Result<TValue> Fail<TValue>(ErrorCode errorCode, string format, params object[] args)
		=> new(errorCode, format, args);
}
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Text.RegularExpressions;

namespace Nkraft.CrossUtility.Patterns;

public enum ErrorCode
{
	Unknown = 0,

	General = 1,

	InvalidState = 2,

	InvalidParameter = 3,

	NotSupported = 4,
	
	NotHandled = 5,

	Cancelled = 6,
}

public interface IResult
{
	public bool IsSuccess { get; }

	public bool IsFailure { get; }

	public ErrorCode ErrorCode { get; }

	string? ErrorMessage { get; }

	/// <summary>
	/// The exception that caused the failure, if any. Kept so callers that need to fail fast
	/// (e.g. app startup) can rethrow the original instead of a message-only substitute.
	/// </summary>
	Exception? Exception { get; }
}

public abstract class ResultBase
{
	protected static readonly Regex PlaceholderRegex =
		new(@"\{(\w+)\}", RegexOptions.Compiled | RegexOptions.CultureInvariant);

	protected static string FormatString(string format, params object?[]? args)
	{
		if (string.IsNullOrWhiteSpace(format))
			return string.Empty;

		if (args is null || args.Length == 0 || format.IndexOf('{') < 0)
			return format;

		var names = new Dictionary<string, int>(StringComparer.Ordinal);
		int count = 0;
		var sb = new StringBuilder();
		int lastIndex = 0;

		foreach (Match m in PlaceholderRegex.Matches(format))
		{
			sb.Append(format, lastIndex, m.Index - lastIndex);
			lastIndex = m.Index + m.Length;

			var name = m.Groups[1].Value;
			if (names.TryGetValue(name, out var index) == false)
			{
				if (count >= args.Length)
				{
					sb.Append(m.Value);
					continue;
				}
				index = count++;
				names[name] = index;
			}

			var value = index < args.Length ? args[index] : null;
			sb.Append(value ?? string.Empty);
		}

		sb.Append(format, lastIndex, format.Length - lastIndex);
		return sb.ToString();
	}
}

public sealed class Result<T> : ResultBase, IResult
{
	public T? Value { get; }

	public bool IsSuccess { get; }

	public bool IsFailure => IsSuccess == false;

	public string? ErrorMessage { get; }

	public ErrorCode ErrorCode { get; }

	public Exception? Exception { get; }

	internal Result(T value)
	{
		Value = value;
		IsSuccess = true;
		ErrorCode = ErrorCode.Unknown;
		ErrorMessage = null;
		Exception = null;
	}

	internal Result(ErrorCode errorCode, Exception? exception, string errorMessage)
	{
		Value = default;
		IsSuccess = false;
		ErrorMessage = errorMessage;
		ErrorCode = errorCode;
		Exception = exception;
	}

	internal Result(ErrorCode errorCode, Exception? exception, string format, params object?[] args)
	{
		Value = default;
		IsSuccess = false;
		ErrorMessage = FormatString(format, args);
		ErrorCode = errorCode;
		Exception = exception;
	}

	public bool TryGetValue([NotNullWhen(true)] out T? value)
	{
		if (IsSuccess && Value is not null)
		{
			value = Value;
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
		=> new Result<Unit>(errorCode, exception: null, errorMessage);

	public static IResult Fail(ErrorCode errorCode, string format, params object?[] args)
		=> new Result<Unit>(errorCode, exception: null, format, args);

	public static IResult Fail(ErrorCode errorCode, Exception? exception, string errorMessage)
		=> new Result<Unit>(errorCode, exception, errorMessage);

	public static IResult Fail(ErrorCode errorCode, Exception? exception, string format, params object?[] args)
		=> new Result<Unit>(errorCode, exception, format, args);

	public static Result<TValue> Ok<TValue>(TValue value)
		=> new(value);

	public static Result<TValue> Fail<TValue>(ErrorCode errorCode, string errorMessage)
		=> new(errorCode, exception: null, errorMessage);

	public static Result<TValue> Fail<TValue>(ErrorCode errorCode, string format, params object?[] args)
		=> new(errorCode, exception: null, format, args);

	public static Result<TValue> Fail<TValue>(ErrorCode errorCode, Exception? exception, string errorMessage)
		=> new(errorCode, exception, errorMessage);

	public static Result<TValue> Fail<TValue>(ErrorCode errorCode, Exception? exception, string format, params object?[] args)
		=> new(errorCode, exception, format, args);
}

public static class ResultExtensions
{
	/// <summary>
	/// Throws if <paramref name="result"/> failed: rethrows <see cref="IResult.Exception"/> with its
	/// original stack trace when present, otherwise throws <see cref="InvalidOperationException"/>
	/// carrying the error code and message. For call sites where a failure is fatal.
	/// </summary>
	public static void ThrowIfFailure(this IResult result)
	{
		if (result.IsSuccess)
			return;

		if (result.Exception is { } exception)
			ExceptionDispatchInfo.Capture(exception).Throw();

		throw new InvalidOperationException($"[{result.ErrorCode}] {result.ErrorMessage}");
	}
}

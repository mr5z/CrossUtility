using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

	internal Result(ErrorCode errorCode, string format, params object?[] args)
	{
		Value = default;
		IsSuccess = false;
		ErrorMessage = FormatString(format, args);
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

	private static string FormatString(string format, params object?[]? args)
	{
		if (string.IsNullOrWhiteSpace(format))
		{
			return string.Empty;
		}

		if (args is null || args.Length == 0)
		{
			return format;
		}

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

	private static readonly Regex PlaceholderRegex =
		new(@"/{(\w+)/}", RegexOptions.Compiled | RegexOptions.CultureInvariant);

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

	public static IResult Fail(ErrorCode errorCode, string format, params object?[] args)
		=> new Result<Unit>(errorCode, format, args);

	public static Result<TValue> Ok<TValue>(TValue value)
		=> new(value);

	public static Result<TValue> Fail<TValue>(ErrorCode errorCode, string errorMessage)
		=> new(errorCode, errorMessage);

	public static Result<TValue> Fail<TValue>(ErrorCode errorCode, string format, params object?[] args)
		=> new(errorCode, format, args);
}
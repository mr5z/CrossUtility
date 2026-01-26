using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;

namespace Nkraft.CrossUtility.Helpers;

public static class QueryStringHelper
{
    public static string ToQueryString<T>(
        T model,
        BindingFlags flags = BindingFlags.Public | BindingFlags.Instance)
        where T : class
    {
        var result = JsonHelper.ToKeyValuePairs(model, flags);
        return string.Join("&", result.Select(e => $"{e.Key}={WebUtility.UrlEncode(e.Value?.ToString())}"));
    }

    public static Dictionary<string, object> ToDictionary(string queryString)
    {
        if (string.IsNullOrWhiteSpace(queryString))
        {
            return [];
        }

        if (queryString.StartsWith('?'))
        {
            queryString = queryString[1..];
        }

        return queryString.Split('&')
            .Select(part => part.Split('='))
            .Where(part => part.Length == 2)
            .ToDictionary(
                part => part.First(),
                part => TryParsePrimitive(part.Last())
            );
    }

    private static object TryParsePrimitive(string value)
	{
		if (int.TryParse(value, out var intResult))
			return intResult;
		if (bool.TryParse(value, out var boolResult))
			return boolResult;
		if (double.TryParse(value, out var doubleResult))
			return doubleResult;
		if (DateTime.TryParse(value, out var dateResult))
			return dateResult;
		if (Guid.TryParse(value, out var guidResult))
			return guidResult;
		if (char.TryParse(value, out var charResult))
			return charResult;

        return value;
	}
}

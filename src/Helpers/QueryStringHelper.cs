using System.Linq;
using System.Net;
using System.Reflection;

namespace CrossUtility.Helpers
{
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
    }
}

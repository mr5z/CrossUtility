using CrossUtility.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CrossUtility.Extensions;

public static class ObjectExtension
{
    public static IDictionary<string, object> AsDictionary(
        this object source,
        BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Instance)
    {
        return source.GetType().GetProperties(bindingAttr).ToDictionary
        (
            propInfo => propInfo.Name,
            propInfo => propInfo.GetValue(source, null)
        );
    }

    public static IDictionary<string, string?> AsStringDictionary(
        this object source,
        BindingFlags flags = BindingFlags.Public | BindingFlags.Instance)
    {
        var properties = JsonHelper.ToKeyValuePairs(source, flags);
        return new Dictionary<string, string?>(properties);
    }
}

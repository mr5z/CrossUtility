using Nkraft.CrossUtility.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nkraft.CrossUtility.Extensions;

internal static class ObjectExtension
{
    extension(object source)
    {
        public IDictionary<string, object> AsDictionary(BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Instance)
        {
            return source.GetType().GetProperties(bindingAttr).ToDictionary
            (
                propInfo => propInfo.Name,
                propInfo => propInfo.GetValue(source, null)
            );
        }

        public IDictionary<string, string?> AsStringDictionary(BindingFlags flags = BindingFlags.Public | BindingFlags.Instance)
        {
            var properties = JsonHelper.ToKeyValuePairs(source, flags);
            return new Dictionary<string, string?>(properties);
        }
    }
}

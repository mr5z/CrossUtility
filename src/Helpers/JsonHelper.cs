using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;

namespace CrossUtility.Helpers
{
    public static class JsonHelper
    {
        private static class TargetAttribute
        {
            public const string NamedAttributePropertyName = "Name";
            public const string IgnoreAttributePropertyName = "Condition";
        }

        public static IEnumerable<KeyValuePair<string, string?>> ToKeyValuePairs<T>(
            T model,
            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance)
            where T : notnull
        {
            var type = model.GetType();
            var properties = type.GetProperties(flags);
            return properties.Where(property =>
                {
                    var attribute = property.GetCustomAttribute<JsonIgnoreAttribute>();
                    var ignoreValue = AttributeHelper.GetValue<JsonIgnoreCondition?>(
                        attribute,
                        TargetAttribute.IgnoreAttributePropertyName
                    ) ?? JsonIgnoreCondition.Never;

                    return ignoreValue switch
                    {
                        JsonIgnoreCondition.Always => false,
                        JsonIgnoreCondition.Never => true,
                        JsonIgnoreCondition.WhenWritingDefault => property.GetValue(model, null) != default,
                        JsonIgnoreCondition.WhenWritingNull => property.GetValue(model, null) != null,
                        _ => throw new InvalidEnumArgumentException(nameof(JsonIgnoreCondition))
                    };
                })
                .Select(property =>
                {
                    var attribute = property.GetCustomAttribute<JsonPropertyNameAttribute>();
                    var preferredName = AttributeHelper.GetValue<string?>(
                        attribute,
                        TargetAttribute.NamedAttributePropertyName
                    );

                    var key = preferredName ?? property.Name;
                    var value = property.GetValue(model, null)?.ToString();

                    return new KeyValuePair<string, string?>(key, value);
                }
            );
        }
    }
}

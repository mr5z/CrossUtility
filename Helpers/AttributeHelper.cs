using System;
using System.Linq;
using System.Reflection;

namespace CrossUtility.Helpers
{
    static class AttributeHelper
    {
        public static T? GetValue<T>(
            PropertyInfo propertyInfo,
            string targetAttributeName,
            string targetAttributePropertyName)
        {
            var attributes = propertyInfo.GetCustomAttributes();
            var targetAttribute = attributes?.FirstOrDefault(e => e.GetType().FullName == targetAttributeName);
            if (targetAttribute == default)
                return default;

            return GetValue<T>(targetAttribute, targetAttributePropertyName);
        }

        public static T? GetValue<T>(Attribute? attribute, string propertyName)
        {
            if (attribute == null)
                return default;

            var properties = attribute.GetType().GetProperties();
            var targetProperty = properties?.FirstOrDefault(e => e.Name == propertyName);
            var value = targetProperty?.GetValue(attribute, null);

            return (value == default) ? default : (T)value;
        }
    }
}

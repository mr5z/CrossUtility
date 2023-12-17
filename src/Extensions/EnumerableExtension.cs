using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CrossUtility.Extensions;

public static class EnumerableExtension
{
    /// <summary>
    /// Retrieve the first element from the <typeparamref name="IEnumerable" />&lt;<typeparamref name="TModel"/>&gt; if any, and return the property value from the input expression.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    /// <param name="source"></param>
    /// <param name="expression"></param>
    /// <returns>The property evaluated from the <typeparamref name="TModel"/>. <see langword="default" /> if source is empty.</returns>
    public static TProperty? Dissect<TModel, TProperty>(this IEnumerable<TModel?> source,
        Expression<Func<TModel, TProperty>> expression)
    {
        var first = source.FirstOrDefault();
        if (first != null)
        {
            var propertyName = expression.GetMemberName();
            var type = typeof(TModel);
            var propInfo = type.GetProperty(propertyName);
            return propInfo == null
                ? throw new InvalidOperationException($"Cannot retrieve '{type.Name}.{propertyName}'")
                : (TProperty)propInfo.GetValue(first, null);
        }

        return default;
    }

    public static bool IsNullOrEmpty<TModel>(this IEnumerable<TModel>? enumerable)
        => enumerable == null || !enumerable.Any();
}

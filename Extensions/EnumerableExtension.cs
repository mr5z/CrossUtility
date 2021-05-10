using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CrossUtility.Extensions
{
    public static class EnumerableExtension
    {
        /// <summary>
        /// Retrieve the first element if any, and evaluate the expression in the source <typeparamref name="IEnumerable" />&lt;<typeparamref name="TModel"/>&gt;.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="source"></param>
        /// <param name="expression"></param>
        /// <returns>The property evaluated from the <typeparamref name="TModel"/>. <see langword="default" /> if source is empty.</returns>
        public static TProperty? Dissect<TModel, TProperty>(this IEnumerable<TModel?> source,
            Expression<Func<TModel, TProperty>> expression)
        {
            if (source.Any())
            {
                var propertyName = expression.GetMemberName();
                var type = typeof(TProperty);
                var propInfo = type.GetProperty(propertyName);
                if (propInfo == null)
                    throw new InvalidOperationException($"Cannot retrieve '{type.Name}.{propertyName}'");

                var first = source.First();
                return (TProperty)propInfo.GetValue(first);
            }

            return default;
        }
    }
}

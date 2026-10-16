using System;
using System.Collections.Generic;
using System.Linq;

namespace Nkraft.CrossUtility.Extensions;

internal static class EnumerableExtension
{
    public static bool IsNullOrEmpty<TModel>(this IEnumerable<TModel>? enumerable)
        => enumerable is null || !enumerable.Any();

    public static IEnumerable<T> WhereNot<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
        => enumerable.Where(e => !predicate(e));
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace CrossUtility.Helpers
{
    public static class EnumHelper
	{
		public static Dictionary<TKey, TValue?> EnumDefaultValues<TKey, TValue>()
			where TKey : struct, Enum
		{
			var type = typeof(TKey);
			if (!type.IsEnum)
				throw new ArgumentException("Key must be type enum.");

			var values = Enum.GetValues(type).OfType<TKey>();

			return values.ToDictionary(e => e, e => default(TValue));
		}
	}
}
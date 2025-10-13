using System.Collections.Generic;
using System.Linq;

namespace Nkraft.CrossUtility.Helpers;

internal class ObjectHelper
{
	public static Dictionary<string, object?> ToDictionary(object? obj)
	{
		if (obj is null)
		{
			return [];
		}

		return obj.GetType()
			.GetProperties()
			.Where(p => p.CanRead)
			.ToDictionary(p => p.Name, p => (object?)p.GetValue(obj));
	}
}

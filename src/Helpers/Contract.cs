using System;
using System.Linq;

namespace CrossUtility.Helpers
{
    public static class Contract
    {
        public static void NotNull<T>(T? obj)
        {
            if (obj == null)
                throw new InvalidOperationException($"{typeof(T).Name} is null");
        }

        public static void ThrowOn(Exception? exception)
        {
            if (exception != null)
                throw exception;
        }

        public static void InvalidArgumentIf<T>(T arg, params Type[] expectedTypes) where T : notnull
        {
            var type = arg.GetType();
            if (!expectedTypes.Any(t => t == type))
            {
                var expectedTypeList = string.Join(", ", expectedTypes.Select(it => it.Name));
                throw new ArgumentException($"Invalid type '{type.Name}', expecting any of: [{expectedTypeList}]");
            }
        }
    }
}
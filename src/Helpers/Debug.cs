using System;
using System.Runtime.CompilerServices;

namespace CrossUtility.Helpers;

public static class Debug
{
    public static void Log(string message, params object?[] args)
    {
#if DEBUG
        var time = DateTime.Now.TimeOfDay;
        System.Diagnostics.Debug.WriteLine($"{time} -> {message}", args);
#endif
    }

    public static void Measure(System.Diagnostics.Stopwatch sw, [CallerMemberName] string? caller = null)
    {
#if DEBUG
        sw.Stop();
        Log("execution time for {0}: {1:N2}s", caller!, sw.Elapsed.TotalSeconds);
#endif
    }
}

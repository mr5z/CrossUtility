using System;
using System.Threading.Tasks;

namespace Nkraft.CrossUtility.Extensions;

internal static class TaskExtension
{
    extension(Task task)
    {
        public void FireAndForget(Action<Exception>? exceptionHandler = null)
        {
            _ = task.ContinueWith(t =>
            {
                ReportException(t, exceptionHandler);
            });
        }

        public void FireAndForget(Action @continue, Action<Exception>? exceptionHandler = null)
        {
            _ = task.ContinueWith(t =>
            {
                try { @continue(); }
                finally { ReportException(t, exceptionHandler); }
            });
        }
    }

    extension<T>(Task<T> task)
    {
        public void FireAndForget(Action<Exception>? exceptionHandler = null)
        {
            _ = task.ContinueWith(t =>
            {
                ReportException(t, exceptionHandler);
            });
        }

        public void FireAndForget(Action @continue, Action<Exception>? exceptionHandler = null)
        {
            _ = task.ContinueWith(t =>
            {
                try { @continue(); }
                finally { ReportException(t, exceptionHandler); }
            });
        }
    }

    private static void ReportException(Task task, Action<Exception>? exceptionHandler)
    {
        if (task is { IsFaulted: false, IsCanceled: false })
            return;

        var ex = task.IsCanceled
            ? new TaskCanceledException(task)
            : task.Exception!.GetBaseException();

        exceptionHandler?.Invoke(ex);
    }
}

// (c) gfoidl, all rights reserved

using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Cairo.Utilities;

public static unsafe class CairoDebug
{
    private static ConcurrentDictionary<IntPtr, string>? s_traces;

    [MemberNotNullWhen(true, nameof(s_traces))]
    public static bool Enabled { get; } = GetIsEnabled();

    private static bool GetIsEnabled()
    {
        bool isEnabled = RuntimeConfigParser.GetRuntimeSettingSwitch(
            "Cairo.DebugDispose",
            "CAIRO_DEBUG_DISPOSE",
            defaultValue: false);

        if (isEnabled)
        {
            s_traces = [];
        }

        return isEnabled;
    }

    public static void OnAllocated(void* ptr) => OnAllocated((nint)ptr);
    public static void OnAllocated(IntPtr obj)
    {
        if (Enabled)
        {
            s_traces.TryAdd(obj, Environment.StackTrace);
        }
    }

    public static void OnDisposed<T>(void* ptr, bool disposing) => OnDisposed<T>((nint)ptr, disposing);
    public static void OnDisposed<T>(IntPtr obj, bool disposing) => OnDisposed(obj, disposing, typeof(T));
    public static void OnDisposed(void* ptr, bool disposing, Type type) => OnDisposed((nint)ptr, disposing, type);
    public static void OnDisposed(IntPtr obj, bool disposing, Type type)
    {
        if (disposing && !Enabled)
        {
            throw new InvalidOperationException();
        }

        if (Environment.HasShutdownStarted)
        {
            return;
        }

        if (!disposing)
        {
            Console.Error.WriteLine($"{type.FullName} is leaking, programmer is missing a call to Dispose");

            if (Enabled)
            {
                if (s_traces.TryGetValue(obj, out string? stackTrace))
                {
                    Console.Error.WriteLine("Allocated from:");
                    Console.Error.WriteLine(stackTrace);
                }
            }
            else
            {
                Console.Error.WriteLine("Set app context switch 'Cairo.DebugDispose' or set environment variable CAIRO_DEBUG_DISPOSE to track allocation traces");
            }
        }

        if (Enabled)
        {
            s_traces.TryRemove(obj, out _);
        }
    }
}

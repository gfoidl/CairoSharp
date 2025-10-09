// (c) gfoidl, all rights reserved

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Cairo;

[Serializable]
public class CairoException : Exception
{
    public Status Status { get; }

    public CairoException() { }
    public CairoException(string message) : base(message) { }
    public CairoException(string message, Exception inner) : base(message, inner) { }

    public CairoException(Status status) : this(status.GetString()) => this.Status = status;

    [DoesNotReturn]
    [StackTraceHidden]
    public static void ThrowOutOfMemory(string? message = null)
    {
        throw new CairoException(message ?? "Cairo signals out-of-memory");
    }
}

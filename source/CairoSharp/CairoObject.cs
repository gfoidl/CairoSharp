// (c) gfoidl, all rights reserved

using System.Diagnostics;
using Cairo.Utilities;

namespace Cairo;

/// <summary>
/// Base object for cairo.
/// </summary>
public abstract unsafe class CairoObject : IDisposable, IEquatable<CairoObject>
{
    private void* _handle;

    protected CairoObject(void* handle, bool allowNullPointer = false)
    {
        if (!allowNullPointer)
        {
            ArgumentNullException.ThrowIfNull(handle);
        }

        _handle = handle;

        CairoDebug.OnAllocated(handle);
    }

    protected internal void* Handle => _handle;

    [DebuggerStepThrough]
    protected internal void CheckDisposed() => ObjectDisposedException.ThrowIf(_handle is null, this);

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected void Dispose(bool disposing)
    {
        if (!disposing || CairoDebug.Enabled)
        {
            CairoDebug.OnDisposed(_handle, disposing, this.GetType());
        }

        if (_handle is null)
        {
            return;
        }

        this.DisposeCore(_handle);
        _handle = null;
    }

    protected abstract void DisposeCore(void* handle);

    ~CairoObject() => Dispose(false);

    /// <summary>
    /// Two <see cref="CairoObject"/>s are equal, when there native handles are equal.
    /// </summary>
    public bool Equals(CairoObject? other)
    {
        if (other is null)
        {
            return false;
        }

        return this.Handle == other.Handle;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj) => this.Equals(obj as CairoObject);

    /// <summary>
    /// The hashcode of this object.
    /// </summary>
    public override int GetHashCode() => ((nint)this.Handle).GetHashCode();
}

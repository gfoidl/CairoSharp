// (c) gfoidl, all rights reserved

using System.Diagnostics;
using Cairo.Utilities;

namespace Cairo;

/// <summary>
/// Generic base object for cairo.
/// </summary>
public abstract unsafe class CairoObject<T> : CairoObject, IEquatable<CairoObject<T>>
    where T : unmanaged
{
    protected readonly bool _isOwnedByCairo;
    protected readonly bool _needsDestroy;

    private T* _handle;

    protected CairoObject(T* handle, bool isOwnedByCairo = false, bool needsDestroy = true, bool allowNullPointer = false)
    {
        if (!allowNullPointer)
        {
            ArgumentNullException.ThrowIfNull(handle);
        }

        _handle         = handle;
        _isOwnedByCairo = isOwnedByCairo;
        _needsDestroy   = needsDestroy;
    }

    protected internal T* Handle => _handle;

    [DebuggerStepThrough]
    protected internal void CheckDisposed() => ObjectDisposedException.ThrowIf(_handle is null, this);

    protected sealed override void Dispose(bool disposing)
    {
        if (_handle is not null && _needsDestroy)
        {
            this.DisposeCore(_handle);
        }

        _handle = null;
    }

    protected abstract void DisposeCore(T* handle);

    /// <summary>
    /// Two <see cref="CairoObject"/>s are equal, when there native handles are equal.
    /// </summary>
    public bool Equals(CairoObject<T>? other)
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

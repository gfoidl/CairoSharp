// (c) gfoidl, all rights reserved

using System.Diagnostics;
using Cairo.Utilities;

namespace Cairo;

public abstract unsafe class CairoObject<T> : IDisposable, IEquatable<CairoObject<T>>
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

        CairoDebug.OnAllocated(handle);
    }

    protected internal T* Handle => _handle;

    [DebuggerStepThrough]
    protected internal void CheckDisposed() => ObjectDisposedException.ThrowIf(_handle is null, this);

    /// <summary>
    /// Releases the resources used by the wrapped cairo object.
    /// </summary>
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!disposing || CairoDebug.Enabled)
        {
            CairoDebug.OnDisposed(_handle, disposing, this.GetType());
        }

        if (_handle is not null && _needsDestroy)
        {
            this.DisposeCore(_handle);
        }

        _handle = null;
    }

    protected abstract void DisposeCore(T* handle);

    ~CairoObject() => this.Dispose(false);

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

// (c) gfoidl, all rights reserved

using Cairo.Utilities;

namespace Cairo;

/// <summary>
/// Base object for cairo.
/// </summary>
public abstract unsafe class CairoObject : IDisposable
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
}

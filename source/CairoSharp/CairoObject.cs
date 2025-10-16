// (c) gfoidl, all rights reserved

namespace Cairo;

// This type only exists to make the xml docs easier, as not every
// generic instantiation has to be given.

/// <summary>
/// Base object for cairo.
/// </summary>
public abstract unsafe class CairoObject : IDisposable
{
    /// <summary>
    /// Releases the resources used by the wrapped cairo object.
    /// </summary>
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected abstract void Dispose(bool disposing);

    ~CairoObject() => this.Dispose(false);
}

// (c) gfoidl, all rights reserved

namespace Cairo.Extensions.Loading;

/// <summary>
/// Base class for documents to be loaded into <see cref="CairoContext"/>.
/// </summary>
public abstract unsafe class Document : IDisposable
{
    public void Dispose()
    {
        this.DisposeCore();
        GC.SuppressFinalize(this);
    }

    protected abstract void DisposeCore();

    ~Document() => this.DisposeCore();

    protected abstract void CheckNotDisposed();
}

// (c) gfoidl, all rights reserved

using Cairo.Surfaces;
using static Cairo.Drawing.Patterns.PatternNative;

namespace Cairo.Drawing.Patterns;

/// <summary>
/// A <see cref="Pattern"/> for a <see cref="Surfaces.Surface"/>.
/// </summary>
public sealed unsafe class SurfacePattern : Pattern
{
    internal SurfacePattern(void* handle, bool isOwnedByCairo, bool needsDestroy = true)
        : base(handle, isOwnedByCairo, needsDestroy) { }

    /// <summary>
    /// Create a new <see cref="SurfacePattern"/> for the given surface.
    /// </summary>
    /// <param name="surface">the surface</param>
    public SurfacePattern(Surface surface) : base(cairo_pattern_create_for_surface(surface.Handle)) { }

    /// <summary>
    /// Gets the surface of a surface pattern.
    /// </summary>
    /// <remarks>
    /// The reference returned in surface is owned by the pattern.
    /// </remarks>
    public Surface Surface
    {
        get
        {
            this.CheckDisposed();

            Status status = cairo_pattern_get_surface(this.Handle, out void* surfaceHandle);

            status.ThrowIfStatus(Status.PatternTypeMismatch);

            return new Surface(surfaceHandle, isOwnedByCairo: true);
        }
    }
}

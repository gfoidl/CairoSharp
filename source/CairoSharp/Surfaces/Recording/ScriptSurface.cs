// (c) gfoidl, all rights reserved

using static Cairo.Surfaces.Recording.ScriptSurfaceNative;

namespace Cairo.Surfaces.Recording;

/// <summary>
/// Script Surfaces — Rendering to replayable scripts
/// </summary>
/// <remarks>
/// The script surface provides the ability to render to a native script that matches
/// the cairo drawing model. The scripts can be replayed using tools under the
/// util/cairo-script directory (of the native build), or with cairo-perf-trace.
/// </remarks>
public sealed unsafe class ScriptSurface : Surface
{
    /// <summary>
    /// Create a new surface that will emit its rendering through script.
    /// </summary>
    /// <param name="script">the script (output device)</param>
    /// <param name="content">the content of the surface</param>
    /// <param name="widthInPixels">width in pixels</param>
    /// <param name="heightInPixels">height in pixels</param>
    /// <returns>
    /// a pointer to the newly created surface. The caller owns the surface and should call <see cref="CairoObject.Dispose()"/>
    /// when done with it.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="script"/> is <c>null</c></exception>
    public ScriptSurface(ScriptDevice script, Content content, double widthInPixels, double heightInPixels)
        : base(CreateCore(script, content, widthInPixels, heightInPixels)) { }

    private static void* CreateCore(ScriptDevice script, Content content, double width, double height)
    {
        ArgumentNullException.ThrowIfNull(script);

        return cairo_script_surface_create(script.Handle, content, width, height);
    }

    /// <summary>
    /// Create a proxy surface that will render to <paramref name="target"/> and record the operations to
    /// <paramref name="script"/>.
    /// </summary>
    /// <param name="script">the script (output device)</param>
    /// <param name="target">a target surface to wrap</param>
    /// <returns>
    /// a pointer to the newly created surface. The caller owns the surface and should call <see cref="CairoObject.Dispose()"/>
    /// when done with it.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="script"/> is <c>null</c></exception>
    /// <exception cref="ArgumentNullException"><paramref name="target"/> is <c>null</c></exception>
    public ScriptSurface(ScriptDevice script, Surface target) : base(CreateCore(script, target)) { }

    private static void* CreateCore(ScriptDevice script, Surface target)
    {
        ArgumentNullException.ThrowIfNull(script);
        ArgumentNullException.ThrowIfNull(target);

        return cairo_script_surface_create_for_target(script.Handle, target.Handle);
    }
}

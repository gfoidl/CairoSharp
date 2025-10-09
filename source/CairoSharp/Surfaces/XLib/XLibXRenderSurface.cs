// (c) gfoidl, all rights reserved

using static Cairo.Surfaces.XLib.XLibXRenderSurfaceNative;
using Drawable = uint;

namespace Cairo.Surfaces.XLib;

/// <summary>
/// XLib-XRender Backend — X Window System rendering using XLib and the X Render extension
/// </summary>
/// <remarks>
/// The XLib surface is used to render cairo graphics to X Window System windows and pixmaps using
/// the XLib and Xrender libraries.
/// <para>
/// Note that the XLib surface automatically takes advantage of X Render extension if it is available.
/// </para>
/// </remarks>
public sealed unsafe class XLibXRenderSurface : Surface
{
    internal XLibXRenderSurface(void* handle, bool isOwnedByCairo = false) : base(handle, isOwnedByCairo) { }

    /// <summary>
    /// Creates an Xlib surface that draws to the given drawable. The way that colors are represented
    /// in the drawable is specified by the provided picture format.
    /// </summary>
    /// <param name="display">an X Display</param>
    /// <param name="drawable">an X Drawable, (a Pixmap or a Window)</param>
    /// <param name="screen">the X Screen associated with drawable </param>
    /// <param name="format">
    /// the picture format to use for drawing to drawable. The depth of format must match the depth of the drawable.
    /// </param>
    /// <param name="width">the current width of drawable.</param>
    /// <param name="height">the current height of drawable.</param>
    /// <remarks>
    /// Note: If drawable is a Window, then the method <see cref="XLibSurface.SetSize(int, int)"/> must
    /// be called whenever the size of the window changes.
    /// </remarks>
    /// <exception cref="CairoException">when construction fails</exception>
    public XLibXRenderSurface(IntPtr display, Drawable drawable, IntPtr screen, IntPtr format, int width, int height)
        : base(cairo_xlib_surface_create_with_xrender_format(display.ToPointer(), drawable, screen.ToPointer(), format.ToPointer(), width, height)) { }

    /// <summary>
    /// Gets the X Render picture format that surface uses for rendering with the X Render extension.
    /// </summary>
    /// <remarks>
    /// If the surface was created by <see cref="XLibXRenderSurface(nint, Drawable, nint, nint, int, int)"/>
    /// originally, the return value is the format passed to that constructor.
    /// </remarks>
    public IntPtr XRenderFormat
    {
        get
        {
            this.CheckDisposed();
            return new IntPtr(cairo_xlib_surface_get_xrender_format(this.Handle));
        }
    }
}

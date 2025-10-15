// (c) gfoidl, all rights reserved

using static Cairo.Surfaces.XLib.XLibSurfaceNative;
using Drawable = uint;
using Pixmap   = uint;

namespace Cairo.Surfaces.XLib;

/// <summary>
/// XLib Surfaces — X Window System rendering using XLib
/// </summary>
/// <remarks>
/// The XLib surface is used to render cairo graphics to X Window System
/// windows and pixmaps using the XLib library.
/// <para>
/// Note that the XLib surface automatically takes advantage of X render extension
/// if it is available.
/// </para>
/// </remarks>
public unsafe class XLibSurface : Surface
{
    internal XLibSurface(cairo_surface_t* surface, bool isOwnedByCairo = false, bool needsDestroy = true)
        : base(surface, isOwnedByCairo, needsDestroy) { }

    /// <summary>
    /// Creates an Xlib surface that draws to the given drawable. The way that colors are
    /// represented in the drawable is specified by the provided visual.
    /// </summary>
    /// <param name="display">an X Display</param>
    /// <param name="drawable">an X Drawable, (a Pixmap or a Window)</param>
    /// <param name="visual">
    /// the visual to use for drawing to drawable. The depth of the visual must match the depth
    /// of the drawable. Currently, only TrueColor visuals are fully supported.
    /// </param>
    /// <param name="width">the current width of drawable.</param>
    /// <param name="height">the current height of drawable.</param>
    /// <remarks>
    /// Note: If drawable is a Window, then the method <see cref="SetSize(int, int)"/> must be called whenever
    /// the size of the window changes.
    /// <para>
    /// When drawable is a Window containing child windows then drawing to the created surface will
    /// be clipped by those child windows. When the created surface is used as a source, the contents
    /// of the children will be included.
    /// </para>
    /// </remarks>
    /// <exception cref="CairoException">when construction fails</exception>
    public XLibSurface(IntPtr display, Drawable drawable, IntPtr visual, int width, int height)
        : base(cairo_xlib_surface_create(display.ToPointer(), drawable, visual.ToPointer(), width, height)) { }

    /// <summary>
    /// Creates an Xlib surface that draws to the given bitmap. This will be drawn to as a <see cref="Format.A1"/> object.
    /// </summary>
    /// <param name="display">an X Display</param>
    /// <param name="bitmap">an X Drawable, (a depth-1 Pixmap)</param>
    /// <param name="screen">the X Screen associated with bitmap </param>
    /// <param name="width">the current width of bitmap.</param>
    /// <param name="height">the current height of bitmap.</param>
    /// <returns>the newly created surface</returns>
    /// <exception cref="CairoException">when construction fails</exception>
    public static XLibSurface FromBitmap(IntPtr display, Pixmap bitmap, IntPtr screen, int width, int height)
    {
        cairo_surface_t* surface = cairo_xlib_surface_create_for_bitmap((uint*)display.ToPointer(), bitmap, (uint*)screen.ToPointer(), width, height);
        return new XLibSurface(surface);
    }

    /// <summary>
    /// Informs cairo of the new size of the X Drawable underlying the surface. For a surface created
    /// for a Window (rather than a Pixmap), this method must be called each time the size
    /// of the window changes. (For a subwindow, you are normally resizing the window yourself,
    /// but for a toplevel window, it is necessary to listen for ConfigureNotify events.)
    /// </summary>
    /// <param name="width">the new width of the surface</param>
    /// <param name="height">the new height of the surface</param>
    /// <remarks>
    /// A Pixmap can never change size, so it is never necessary to call this method on a
    /// surface created for a Pixmap.
    /// </remarks>
    public void SetSize(int width, int height)
    {
        this.CheckDisposed();
        cairo_xlib_surface_set_size(this.Handle, width, height);
    }

    /// <summary>
    /// Get the X Display for the underlying X Drawable.
    /// </summary>
    public IntPtr Display
    {
        get
        {
            this.CheckDisposed();
            return new IntPtr(cairo_xlib_surface_get_display(this.Handle));
        }
    }

    /// <summary>
    /// Get the X Screen for the underlying X Drawable.
    /// </summary>
    public IntPtr Screen
    {
        get
        {
            this.CheckDisposed();
            return new IntPtr(cairo_xlib_surface_get_screen(this.Handle));
        }
    }

    /// <summary>
    /// Informs cairo of a new X Drawable underlying the surface. The drawable must match
    /// the display, screen and format of the existing drawable or the application will
    /// get X protocol errors and will probably terminate. No checks are done by this
    /// method to ensure this compatibility.
    /// </summary>
    /// <param name="drawable">the new drawable for the surface</param>
    /// <param name="width">the width of the new drawable</param>
    /// <param name="height">the height of the new drawable</param>
    public void SetDrawable(Drawable drawable, int width, int height)
    {
        this.CheckDisposed();
        cairo_xlib_surface_set_drawable(this.Handle, drawable, width, height);
    }

    /// <summary>
    /// Get the underlying X Drawable used for the surface.
    /// </summary>
    public Drawable Drawable
    {
        get
        {
            this.CheckDisposed();
            return cairo_xlib_surface_get_drawable(this.Handle);
        }
    }

    /// <summary>
    /// Gets the X Visual associated with surface , suitable for use with the underlying X Drawable.
    /// If surface was created by <see cref="XLibSurface(nint, Drawable, nint, int, int)"/>, the return value
    /// is the Visual passed to that constructor.
    /// </summary>
    public IntPtr Visual
    {
        get
        {
            this.CheckDisposed();
            return new IntPtr(cairo_xlib_surface_get_visual(this.Handle));
        }
    }

    /// <summary>
    /// Get the width of the X Drawable underlying the surface in pixels.
    /// </summary>
    public int Width
    {
        get
        {
            this.CheckDisposed();
            return cairo_xlib_surface_get_width(this.Handle);
        }
    }

    /// <summary>
    /// Get the height of the X Drawable underlying the surface in pixels.
    /// </summary>
    public int Height
    {
        get
        {
            this.CheckDisposed();
            return cairo_xlib_surface_get_height(this.Handle);
        }
    }

    /// <summary>
    /// Get the number of bits used to represent each pixel value.
    /// </summary>
    public int Depth
    {
        get
        {
            this.CheckDisposed();
            return cairo_xlib_surface_get_depth(this.Handle);
        }
    }

    /// <summary>
    /// Restricts all future Xlib surfaces for this devices to the specified version of the
    /// RENDER extension. This method exists solely for debugging purpose. It lets you
    /// find out how cairo would behave with an older version of the RENDER extension.
    /// </summary>
    /// <param name="majorVersion">major version to restrict to</param>
    /// <param name="minorVersion">minor version to restrict to</param>
    /// <remarks>
    /// Use the special values -1 and -1 for disabling the RENDER extension.
    /// </remarks>
    /// <exception cref="CairoException">when there is no device associated</exception>
    public void DebugCapXrenderVersion(int majorVersion, int minorVersion)
    {
        this.CheckDisposed();

        Device device = this.GetDeviceOrThrow();
        cairo_xlib_device_debug_cap_xrender_version(device.Handle, majorVersion, minorVersion);
    }

    /// <summary>
    /// Render supports two modes of precision when rendering trapezoids. Set the precision to the desired mode.
    /// </summary>
    public int DebugPrecision
    {
        get
        {
            this.CheckDisposed();

            Device device = this.GetDeviceOrThrow();
            return cairo_xlib_device_debug_get_precision(device.Handle);
        }
        set
        {
            this.CheckDisposed();

            Device device = this.GetDeviceOrThrow();
            cairo_xlib_device_debug_set_precision(device.Handle, value);
        }
    }
}

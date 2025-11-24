// (c) gfoidl, all rights reserved

using static Cairo.Surfaces.XCB.XCBSurfaceNative;

namespace Cairo.Surfaces.XCB;

/// <summary>
/// XCB Surfaces — X Window System rendering using the XCB library
/// </summary>
/// <remarks>
/// The XCB surface is used to render cairo graphics to X Window System windows and
/// pixmaps using the XCB library.
/// <para>
/// Note that the XCB surface automatically takes advantage of the X render extension if it is available.
/// </para>
/// </remarks>
public sealed unsafe class XCBSurface : Surface
{
    internal XCBSurface(cairo_surface_t* surface, bool isOwnedByCairo = false, bool needsDestroy = true)
        : base(surface, isOwnedByCairo, needsDestroy) { }

    /// <summary>
    /// Creates an XCB surface that draws to the given drawable. The way that colors are represented
    /// in the drawable is specified by the provided visual.
    /// </summary>
    /// <param name="connection">an XCB connection</param>
    /// <param name="drawable">an XCB drawable</param>
    /// <param name="visual">
    /// the visual to use for drawing to drawable. The depth of the visual must match the depth of the
    /// drawable. Currently, only TrueColor visuals are fully supported.
    /// </param>
    /// <param name="widht">the current width of drawable </param>
    /// <param name="height">the current height of drawable </param>
    /// <remarks>
    /// This constructor always returns a valid pointer, but it will return a pointer to a "nil" surface
    /// if an error such as out of memory occurs. You can use <see cref="Surface.Status"/>
    /// to check for this.
    /// </remarks>
    /// <exception cref="CairoException">when construction fails</exception>
    public XCBSurface(IntPtr connection, uint drawable, IntPtr visual, int widht, int height)
        : base(cairo_xcb_surface_create((uint*)connection.ToPointer(), drawable, (uint*)visual.ToPointer(), widht, height)) { }

    /// <summary>
    /// Creates an XCB surface that draws to the given bitmap. This will be drawn to as a <see cref="Format.A1"/> object.
    /// </summary>
    /// <param name="connection">an XCB connection</param>
    /// <param name="screen">the XCB screen associated with bitmap </param>
    /// <param name="bitmap">an XCB drawable (a Pixmap with depth 1)</param>
    /// <param name="width">the current width of bitmap </param>
    /// <param name="height">the current height of bitmap </param>
    /// <returns>
    /// a pointer to the newly created surface. The caller owns the surface and should call <see cref="CairoObject.Dispose()"/>
    /// when done with it.
    /// <para>
    /// This method always returns a valid pointer, but it will return a pointer to a "nil" surface if an error
    /// such as out of memory occurs. You can use <see cref="Surface.Status"/> to check for this.
    /// </para>
    /// </returns>
    /// <exception cref="CairoException">when construction fails</exception>
    public static XCBSurface FromBitmap(IntPtr connection, IntPtr screen, uint bitmap, int width, int height)
    {
        cairo_surface_t* surface = cairo_xcb_surface_create_for_bitmap((uint*)connection.ToPointer(), (uint*)screen.ToPointer(), bitmap, width, height);
        return new XCBSurface(surface);
    }

    /// <summary>
    /// Creates an XCB surface that draws to the given drawable. The way that colors are represented in
    /// the drawable is specified by the provided picture format.
    /// </summary>
    /// <param name="connection">an XCB connection</param>
    /// <param name="screen">the XCB screen associated with drawable </param>
    /// <param name="drawable">an XCB drawable</param>
    /// <param name="format">
    /// the picture format to use for drawing to drawable. The depth of format mush match the depth
    /// of the drawable.
    /// </param>
    /// <param name="width">the current width of drawable </param>
    /// <param name="height">the current height of drawable </param>
    /// <remarks>
    /// Note: If drawable is a Window, then the property <see cref="SetSize"/> must be called whenever
    /// the size of the window changes.
    /// <para>
    /// When drawable is a Window containing child windows then drawing to the created surface will be
    /// clipped by those child windows. When the created surface is used as a source, the contents of the
    /// children will be included.
    /// </para>
    /// </remarks>
    /// <exception cref="CairoException">when construction fails</exception>
    public XCBSurface(IntPtr connection, IntPtr screen, uint drawable, IntPtr format, int width, int height)
        : base(cairo_xcb_surface_create_with_xrender_format(connection.ToPointer(), screen.ToPointer(), drawable, format.ToPointer(), width, height)) { }

    /// <summary>
    /// Informs cairo of the new size of the XCB drawable underlying the surface. For a
    /// surface created for a window (rather than a pixmap), this method must be called
    /// each time the size of the window changes. (For a subwindow, you are normally resizing
    /// the window yourself, but for a toplevel window, it is necessary to listen for
    /// ConfigureNotify events.)
    /// </summary>
    /// <param name="width">the new width of the surface</param>
    /// <param name="height">the new height of the surface</param>
    /// <remarks>
    /// A pixmap can never change size, so it is never necessary to call this method on a
    /// surface created for a pixmap.
    /// <para>
    /// If <see cref="Surface.Flush"/> wasn't called, some pending operations might be discarded.
    /// </para>
    /// </remarks>
    public void SetSize(int width, int height)
    {
        this.CheckDisposed();
        cairo_xcb_surface_set_size(this.Handle, width, height);
    }

    /// <summary>
    /// Informs cairo of the new drawable and size of the XCB drawable underlying the surface.
    /// </summary>
    /// <param name="drawable">the new drawable of the surface</param>
    /// <param name="width">the new width of the surface</param>
    /// <param name="height">the new height of the surface</param>
    /// <remarks>
    /// If <see cref="Surface.Flush"/> wasn't called, some pending operations might be discarded.
    /// </remarks>
    public void SetDrawable(uint drawable, int width, int height)
    {
        this.CheckDisposed();
        cairo_xcb_surface_set_drawable(this.Handle, drawable, width, height);
    }

    /// <summary>
    /// Get the connection for the XCB device.
    /// </summary>
    public IntPtr Connection
    {
        get
        {
            this.CheckDisposed();

            Device device = this.GetDeviceOrThrow();
            return new IntPtr(cairo_xcb_device_get_connection(device.Handle));
        }
    }

    /// <summary>
    /// Restricts all future XCB surfaces for this devices to the specified version of
    /// the RENDER extension. This method exists solely for debugging purpose. It let's
    /// you find out how cairo would behave with an older version of the RENDER extension.
    /// </summary>
    /// <param name="majorVersion">major version to restrict to</param>
    /// <param name="minorVersion">minor version to restrict to</param>
    /// <remarks>
    /// Use the special values -1 and -1 for disabling the RENDER extension.
    /// </remarks>
    public void DebugCapXrenderVersion(int majorVersion, int minorVersion)
    {
        this.CheckDisposed();

        Device device = this.GetDeviceOrThrow();
        cairo_xcb_device_debug_cap_xrender_version(device.Handle, majorVersion, minorVersion);
    }

    /// <summary>
    /// Restricts all future XCB surfaces for this devices to the specified version of the
    /// SHM extension. This method exists solely for debugging purpose. It let's you find
    /// out how cairo would behave with an older version of the SHM extension.
    /// </summary>
    /// <param name="majorVersion">major version to restrict to</param>
    /// <param name="minorVersion">minor version to restrict to</param>
    /// <remarks>
    /// Use the special values -1 and -1 for disabling the SHM extension.
    /// </remarks>
    public void DebugCapXshmVersion(int majorVersion, int minorVersion)
    {
        this.CheckDisposed();

        Device device = this.GetDeviceOrThrow();
        cairo_xcb_device_debug_cap_xshm_version(device.Handle, majorVersion, minorVersion);
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
            return cairo_xcb_device_debug_get_precision(device.Handle);
        }
        set
        {
            this.CheckDisposed();

            Device device = this.GetDeviceOrThrow();
            cairo_xcb_device_debug_set_precision(device.Handle, value);
        }
    }
}

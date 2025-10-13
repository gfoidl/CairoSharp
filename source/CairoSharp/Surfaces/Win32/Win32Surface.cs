// (c) gfoidl, all rights reserved

using Cairo.Surfaces.Images;
using static Cairo.Surfaces.Win32.Win32SurfaceNative;

namespace Cairo.Surfaces.Win32;

/// <summary>
/// Win32 Surfaces — Microsoft Windows surface support
/// </summary>
/// <remarks>
/// The Microsoft Windows surface is used to render cairo graphics to Microsoft Windows
/// windows, bitmaps, and printing device contexts.
/// </remarks>
public sealed unsafe class Win32Surface : Surface
{
    internal Win32Surface(void* handle, bool isOwnedByCairo = false, bool needsDestroy = true)
        : base(handle, isOwnedByCairo, needsDestroy) { }

    /// <summary>
    /// Creates a cairo surface that targets the given DC. The DC will be queried for its initial
    /// clip extents, and this will be used as the size of the cairo surface. The resulting
    /// surface will always be of format <see cref="Format.Rgb24"/>; should you need another surface
    /// format, you will need to create one through the other constructors.
    /// </summary>
    /// <param name="hdc">the DC to create a surface for</param>
    /// <exception cref="ArgumentNullException">when the surface could not be created due to a failure</exception>
    public Win32Surface(IntPtr hdc) : base(cairo_win32_surface_create(hdc.ToPointer())) { }

    /// <summary>
    /// Creates a device-independent-bitmap surface not associated with any particular existing
    /// surface or device context. The created bitmap will be uninitialized.
    /// </summary>
    /// <param name="format">format of pixels in the surface to create</param>
    /// <param name="width">width of the surface, in pixels</param>
    /// <param name="height">height of the surface, in pixels</param>
    /// <exception cref="ArgumentNullException">when the surface could not be created due to a failure</exception>
    public Win32Surface(Format format, int width, int height)
        : base(cairo_win32_surface_create_with_dib(format, width, height)) { }

    /// <summary>
    /// Creates a device-dependent-bitmap surface not associated with any particular
    /// existing surface or device context. The created bitmap will be uninitialized.
    /// </summary>
    /// <param name="hdc">a DC compatible with the surface to create</param>
    /// <param name="format">format of pixels in the surface to create</param>
    /// <param name="width">width of the surface, in pixels</param>
    /// <param name="height">height of the surface, in pixels</param>
    /// <exception cref="ArgumentNullException">when the surface could not be created due to a failure</exception>
    public Win32Surface(IntPtr hdc, Format format, int width, int height)
        : base(cairo_win32_surface_create_with_ddb(hdc.ToPointer(), format, width, height)) { }

    /// <summary>
    /// Creates a cairo surface that targets the given DC. The DC will be queried for its initial clip
    /// extents, and this will be used as the size of the cairo surface.
    /// </summary>
    /// <param name="hdc">the DC to create a surface for</param>
    /// <param name="format">format of pixels in the surface to create</param>
    /// <remarks>
    /// Supported formats are: <see cref="Format.Argb32"/>, <see cref="Format.Rgb24"/>
    /// <para>
    /// Note: format only tells cairo how to draw on the surface, not what the format of
    /// the surface is. Namely, cairo does not (and cannot) check that hdc actually supports
    /// alpha-transparency.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">when the surface could not be created due to a failure</exception>
    public Win32Surface(IntPtr hdc, Format format) : base(cairo_win32_surface_create_with_format(hdc.ToPointer(), format)) { }

    /// <summary>
    /// Creates a cairo surface that targets the given DC. The DC will be queried for its
    /// initial clip extents, and this will be used as the size of the cairo surface.
    /// The DC should be a printing DC; antialiasing will be ignored, and GDI will be
    /// used as much as possible to draw to the surface.
    /// </summary>
    /// <param name="hdc">the DC to create a surface for</param>
    /// <returns>the newly created surface</returns>
    /// <remarks>
    /// The returned surface will be wrapped using the paginated surface to provide correct
    /// complex rendering behaviour; <see cref="Surface.ShowPage"/> and associated methods must
    /// be used for correct output.
    /// <para>
    /// The following mime types are supported on source patterns: <see cref="MimeTypes.Jpeg"/>,
    /// <see cref="MimeTypes.Png"/>.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">when the surface could not be created due to a failure</exception>
    public static Win32Surface CreatePrintingSurface(IntPtr hdc)
    {
        void* handle = cairo_win32_printing_surface_create(hdc.ToPointer());
        return new Win32Surface(handle);
    }

    /// <summary>
    /// Returns the HDC associated with this surface, or <see cref="IntPtr.Zero"/> if none.
    /// Also returns <see cref="IntPtr.Zero"/> if the surface is not a win32 surface.
    /// </summary>
    /// <remarks>
    /// A call to <see cref="Surface.Flush"/> is required before using the HDC to ensure
    /// that all pending drawing operations are finished and to restore any temporary modification
    /// cairo has made to its state. A call to <see cref="Surface.MarkDirty()"/> is required after
    /// the state or the content of the HDC has been modified.
    /// </remarks>
    public IntPtr Hdc
    {
        get
        {
            this.CheckDisposed();
            return new IntPtr(cairo_win32_surface_get_dc(this.Handle));
        }
    }

    /// <summary>
    /// Returns a <see cref="ImageSurface"/> that refers to the same bits as the DIB of the
    /// Win32 surface. If the passed-in win32 surface is not a DIB surface, <c>null</c> is returned.
    /// </summary>
    /// <returns>
    /// a <see cref="ImageSurface"/> (owned by the <see cref="Win32Surface"/>), or <c>null</c> if the win32
    /// surface is not a DIB.
    /// </returns>
    public ImageSurface? GetImage()
    {
        this.CheckDisposed();

        void* handle = cairo_win32_surface_get_image(this.Handle);

        if (handle is null)
        {
            return null;
        }

        return new ImageSurface(handle, isOwnedByCairo: true, needsDestroy: /* not documented in cairo */ false);
    }
}

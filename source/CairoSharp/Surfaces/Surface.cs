// (c) gfoidl, all rights reserved

using System.Buffers;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Cairo.Fonts;
using Cairo.Fonts.Scaled;
using Cairo.Surfaces.Images;
using Cairo.Surfaces.PDF;
using Cairo.Surfaces.PostScript;
using Cairo.Surfaces.SVG;
using Cairo.Surfaces.Win32;
using Cairo.Surfaces.XCB;
using Cairo.Surfaces.XLib;
using static Cairo.Surfaces.Images.PngSupportNative;
using static Cairo.Surfaces.SurfaceNative;

namespace Cairo.Surfaces;

[EditorBrowsable(EditorBrowsableState.Never)]
public struct cairo_surface_t;

/// <summary>
/// Base class for surfaces
/// </summary>
/// <remarks>
/// <see cref="Surface"/> is the abstract type representing all different drawing targets that cairo can render
/// to. The actual drawings are performed using a <see cref="CairoContext"/>.
/// <para>
/// A cairo surface is created by using backend-specific constructors
/// </para>
/// <para>
/// Most surface types allow accessing the surface without using cairo functions. If you do this, keep in mind
/// that it is mandatory that you call <see cref="Flush"/> before reading from or writing to the surface and
/// that you must use <see cref="MarkDirty()"/> after modifying it.
/// </para>
/// <para>
/// Example: Directly modifying an image surface
/// <code>
/// void ModifyImageSurface(ImageSurface surface)
/// {
///     // flush to ensure all writing to the image was done
///     surface.Flush();
///
///     // modify the image
///     ModifyImageData(surface.Data, surface.Width, surface.Height, surface.Stride);
///
///     // mark the image dirty so cairo clears its caches.
///     surface.MarkDirty();
/// }
/// </code>
/// </para>
/// <para>
/// Note that for other surface types it might be necessary to acquire the surface's device first.
/// See <see cref="Device.Acquire"/> for a discussion of devices.
/// </para>
/// </remarks>
public unsafe class Surface : CairoObject<cairo_surface_t>
{
    internal Surface(cairo_surface_t* surface, bool isOwnedByCairo = false, bool needsDestroy = true)
        : base(surface, isOwnedByCairo, needsDestroy)
    {
        this.Status.ThrowIfNotSuccess();

        if (isOwnedByCairo && needsDestroy)
        {
            cairo_surface_reference(surface);
        }
    }

    protected override void DisposeCore(cairo_surface_t* surface)
    {
        cairo_surface_destroy(surface);

        PrintDebugInfo(surface);
        [Conditional("DEBUG")]
        static void PrintDebugInfo(cairo_surface_t* surface)
        {
            uint rc = cairo_surface_get_reference_count(surface);
            Debug.WriteLine($"Surface 0x{(nint)surface}: reference count = {rc}");
        }
    }

    /// <summary>
    /// Create a new surface that is as compatible as possible with an existing surface. For example the new
    /// surface will have the same device scale, fallback resolution and font options as this one.
    /// Generally, the new surface will also use the same backend as this one, unless that is not possible
    /// for some reason. The type of the returned surface may be examined with <see cref="SurfaceType"/>.
    /// </summary>
    /// <param name="content">the content for the new surface</param>
    /// <param name="widthInDeviceSpaceUnits">width of the new surface, (in device-space units)</param>
    /// <param name="heightInDeviceSpaceUnits">height of the new surface (in device-space units)</param>
    /// <returns>
    /// a pointer to the newly allocated surface. The caller owns the surface and should call
    /// <see cref="CairoObject.Dispose()"/> when done with it.
    /// <para>
    /// This constructor always returns a valid pointer, but it will return a pointer to a "nil" surface
    /// if other is already in an error state or any other error occurs.
    /// </para>
    /// </returns>
    /// <exception cref="CairoException">when construction fails</exception>
    public Surface CreateSimilar(Content content, int widthInDeviceSpaceUnits, int heightInDeviceSpaceUnits)
    {
        this.CheckDisposed();

        cairo_surface_t* handle = cairo_surface_create_similar(this.Handle, content, widthInDeviceSpaceUnits, heightInDeviceSpaceUnits);
        return new Surface(handle);
    }

    /// <summary>
    /// Create a new image surface that is as compatible as possible for uploading to and the use in conjunction
    /// with an existing surface. However, this surface can still be used like any normal image surface. Unlike
    /// <see cref="CreateSimilar(Content, int, int)"/> the new image surface won't inherit the device scale from this one.
    /// </summary>
    /// <param name="format">the format for the new surface</param>
    /// <param name="width">width of the new surface, (in pixels)</param>
    /// <param name="height">height of the new surface (in pixels)</param>
    /// <returns>
    /// a pointer to the newly allocated image surface. The caller owns the surface and should call
    /// <see cref="CairoObject.Dispose()"/> when done with it.
    /// <para>
    /// This method always returns a valid pointer, but it will return a pointer to a "nil" surface
    /// if other is already in an error state or any other error occurs.
    /// </para>
    /// </returns>
    /// <remarks>
    /// Initially the surface contents are all 0 (transparent if contents have transparency, black otherwise.)
    /// <para>
    /// Use <see cref="CreateSimilar(Content, int, int)"/> if you don't need an image surface.
    /// </para>
    /// </remarks>
    /// <exception cref="CairoException">when construction fails</exception>
    public ImageSurface CreateSimilarImage(Format format, int width, int height)
    {
        this.CheckDisposed();

        cairo_surface_t* handle = cairo_surface_create_similar_image(this.Handle, format, width, height);
        return new ImageSurface(handle);
    }

    /// <summary>
    /// Create a new surface that is a rectangle within the target surface. All operations drawn to this surface
    /// are then clipped and translated onto the target surface. Nothing drawn via this sub-surface outside of
    /// its bounds is drawn onto the target surface, making this a useful method for passing constrained child
    /// surfaces to library routines that draw directly onto the parent surface, i.e. with no further backend
    /// allocations, double buffering or copies.
    /// </summary>
    /// <param name="x">the x-origin of the sub-surface from the top-left of the target surface (in device-space units)</param>
    /// <param name="y">the y-origin of the sub-surface from the top-left of the target surface (in device-space units)</param>
    /// <param name="width">width of the sub-surface (in device-space units)</param>
    /// <param name="height">height of the sub-surface (in device-space units)</param>
    /// <returns>
    /// a pointer to the newly allocated surface. The caller owns the surface and should call
    /// <see cref="CairoObject.Dispose()"/> when done with it.
    /// <para>
    /// This method always returns a valid pointer, but it will return a pointer to a "nil" surface if other is already
    /// in an error state or any other error occurs.
    /// </para>
    /// </returns>
    public Surface CreateForRectangle(double x, double y, double width, double height)
    {
        this.CheckDisposed();

        cairo_surface_t* surface = cairo_surface_create_for_rectangle(this.Handle, x, y, width, height);
        return new Surface(surface);
    }

    /// <summary>
    /// Checks whether an error has previously occurred for this surface.
    /// </summary>
    public Status Status
    {
        get
        {
            this.CheckDisposed();
            return cairo_surface_status(this.Handle);
        }
    }

    /// <summary>
    /// This method finishes the surface and drops all references to external resources. For example,
    /// for the Xlib backend it means that cairo will no longer access the drawable, which can be freed.
    /// After calling <see cref="Finish"/> the only valid operations on a surface are checking status, getting
    /// and setting user data, referencing and destroying, and flushing and finishing it. Further drawing to the
    /// surface will not affect the surface but will instead trigger a <see cref="Status.SurfaceFinished"/> error.
    /// </summary>
    /// <remarks>
    /// When the last call to <see cref="CairoObject.Dispose()"/> decreases the reference count to zero,
    /// cairo will call <c>cairo_surface_finish()</c> if it hasn't been called already, before freeing the resources
    /// associated with the surface.
    /// </remarks>
    public void Finish()
    {
        this.CheckDisposed();
        cairo_surface_finish(this.Handle);
    }

    /// <summary>
    /// Do any pending drawing for the surface and also restore any temporary modifications cairo has made
    /// to the surface's state. This method must be called before switching from drawing on the surface
    /// with cairo to drawing on it directly with native APIs, or accessing its memory outside of cairo.
    /// If the surface doesn't support direct access, then this method does nothing.
    /// </summary>
    public void Flush()
    {
        this.CheckDisposed();
        cairo_surface_flush(this.Handle);
    }

    /// <summary>
    /// This property returns the device for a surface -- or -- <c>null</c> if the surface does
    /// not have an associated device.
    /// </summary>
    /// <remarks>
    /// Ownership isn't transferred, so don't call <see cref="CairoObject.Dispose()"/> on the
    /// returned <see cref="Device"/>.
    /// </remarks>
    public Device? Device
    {
        get
        {
            this.CheckDisposed();
            cairo_device_t* device = cairo_surface_get_device(this.Handle);

            if (device is null)
            {
                return null;
            }

            return new Device(device);
        }
    }

    protected Device GetDeviceOrThrow() => this.Device ?? throw new CairoException("No device is associated with this surface");

    /// <summary>
    /// Retrieves the default font rendering options for the surface. This allows display surfaces to
    /// report the correct subpixel order for rendering on them, print surfaces to disable hinting
    /// of metrics and so forth. The result can then be used with
    /// <see cref="ScaledFont(FontFace, ref Matrix, ref Matrix, FontOptions)"/>.
    /// </summary>
    /// <param name="options">
    /// a <see cref="FontOptions"/> object into which to store the retrieved options. All existing values are overwritten
    /// </param>
    public void GetFontOptions(FontOptions options)
    {
        this.CheckDisposed();
        ArgumentNullException.ThrowIfNull(options);

        cairo_surface_get_font_options(this.Handle, options.Handle);
    }

    /// <summary>
    /// This property returns the content type of <see cref="Surface"/> which indicates whether the
    /// surface contains color and/or alpha information.
    /// </summary>
    public Content Content
    {
        get
        {
            this.CheckDisposed();
            return cairo_surface_get_content(this.Handle);
        }
    }

    /// <summary>
    /// Tells cairo that drawing has been done to surface using means other than cairo, and that
    /// cairo should reread any cached areas. Note that you must call <see cref="Flush"/> before
    /// doing such drawing.
    /// </summary>
    public void MarkDirty()
    {
        this.CheckDisposed();
        cairo_surface_mark_dirty(this.Handle);
    }

    /// <summary>
    /// Like <see cref="MarkDirty()"/>, but drawing has been done only to the specified rectangle,
    /// so that cairo can retain cached contents for other parts of the surface.
    /// </summary>
    /// <param name="rectangle">The dirty rectangle</param>
    public void MarkDirty(Rectangle rectangle)
    {
        this.CheckDisposed();
        cairo_surface_mark_dirty_rectangle(
            this.Handle,
            (int)rectangle.X,
            (int)rectangle.Y,
            (int)rectangle.Width,
            (int)rectangle.Height);
    }

    /// <summary>
    /// Sets an offset that is added to the device coordinates determined by the CTM when drawing
    /// to surface. One use case for this property is when we want to create a <see cref="Surface"/>
    /// that redirects drawing for a portion of an onscreen surface to an offscreen surface in a way that
    /// is completely invisible to the user of the cairo API. Setting a transformation via
    /// <see cref="TransformationsExtensions.Translate(CairoContext, double, double)"/>
    /// isn't sufficient to do this, since functions like
    /// <see cref="TransformationsExtensions.DeviceToUser(CairoContext, ref double, ref double)"/> will
    /// expose the hidden offset.
    /// </summary>
    public PointD DeviceOffset
    {
        get
        {
            this.CheckDisposed();
            cairo_surface_get_device_offset(this.Handle, out double xOffset, out double yOffset);
            return new PointD(xOffset, yOffset);
        }
        set
        {
            this.CheckDisposed();
            cairo_surface_set_device_offset(this.Handle, value.X, value.Y);
        }
    }

    /// <summary>
    /// Sets a scale that is multiplied to the device coordinates determined by the CTM when drawing
    /// to surface. One common use for this is to render to very high resolution display devices at
    /// a scale factor, so that code that assumes 1 pixel will be a certain size will still work.
    /// Setting a transformation via <see cref="TransformationsExtensions.Scale(CairoContext, double, double)"/>
    /// isn't sufficient to do this, since functions like
    /// <see cref="TransformationsExtensions.DeviceToUser(CairoContext, ref double, ref double)"/> will
    /// expose the hidden scale.
    /// </summary>
    public PointD DeviceScale
    {
        get
        {
            this.CheckDisposed();
            cairo_surface_get_device_scale(this.Handle, out double xScale, out double yScale);
            return new PointD(xScale, yScale);
        }
        set
        {
            this.CheckDisposed();
            cairo_surface_set_device_scale(this.Handle, value.X, value.Y);
        }
    }

    /// <summary>
    /// Set the horizontal and vertical resolution for image fallbacks.
    /// </summary>
    /// <remarks>
    /// When certain operations aren't supported natively by a backend, cairo will fallback by rendering
    /// operations to an image and then overlaying that image onto the output. For backends that are natively
    /// vector-oriented, this property can be used to set the resolution used for these image fallbacks,
    /// (larger values will result in more detailed images, but also larger file sizes).
    /// <para>
    /// Some examples of natively vector-oriented backends are the ps, pdf, and svg backends.
    /// </para>
    /// <para>
    /// For backends that are natively raster-oriented, image fallbacks are still possible, but they
    /// are always performed at the native device resolution. So this property has no effect on those backends.
    /// </para>
    /// <para>
    /// Note: The fallback resolution only takes effect at the time of completing a page (with <see cref="CairoContext.ShowPage"/>
    /// or <see cref="CairoContext.CopyPage"/>) so there is currently no way to have more than one fallback resolution in
    /// effect on a single page.
    /// </para>
    /// <para>
    /// The default fallback resolution is 300 pixels per inch in both dimensions.
    /// </para>
    /// </remarks>
    public (double xPixelsPerInch, double yPixelsPerInch) FallbackResolution
    {
        get
        {
            this.CheckDisposed();
            cairo_surface_get_fallback_resolution(this.Handle, out double x, out double y);
            return (x, y);
        }
        set
        {
            this.CheckDisposed();
            cairo_surface_set_fallback_resolution(this.Handle, value.xPixelsPerInch, value.yPixelsPerInch);
        }
    }

    /// <summary>
    /// This property returns the type of the backend used to create a surface.
    /// </summary>
    public SurfaceType SurfaceType
    {
        get
        {
            this.CheckDisposed();
            return cairo_surface_get_type(this.Handle);
        }
    }

    /// <summary>
    /// Returns the current reference count of surface.  If the object is a nil object, 0 will be returned.
    /// </summary>
    internal int ReferenceCount
    {
        get
        {
            this.CheckDisposed();
            return (int)cairo_surface_get_reference_count(this.Handle);
        }
    }

    /// <summary>
    /// Emits the current page for backends that support multiple pages, but doesn't clear it, so that
    /// the contents of the current page will be retained for the next page. Use <see cref="ShowPage"/>
    /// if you want to get an empty page after the emission.
    /// </summary>
    /// <remarks>
    /// There is a convenience method for this, namely <see cref="CairoContext.CopyPage"/>.
    /// </remarks>
    public void CopyPage()
    {
        this.CheckDisposed();
        cairo_surface_copy_page(this.Handle);
    }

    /// <summary>
    /// Emits and clears the current page for backends that support multiple pages. Use <see cref="CopyPage"/> if
    /// you don't want to clear the page.
    /// </summary>
    /// <remarks>
    /// There is a convenience method for this, namely <see cref="CairoContext.ShowPage"/>.
    /// </remarks>
    public void ShowPage()
    {
        this.CheckDisposed();
        cairo_surface_show_page(this.Handle);
    }

    /// <summary>
    /// Returns whether the surface supports sophisticated cairo_show_text_glyphs() operations.
    /// That is, whether it actually uses the provided text and cluster data to a cairo_show_text_glyphs() call.
    /// <para>
    /// <c>true</c> if the surface supports cairo_show_text_glyphs(), <c>false</c> otherwise.
    /// </para>
    /// </summary>
    /// <remarks>
    /// Note: Even if this property returns <c>false</c>, a cairo_show_text_glyphs() operation targeted
    /// at surface will still succeed. It just will act like a cairo_show_glyphs() operation. Users can
    /// use this property to avoid computing UTF-8 text and cluster mapping if the target surface does not use it.
    /// </remarks>
    public bool HasShowTextGlyphs
    {
        get
        {
            this.CheckDisposed();
            return cairo_surface_has_show_text_glyphs(this.Handle);
        }
    }

    /// <summary>
    /// Attach an image in the format mime_type to surface. To remove the data from a surface,
    /// call this method with same mime type and NULL for data.
    /// </summary>
    /// <param name="mimeType">the MIME type of the image data</param>
    /// <param name="data">the image data to attach to the surface</param>
    /// <remarks>
    /// The attached image (or filename) data can later be used by backends which support it (currently:
    /// PDF, PS, SVG and Win32 Printing surfaces) to emit this data instead of making a snapshot of
    /// the surface. This approach tends to be faster and requires less memory and disk space.
    /// <para>
    /// The recognized MIME types are the following: <see cref="MimeTypes.Jpeg"/>, <see cref="MimeTypes.Png"/>,
    /// <see cref="MimeTypes.Jp2"/>, <see cref="MimeTypes.Uri"/>, <see cref="MimeTypes.UniqueId"/>,
    /// <see cref="MimeTypes.Jbig2"/>, <see cref="MimeTypes.Jbig2Global"/>, <see cref="MimeTypes.Jbig2GlobalId"/>,
    /// <see cref="MimeTypes.CcittFax"/>, <see cref="MimeTypes.CcittFaxParams"/>.
    /// </para>
    /// <para>
    /// See corresponding backend surface docs for details about which MIME types it can handle. Caution:
    /// the associated MIME data will be discarded if you draw on the surface afterwards. Use this method with care.
    /// </para>
    /// <para>
    /// Even if a backend supports a MIME type, that does not mean cairo will always be able to use the
    /// attached MIME data. For example, if the backend does not natively support the compositing operation
    /// used to apply the MIME data to the backend. In that case, the MIME data will be ignored. Therefore,
    /// to apply an image in all cases, it is best to create an image surface which contains the decoded image
    /// data and then attach the MIME data to that. This ensures the image will always be used while still allowing
    /// the MIME data to be used whenever possible.
    /// </para>
    /// <para>
    /// An example is given in
    /// <a href="https://gitlab.com/saiwp/cairo/-/blob/master/test/pdf-mime-data.c?ref_type=heads#L34-39">this C test</a>.
    /// </para>
    /// </remarks>
    public void SetMimeData(string mimeType, ReadOnlySpan<byte> data)
    {
        this.CheckDisposed();

        if (data.IsEmpty)
        {
            return;
        }

        cairo_destroy_func_t destroyFunc = &EmptyDestroyFunction;

        fixed (byte* ptr = &MemoryMarshal.GetReference(data))
        {
            Status status = cairo_surface_set_mime_data(
                this.Handle,
                mimeType,
                ptr,
                new CULong((uint)data.Length),
                destroyFunc,
                null);

            status.ThrowIfStatus(Status.NoMemory);
        }

        // Let the GC do its work.
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        static void EmptyDestroyFunction(void* state) { }
    }

    /// <summary>
    /// Return mime data previously attached to surface using the specified mime type. If no
    /// data has been attached with the given mime type, data is set NULL.
    /// </summary>
    /// <param name="mimeType">the mime type of the image data</param>
    /// <returns>A span with the data or an empty span</returns>
    public ReadOnlySpan<byte> GetMimeDate(string mimeType)
    {
        this.CheckDisposed();

        cairo_surface_get_mime_data(this.Handle, mimeType, out byte* data, out CULong length);
        return new ReadOnlySpan<byte>(data, (int)length.Value);
    }

    /// <summary>
    /// Return whether surface supports mime_type.
    /// </summary>
    /// <param name="mimeType">the mime type</param>
    /// <returns>
    /// <c>true</c> if surface supports mime_type, <c>false</c> otherwise
    /// </returns>
    public bool SupportsMimeType(string mimeType)
    {
        this.CheckDisposed();

        return cairo_surface_supports_mime_type(this.Handle, mimeType);
    }

    /// <summary>
    /// Returns an image surface that is the most efficient mechanism for modifying the backing store
    /// of the target surface. The region retrieved may be limited to the extents or <c>null</c>
    /// for the whole surface
    /// </summary>
    /// <param name="extents">limit the extraction to an rectangular region</param>
    /// <returns>
    /// a pointer to the newly allocated image surface. The caller must use <see cref="UnmapImage"/>
    /// to destroy this image surface.
    /// <para>
    /// This method always returns a valid pointer, but it will return a pointer to a "nil" surface if
    /// other is already in an error state or any other error occurs. If the returned pointer does not have
    /// an error status, it is guaranteed to be an image surface whose format is not <see cref="Format.Invalid"/>.
    /// </para>
    /// </returns>
    /// <remarks>
    /// Note, the use of the original surface as a target or source whilst it is mapped is undefined.
    /// The result of mapping the surface multiple times is undefined. Calling <see cref="CairoObject.Dispose()"/>
    /// or <see cref="Finish"/> on the resulting image surface results in undefined behavior.
    /// Changing the device transform of the image surface or of surface before the image surface is unmapped
    /// results in undefined behavior.
    /// </remarks>
    public ImageSurface MapToImage(RectangleInt extents)
    {
        this.CheckDisposed();

        cairo_surface_t* imageSurface = cairo_surface_map_to_image(this.Handle, &extents);
        return new ImageSurface(imageSurface, isOwnedByCairo: false, needsDestroy: false);
    }

    /// <summary>
    /// Returns an image surface that is the most efficient mechanism for modifying the backing store
    /// of the target surface. The region retrieved may be limited to the extents or <c>null</c>
    /// for the whole surface
    /// </summary>
    /// <remarks>
    /// <see cref="MapToImage(RectangleInt)"/> for futher information.
    /// </remarks>
    public ImageSurface MapToImage()
    {
        this.CheckDisposed();

        cairo_surface_t* imageSurface = cairo_surface_map_to_image(this.Handle, null);
        return new ImageSurface(imageSurface, isOwnedByCairo: false, needsDestroy: false);
    }

    /// <summary>
    /// Unmaps the image surface as returned from <see cref="MapToImage()"/>.
    /// </summary>
    /// <param name="image">the currently mapped image</param>
    /// <remarks>
    /// The content of the image will be uploaded to the target surface. Afterwards, the image is destroyed.
    /// <para>
    /// Using an image surface which wasn't returned by <see cref="MapToImage()"/> results in undefined behavior.
    /// </para>
    /// </remarks>
    public void UnmapImage(ImageSurface image)
    {
        this.CheckDisposed();

        cairo_surface_unmap_image(this.Handle, image.Handle);
    }

    /// <summary>
    /// Writes the contents of surface to a new file <paramref name="fileName"/> as a PNG image.
    /// </summary>
    /// <param name="fileName">the name of a file to write to; on Windows this filename is encoded in UTF-8.</param>
    /// <param name="throwOnError">
    /// when set to <c>true</c> (the default) throws a <see cref="CairoException"/> when the status
    /// does not indicate success.
    /// </param>
    /// <returns>
    /// <see cref="Status.Success"/> if the PNG file was written successfully. Otherwise,
    /// <see cref="Status.NoMemory"/> if memory could not be allocated for the operation or
    /// <see cref="Status.SurfaceTypeMismatch"/> if the surface does not have pixel contents, or
    /// <see cref="Status.WriteError"/> if an I/O error occurs while attempting to write the file, or
    /// <see cref="Status.PngError"/> if libpng returned an error.
    /// </returns>
    /// <exception cref="CairoException">
    /// when the operation fails and <paramref name="throwOnError"/> is set to <c>true</c>
    /// </exception>
    /// <exception cref="ArgumentNullException"><paramref name="fileName"/> is <c>null</c></exception>
    public Status WriteToPng(string fileName, bool throwOnError = true)
    {
        this.CheckDisposed();
        ArgumentNullException.ThrowIfNull(fileName);

        if (Ascii.IsValid(fileName))
        {
            Status status = cairo_surface_write_to_png(this.Handle, fileName);

            if (status == Status.WriteError)
            {
                // Try it again in case that cairo couldn't write to the stream, so
                // a better error message is thrown by .NET.
                // Side note: this is also why the cairo docs for wrappers suggest the
                // stream based function, but I'd like to have this approach here.
                goto Stream;
            }

            if (throwOnError)
            {
                status.ThrowIfNotSuccess();
            }

            return status;
        }

    Stream:
        using FileStream stream = File.Create(fileName);
        return this.WriteToPng(stream, throwOnError);
    }

    /// <summary>
    /// Writes the contents of surface to the given stream.
    /// </summary>
    /// <param name="stream">The stream to which the PNG content is written to</param>
    /// <param name="throwOnError">
    /// when set to <c>true</c> (the default) throws a <see cref="CairoException"/> when the status
    /// does not indicate success.
    /// </param>
    /// <returns>
    /// <see cref="Status.Success"/> if the PNG file was written successfully. Otherwise,
    /// <see cref="Status.NoMemory"/> is returned if memory could not be allocated for the operation,
    /// <see cref="Status.SurfaceTypeMismatch"/> if the surface does not have pixel contents, or
    /// <see cref="Status.PngError"/> if libpng returned an error.
    /// </returns>
    /// <exception cref="CairoException">
    /// when the operation fails and <paramref name="throwOnError"/> is set to <c>true</c>
    /// </exception>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <c>null</c></exception>
#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
    public Status WriteToPng(Stream stream, bool throwOnError = true)
    {
        this.CheckDisposed();
        ArgumentNullException.ThrowIfNull(stream);

        cairo_write_func_t writeFunc = &WriteFunc;
        Status status                = cairo_surface_write_to_png_stream(this.Handle, writeFunc, &stream);

        if (throwOnError)
        {
            status.ThrowIfNotSuccess();
        }

        return status;

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        static Status WriteFunc(void* state, byte* data, uint dataLength)
        {
            Stream stream           = *(Stream*)state;
            ReadOnlySpan<byte> span = new(data, (int)dataLength);

            stream.Write(span);

            return Status.Success;
        }
    }

    [Obsolete($"Use the {nameof(WriteToPng)} with the stream overload instead.")]
    public Status WriteToPngStream(Stream stream, bool throwOnError = true) => this.WriteToPng(stream, throwOnError);

    /// <summary>
    /// Writes the contents of surface to the given buffer writer.
    /// </summary>
    /// <param name="bufferWriter">The buffer writer to which the PNG content is written to</param>
    /// <param name="throwOnError">
    /// when set to <c>true</c> (the default) throws a <see cref="CairoException"/> when the status
    /// does not indicate success.
    /// </param>
    /// <returns>
    /// <see cref="Status.Success"/> if the PNG file was written successfully. Otherwise,
    /// <see cref="Status.NoMemory"/> is returned if memory could not be allocated for the operation,
    /// <see cref="Status.SurfaceTypeMismatch"/> if the surface does not have pixel contents, or
    /// <see cref="Status.PngError"/> if libpng returned an error.
    /// </returns>
    /// <exception cref="CairoException">
    /// when the operation fails and <paramref name="throwOnError"/> is set to <c>true</c>
    /// </exception>
    /// <exception cref="ArgumentNullException"><paramref name="bufferWriter"/> is <c>null</c></exception>
    public Status WriteToPng(IBufferWriter<byte> bufferWriter, bool throwOnError = true)
    {
        this.CheckDisposed();
        ArgumentNullException.ThrowIfNull(bufferWriter);

        cairo_write_func_t writeFunc = &WriteFunc;
        Status status                = cairo_surface_write_to_png_stream(this.Handle, writeFunc, &bufferWriter);

        if (throwOnError)
        {
            status.ThrowIfNotSuccess();
        }

        return status;

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        static Status WriteFunc(void* state, byte* data, uint dataLength)
        {
            IBufferWriter<byte> bufferWriter = *(IBufferWriter<byte>*)state;
            ReadOnlySpan<byte> span          = new(data, (int)dataLength);
            Span<byte> buffer                = bufferWriter.GetSpan(span.Length);

            span.CopyTo(buffer);
            bufferWriter.Advance(span.Length);

            return Status.Success;
        }
    }
#pragma warning restore CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type

    internal static Surface Lookup(cairo_surface_t* surface, bool isOwnedByCairo, bool needsDestroy = true)
    {
        SurfaceType surfaceType = cairo_surface_get_type(surface);

        return surfaceType switch
        {
            SurfaceType.Image => new ImageSurface     (surface, isOwnedByCairo, needsDestroy),
            SurfaceType.Xlib  => new XLibSurface      (surface, isOwnedByCairo, needsDestroy),
            SurfaceType.Xcb   => new XCBSurface       (surface, isOwnedByCairo, needsDestroy),
            SurfaceType.Win32 => new Win32Surface     (surface, isOwnedByCairo, needsDestroy),
            SurfaceType.Pdf   => new PdfSurface       (surface, isOwnedByCairo, needsDestroy),
            SurfaceType.PS    => new PostScriptSurface(surface, isOwnedByCairo, needsDestroy),
            SurfaceType.Svg   => new SvgSurface       (surface, isOwnedByCairo, needsDestroy),
            _                 => new Surface          (surface, isOwnedByCairo, needsDestroy)
        };
    }

    /// <summary>
    /// Indicates whether this surface is a "nil" surface or not.
    /// </summary>
    public bool IsNilSurface => this.Status != Status.Success;

    /// <summary>
    /// Indicates if the <see cref="Surface"/> can have multiple pages
    /// </summary>
    public virtual bool CanHaveMultiplePages => false;
}

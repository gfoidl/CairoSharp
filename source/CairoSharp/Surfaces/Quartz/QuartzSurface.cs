// (c) gfoidl, all rights reserved

using Cairo.Surfaces.Images;
using static Cairo.Surfaces.Quartz.QuartzSurfaceNative;

namespace Cairo.Surfaces.Quartz;

// https://www.cairographics.org/manual/cairo-Quartz-Surfaces.html

/// <summary>
/// Quartz Surfaces — Rendering to Quartz surfaces
/// </summary>
/// <remarks>
/// The Quartz surface is used to render cairo graphics targeting the Apple OS X Quartz rendering system.
/// </remarks>
public sealed unsafe class QuartzSurface : Surface
{
    /// <summary>
    /// Creates a Quartz surface backed by a CGBitmapContext using the main display's colorspace to
    /// avoid an expensive colorspace transform done serially on the CPU. This may produce slightly
    /// different colors from what's intended. Programs for which color management is important should
    /// create their own CGBitmapContext with a device-independent color space; most will expect
    /// Cairo to draw in sRGB and would use CGColorSpaceCreateWithName(kCGColorSpaceSRGB).
    /// </summary>
    /// <param name="format">format of pixels in the surface to create</param>
    /// <param name="width">width of the surface, in pixels</param>
    /// <param name="height">height of the surface, in pixels</param>
    /// <remarks>
    /// All Cairo operations, including those that require software rendering, will succeed on this surface.
    /// </remarks>
    /// <exception cref="CairoException">when construction fails</exception>
    public QuartzSurface(Format format, int width, int height) : base(cairo_quartz_surface_create(format, (uint)width, (uint)height)) { }

    /// <summary>
    /// Creates a Quartz surface that wraps the given CGContext. The CGContext is assumed to
    /// be in the standard Cairo coordinate space (that is, with the origin at the upper left
    /// and the Y axis increasing downward). If the CGContext is in the Quartz coordinate space
    /// (with the origin at the bottom left), then it should be flipped before this function
    /// is called.
    /// </summary>
    /// <param name="cgContext">the existing CGContext for which to create the surface</param>
    /// <param name="width">surface</param>
    /// <param name="height">height of the surface, in pixels</param>
    /// <remarks>
    /// All Cairo operations are implemented in terms of Quartz operations, as long as
    /// Quartz-compatible elements are used (such as Quartz fonts).
    /// </remarks>
    /// <exception cref="CairoException">when construction fails</exception>
    public QuartzSurface(IntPtr cgContext, int width, int height)
        : base(cairo_quartz_surface_create_for_cg_context(cgContext.ToPointer(), (uint)width, (uint)height)) { }

    /// <summary>
    /// Returns the CGContextRef that the given Quartz surface is backed by.
    /// </summary>
    /// <remarks>
    /// A call to <see cref="Surface.Flush"/> is required before using the CGContextRef to
    /// ensure that all pending drawing operations are finished and to restore any temporary
    /// modification cairo has made to its state. A call to <see cref="Surface.MarkDirty()"/> is
    /// required after the state or the content of the CGContextRef has been modified.
    /// </remarks>
    public IntPtr CgContext
    {
        get
        {
            this.CheckDisposed();

            void* cgContextRef = cairo_quartz_surface_get_cg_context(this.Handle);
            return new IntPtr(cgContextRef);
        }
    }

    /// <summary>
    /// Creates a Quartz surface backed by a CGBitmapContext that references the given image surface.
    /// The resulting surface can be rendered quickly when used as a source when rendering to
    /// a cairo_quartz_surface.
    /// </summary>
    /// <param name="imageSurface">a cairo image surface to wrap with a quartz image surface</param>
    /// <exception cref="CairoException">when construction fails</exception>
    public QuartzSurface(ImageSurface imageSurface) : base(cairo_quartz_image_surface_create(imageSurface.Handle)) { }

    /// <summary>
    /// Returns a <see cref="ImageSurface"/> that refers to the same bits as the image of the quartz surface.
    /// </summary>
    /// <returns>
    /// a <see cref="ImageSurface"/> (owned by the <see cref="QuartzSurface"/>), or <c>null</c> if
    /// the quartz surface is not an image surface.
    /// </returns>
    public ImageSurface? GetImage()
    {
        this.CheckDisposed();

        void* handle = cairo_quartz_image_surface_get_image(this.Handle);

        if (handle is null)
        {
            return null;
        }

        return new ImageSurface(handle, isOwnedByCairo: true, needsDestroy: /* not documented in cairo */ false);
    }
}

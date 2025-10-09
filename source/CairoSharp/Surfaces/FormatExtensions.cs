// (c) gfoidl, all rights reserved

using static Cairo.Surfaces.Images.ImageSurfaceNative;

namespace Cairo.Surfaces;

public static unsafe class FormatExtensions
{
    extension(Format format)
    {
        /// <summary>
        /// This method provides a stride value that will respect all alignment requirements
        /// of the accelerated image-rendering code within cairo.
        /// </summary>
        /// <param name="width">The desired width of an image surface to be created.</param>
        /// <returns>
        /// the appropriate stride to use given the desired format and width, or -1 if either
        /// the format is invalid or the width too large.
        /// </returns>
        /// <remarks>
        /// See <a href="https://www.cairographics.org/manual/cairo-Image-Surfaces.html#cairo-format-stride-for-width">cairo docs</a>
        /// for typical usage.
        /// </remarks>
        public int GetStrideForWidth(int width) => cairo_format_stride_for_width(format, width);
    }
}

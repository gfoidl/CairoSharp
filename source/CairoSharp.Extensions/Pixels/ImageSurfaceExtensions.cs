// (c) gfoidl, all rights reserved

using Cairo.Surfaces.Images;

namespace Cairo.Extensions.Pixels;

/// <summary>
/// Extension methods for <see cref="ImageSurface"/>.
/// </summary>
public static class ImageSurfaceExtensions
{
    extension(ImageSurface imageSurface)
    {
        /// <summary>
        /// Returns a <see cref="PixelAccessor"/> for the <see cref="ImageSurface"/>.
        /// </summary>
        /// <returns>
        /// An accessor for the pixel data in <see cref="ImageSurface"/>, for direct
        /// reading / writing of pixel data in the format of the surface (see <see cref="ImageSurface.Format"/>).
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// When the <see cref="ImageSurface.Format"/> != <see cref="Surfaces.Format.Argb32"/>.
        /// </exception>
        public PixelAccessor GetPixelAccessor() => new PixelAccessor(imageSurface);

        /// <summary>
        /// Returns a <see cref="PixelRowAccessor"/> for the <see cref="ImageSurface"/>.
        /// </summary>
        /// <param name="y">the row</param>
        /// <returns>
        /// A <see cref="PixelRowAccessor"/> for direct manipulation of the pixels in the row.
        /// </returns>
        /// <remarks>
        /// You must call <see cref="Surfaces.Surface.Flush"/> before a call to this method, and
        /// you must call <see cref="Surfaces.Surface.MarkDirty()"/> when done with the pixel
        /// operations.
        /// </remarks>
        /// <exception cref="NotSupportedException">
        /// When the <see cref="ImageSurface.Format"/> != <see cref="Surfaces.Format.Argb32"/>.
        /// </exception>
        public PixelRowAccessor GetPixelRowAccessor(int y)
        {
            PixelAccessor pixelAccessor = new(imageSurface, setSurfaceState: false);
            return pixelAccessor.GetRowAccessor(y);
        }
    }
}

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
    }
}

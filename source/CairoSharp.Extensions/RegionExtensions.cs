// (c) gfoidl, all rights reserved

using Cairo.Drawing.Regions;

namespace Cairo.Extensions;

/// <summary>
/// Extensions for <see cref="Region"/>.
/// </summary>
public static class RegionExtensions
{
    extension(CairoContext cr)
    {
        /// <summary>
        /// Adds the given <see cref="Region"/> to the current path of <see cref="CairoContext"/>.
        /// </summary>
        /// <param name="region">a cairo region</param>
        /// <exception cref="ArgumentNullException">when <paramref name="region"/> is <c>null</c></exception>
        public void AppendRegionToPath(Region region)
        {
            ArgumentNullException.ThrowIfNull(region);

            int boxesCount = region.Rectangles;

            for (int i = 0; i < boxesCount; ++i)
            {
                RectangleInt rectangle = region[i];
                cr.Rectangle(rectangle);
            }
        }
    }
}

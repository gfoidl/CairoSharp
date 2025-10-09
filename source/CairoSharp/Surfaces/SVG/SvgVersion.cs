// (c) gfoidl, all rights reserved

using static Cairo.Surfaces.SVG.SvgSurfaceNative;

namespace Cairo.Surfaces.SVG;

/// <summary>
/// <see cref="SvgVersion"/> is used to describe the version number of the SVG
/// specification that a generated SVG file will conform to.
/// </summary>
public enum SvgVersion
{
    /// <summary>
    /// The version 1.1 of the SVG specification. (Since 1.2)
    /// </summary>
    Version1_1,

    /// <summary>
    /// The version 1.2 of the SVG specification. (Since 1.2)
    /// </summary>
    Version1_2
}

public static unsafe class SvgVersionExtensions
{
    extension(SvgVersion version)
    {
        /// <summary>
        /// Get the string representation of the given version id. This method will return <c>null</c>
        /// if version isn't valid. See <see cref="GetSupportedVersions"/> for a way to get the list
        /// of valid version ids.
        /// </summary>
        /// <returns>the string associated to given version.</returns>
        public string? GetString()
        {
            sbyte* tmp = cairo_svg_version_to_string(version);
            return tmp is not null ? new string(tmp) : null;
        }
    }

    extension(SvgSurface)
    {
        /// <summary>
        /// Used to retrieve the list of supported versions. See <see cref="SvgSurface.RestrictToVersion(SvgVersion)"/>.
        /// </summary>
        /// <returns>supported version list</returns>
        public static ReadOnlySpan<SvgVersion> GetSupportedVersions()
        {
            cairo_svg_get_versions(out SvgVersion* versions, out int length);
            return new ReadOnlySpan<SvgVersion>(versions, length);
        }
    }
}

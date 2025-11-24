// (c) gfoidl, all rights reserved

using Cairo.Utilities;

namespace Cairo;

/// <summary>
/// Information about the <see cref="CairoAPI"/>
/// </summary>
/// <remarks>
/// See https://www.cairographics.org/manual/cairo-Version-Information.html for further infos.
/// </remarks>
public static class CairoAPI
{
    /// <summary>
    /// Returns the version of the cairo library encoded in a single integer as per <see cref="VersionEncode(int, int, int)"/>.
    /// The encoding ensures that later versions compare greater than earlier versions.
    /// </summary>
    public static int Version { get; } = UtilitiesNative.cairo_version();

    /// <summary>
    /// Returns the version of the cairo library as a human-readable string of the form "X.Y.Z".
    /// </summary>
    public static unsafe string VersionString => field ??= UtilitiesNative.cairo_version_string()!;

    /// <summary>
    /// CAIRO_VERSION_ENCODE
    /// </summary>
    public static int VersionEncode(int major, int minor, int patch) => major * 10_000 + minor * 100 + patch;

    /// <summary>
    /// Verifies at runtime that the cairo library meets the required version. When <c>true</c> this method
    /// is nop. Otherwise will throw a <see cref="NotSupportedException"/>.
    /// </summary>
    public static void CheckSupportedVersion(int majorRequired, int minorRequired, int patchRequired)
    {
        // Note: this could be done via conditional compilation constants too, but that would erase
        // APIs that aren't supported at compile time.
        // With this approach users could update the cairo library manually, and use the new APIs.

        int requiredVersion = VersionEncode(majorRequired, minorRequired, patchRequired);

        if (Version < requiredVersion)
        {
            throw new NotSupportedException($"""
                The feature is only available from Cairo version {majorRequired}.{minorRequired}.{patchRequired} onwards.
                Current used version of Cairo: {VersionString}
                """);
        }
    }
}

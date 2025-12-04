// (c) gfoidl, all rights reserved

namespace Cairo.Extensions.Pango;

public static class Pango
{
    /// <summary>
    /// The scale between dimensions used for Pango distances and device units.
    /// </summary>
    /// <remarks>
    /// The definition of device units is dependent on the output device; it will typically
    /// be pixels for a screen, and points for a printer. <see cref="Scale"/> is currently
    /// 1024, but this may be changed in the future.
    /// <para>
    /// When setting font sizes, device units are always considered to be points (as in "12 point
    /// font"), rather than pixels.
    /// </para>
    /// </remarks>
    public const int Scale = 1024;

    /// <summary>
    /// Default resolution, 96.
    /// </summary>
    /// <remarks>
    /// See <see cref="PangoLayout.Resolution"/> for description.
    /// </remarks>
    public const double DefaultResolution = 96d;
}

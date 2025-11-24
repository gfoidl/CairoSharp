// (c) gfoidl, all rights reserved

namespace Cairo.Fonts;

/// <summary>
/// The subpixel order specifies the order of color elements within each pixel on the
/// display device when rendering with an antialiasing mode of <see cref="Antialias.Subpixel"/>.
/// </summary>
public enum SubpixelOrder
{
    /// <summary>
    /// Use the default subpixel order for the target device, since 1.0
    /// </summary>
    Default,

    /// <summary>
    /// Subpixel elements are arranged horizontally with red at the left, since 1.0
    /// </summary>
    Rgb,

    /// <summary>
    /// Subpixel elements are arranged horizontally with blue at the left, since 1.0
    /// </summary>
    Bgr,

    /// <summary>
    /// Subpixel elements are arranged vertically with red at the top, since 1.0
    /// </summary>
    Vrgb,

    /// <summary>
    /// Subpixel elements are arranged vertically with blue at the top, since 1.0
    /// </summary>
    Vbgr
}

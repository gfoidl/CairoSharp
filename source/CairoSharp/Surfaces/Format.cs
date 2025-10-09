// (c) gfoidl, all rights reserved

namespace Cairo.Surfaces;

/// <summary>
/// <see cref="Format"/> is used to identify the memory format of image data.
/// </summary>
/// <remarks>
/// New entries may be added in future versions.
/// </remarks>
public enum Format
{
    /// <summary>
    /// no such format exists or is supported.
    /// </summary>
    Invalid = -1,

    /// <summary>
    /// each pixel is a 32-bit quantity, with alpha in the upper 8 bits, then red, then green,
    /// then blue. The 32-bit quantities are stored native-endian. Pre-multiplied alpha is used.
    /// (That is, 50% transparent red is 0x80800000, not 0x80ff0000.) (Since 1.0)
    /// </summary>
    Argb32 = 0,

    /// <summary>
    /// each pixel is a 32-bit quantity, with the upper 8 bits unused. Red, Green, and Blue are
    /// stored in the remaining 24 bits in that order. (Since 1.0)
    /// </summary>
    Rgb24 = 1,

    /// <summary>
    /// each pixel is a 8-bit quantity holding an alpha value. (Since 1.0)
    /// </summary>
    A8 = 2,

    /// <summary>
    /// each pixel is a 1-bit quantity holding an alpha value. Pixels are packed together
    /// into 32-bit quantities. The ordering of the bits matches the endianness of the platform.
    /// On a big-endian machine, the first pixel is in the uppermost bit, on a little-endian machine
    /// the first pixel is in the least-significant bit. (Since 1.0)
    /// </summary>
    A1 = 3,

    /// <summary>
    /// each pixel is a 16-bit quantity with red in the upper 5 bits, then green in the middle
    /// 6 bits, and blue in the lower 5 bits. (Since 1.2)
    /// </summary>
    Rgb16565 = 4,

    /// <summary>
    /// like RGB24 but with 10bpc. (Since 1.12)
    /// </summary>
    Rgb30 = 5,

    /// <summary>
    /// 3 floats, R, G, B. (Since 1.17.2)
    /// </summary>
    Rgb96F = 6,

    /// <summary>
    /// 4 floats, R, G, B, A. (Since 1.17.2)
    /// </summary>
    Rgba1218f = 7
}

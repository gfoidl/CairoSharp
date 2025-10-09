// (c) gfoidl, all rights reserved

namespace Cairo.Surfaces;

/// <summary>
/// <see cref="Content"/> is used to describe the content that a surface will contain, whether
/// color information, alpha information (translucence vs. opacity), or both.
/// </summary>
/// <remarks>
/// Note: The large values here are designed to keep <see cref="Content"/> values distinct from
/// <see cref="Format"/> values so that the implementation can detect the error if users
/// confuse the two types.
/// </remarks>
public enum Content
{
    /// <summary>
    /// The surface will hold color content only. (Since 1.0)
    /// </summary>
    Color = 0x1000,

    /// <summary>
    /// The surface will hold alpha content only. (Since 1.0)
    /// </summary>
    Alpha = 0x2000,

    /// <summary>
    /// The surface will hold color and alpha content. (Since 1.0)
    /// </summary>
    ColorAlpha = 0x3000
}

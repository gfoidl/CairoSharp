// (c) gfoidl, all rights reserved

namespace Cairo.Fonts;

/// <summary>
/// Specifies the type of hinting to do on font outlines. Hinting is the process of
/// fitting outlines to the pixel grid in order to improve the appearance of the result.
/// Since hinting outlines involves distorting them, it also reduces the faithfulness to
/// the original outline shapes. Not all of the outline hinting styles are supported by
/// all font backends.
/// </summary>
/// <remarks>
/// New entries may be added in future versions.
/// </remarks>
public enum HintStyle
{
    /// <summary>
    /// Use the default hint style for font backend and target device, since 1.0
    /// </summary>
    Default,

    /// <summary>
    /// Do not hint outlines, since 1.0
    /// </summary>
    None,

    /// <summary>
    /// Hint outlines slightly to improve contrast while retaining good fidelity to the original shapes, since 1.0
    /// </summary>
    Slight,

    /// <summary>
    /// Hint outlines with medium strength giving a compromise between fidelity to the original shapes and contrast, since 1.0
    /// </summary>
    Medium,

    /// <summary>
    /// Hint outlines to maximize contrast, since 1.0
    /// </summary>
    Full
}

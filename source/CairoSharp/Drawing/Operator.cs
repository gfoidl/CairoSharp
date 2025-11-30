// (c) gfoidl, all rights reserved

namespace Cairo.Drawing;

/// <summary>
/// <see cref="Operator"/> is used to set the compositing operator for all cairo drawing operations.
/// </summary>
/// <remarks>
/// The default operator is <see cref="Over"/>.
/// <para>
/// The operators marked as unbounded modify their destination even outside of the mask layer
/// (that is, their effect is not bound by the mask layer). However, their effect can still be
/// limited by way of clipping.
/// </para>
/// <para>
/// To keep things simple, the operator descriptions here document the behavior for when both
/// source and destination are either fully transparent or fully opaque. The actual implementation
/// works for translucent layers too. For a more detailed explanation of the effects of each operator,
/// including the mathematical definitions, see
/// <a href="https://cairographics.org/operators/">cairo's compositing operators</a>.
/// </para>
/// </remarks>
public enum Operator
{
    /// <summary>
    /// clear destination layer (bounded) (Since 1.0)
    /// </summary>
    Clear,

    /// <summary>
    /// replace destination layer (bounded) (Since 1.0)
    /// </summary>
    Source,

    /// <summary>
    /// draw source layer on top of destination layer (bounded) (Since 1.0)
    /// </summary>
    Over,

    /// <summary>
    /// draw source where there was destination content (unbounded) (Since 1.0)
    /// </summary>
    In,

    /// <summary>
    /// draw source where there was no destination content (unbounded) (Since 1.0)
    /// </summary>
    Out,

    /// <summary>
    /// draw source on top of destination content and only there (Since 1.0)
    /// </summary>
    Atop,

    /// <summary>
    /// ignore the source (Since 1.0)
    /// </summary>
    Dest,

    /// <summary>
    /// draw destination on top of source (Since 1.0)
    /// </summary>
    DestOver,

    /// <summary>
    /// leave destination only where there was source content (unbounded) (Since 1.0)
    /// </summary>
    DestIn,

    /// <summary>
    /// leave destination only where there was no source content (Since 1.0)
    /// </summary>
    DestOut,

    /// <summary>
    /// leave destination on top of source content and only there (unbounded) (Since 1.0)
    /// </summary>
    DestAtop,

    /// <summary>
    /// source and destination are shown where there is only one of them (Since 1.0)
    /// </summary>
    Xor,

    /// <summary>
    /// source and destination layers are accumulated (Since 1.0)
    /// </summary>
    Add,

    /// <summary>
    /// like over, but assuming source and dest are disjoint geometries (Since 1.0)
    /// </summary>
    Saturate,

    /// <summary>
    /// source and destination layers are multiplied. This causes the result to be at least
    /// as dark as the darker inputs. (Since 1.10)
    /// </summary>
    Multiply,

    /// <summary>
    /// source and destination are complemented and multiplied. This causes the result to be
    /// at least as light as the lighter inputs. (Since 1.10)
    /// </summary>
    Screen,

    /// <summary>
    /// multiplies or screens, depending on the lightness of the destination color. (Since 1.10)
    /// </summary>
    Overlay,

    /// <summary>
    /// replaces the destination with the source if it is darker, otherwise keeps the source. (Since 1.10)
    /// </summary>
    Darken,

    /// <summary>
    /// replaces the destination with the source if it is lighter, otherwise keeps the source. (Since 1.10)
    /// </summary>
    Lighten,

    /// <summary>
    /// brightens the destination color to reflect the source color. (Since 1.10)
    /// </summary>
    ColorDodge,

    /// <summary>
    /// darkens the destination color to reflect the source color. (Since 1.10)
    /// </summary>
    ColorBurn,

    /// <summary>
    /// Multiplies or screens, dependent on source color. (Since 1.10)
    /// </summary>
    HardLight,

    /// <summary>
    /// Darkens or lightens, dependent on source color. (Since 1.10)
    /// </summary>
    SoftLight,

    /// <summary>
    /// Takes the difference of the source and destination color. (Since 1.10)
    /// </summary>
    Difference,

    /// <summary>
    /// Produces an effect similar to difference, but with lower contrast. (Since 1.10)
    /// </summary>
    Exclusion,

    /// <summary>
    /// Creates a color with the hue of the source and the saturation and luminosity
    /// of the target. (Since 1.10)
    /// </summary>
    HslHue,

    /// <summary>
    /// Creates a color with the saturation of the source and the hue and luminosity
    /// of the target. Painting with this mode onto a gray area produces no change. (Since 1.10)
    /// </summary>
    HslSaturation,

    /// <summary>
    /// Creates a color with the hue and saturation of the source and the luminosity of the target.
    /// This preserves the gray levels of the target and is useful for coloring monochrome images
    /// or tinting color images. (Since 1.10)
    /// </summary>
    HslColor,

    /// <summary>
    /// Creates a color with the luminosity of the source and the hue and saturation of the target.
    /// This produces an inverse effect to CAIRO_OPERATOR_HSL_COLOR. (Since 1.10)
    /// </summary>
    HslLuminosity
}

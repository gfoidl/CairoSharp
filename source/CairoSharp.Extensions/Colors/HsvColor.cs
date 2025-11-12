// (c) gfoidl, all rights reserved

using System.Diagnostics;

namespace Cairo.Extensions.Colors;

/// <summary>
/// A color given in the <a href="https://en.wikipedia.org/wiki/HSL_and_HSV">HSV</a> color space.
/// </summary>
/// <param name="Hue">hue component of color in [0,360]</param>
/// <param name="Saturation">saturation component of color in [0,1]</param>
/// <param name="Value">value component of color in [0,1]</param>
/// <remarks>
/// By variation of the <see cref="Hue"/> component from 0 to 360 the resulting color changes from
/// red to yellow, green, cyan, blue, and magenta back to red.
/// <para>
/// With <see cref="Saturation"/> 0 the color is not saturated, i.e. it is a gray scale. With
/// <see cref="Saturation"/> 1 the color is fully saturated, that is that there is no white component
/// present.
/// </para>
/// <para>
/// <see cref="Value"/> is the lightness.
/// </para>
/// </remarks>
[DebuggerNonUserCode]
public readonly record struct HsvColor(double Hue, double Saturation, double Value)
{
    /// <summary>
    /// Validates that the color components are in bounds.
    /// </summary>
    public void Validate()
    {
        if (this.Hue        < 0 || this.Hue        > 360) ThrowHue();
        if (this.Saturation < 0 || this.Saturation >   1) ThrowSaturation();
        if (this.Value      < 0 || this.Value      >   1) ThrowValue();

        static void ThrowHue()        => throw new ArgumentException("Hue outside of [0,360].");
        static void ThrowSaturation() => throw new ArgumentException("Saturation outside of [0,1]");
        static void ThrowValue()      => throw new ArgumentException("Value outside of [0,1]");
    }

    /// <summary>
    /// Returns the RGB (<see cref="Color"/>) for this color.
    /// </summary>
    /// <returns>
    /// RGB (<see cref="Color"/>) for this color.
    /// </returns>
    /// <exception cref="ArgumentException">a color component is outside the valid bounds</exception>
    /// <seealso cref="ColorExtensions.ToHSV(Color)"/>
    public Color ToRGB()
    {
        this.Validate();

        // When saturation is 0 -> gray scale
        if (this.Saturation == 0)
        {
            double value = this.Value;
            return new Color(value, value, value);
        }

        // The color wheel has 6 sectors.
        double sectorPos = this.Hue / 60d;
        int sectorNumber = (int)Math.Floor(sectorPos);
        double fractionalSector = sectorPos - sectorNumber;

        // Value of 3-axes of the color
        double p = this.Value * (1 - this.Saturation);
        double q = this.Value * (1 - this.Saturation * fractionalSector);
        double t = this.Value * (1 - this.Saturation * (1 - fractionalSector));

        // Differentiate by sector
        return sectorNumber switch
        {
            0 => new Color(this.Value, t, p),
            1 => new Color(q, this.Value, p),
            2 => new Color(p, this.Value, t),
            3 => new Color(p, q, this.Value),
            4 => new Color(t, p, this.Value),
            _ => new Color(this.Value, p, q)
        };
    }
}

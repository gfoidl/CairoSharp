// (c) gfoidl, all rights reserved

namespace Cairo.Extensions.Arrows;

/// <summary>
/// An arrow or vector, where the arrow head is not filled (only the outline).
/// </summary>
public sealed class OpenArrow : Arrow
{
    /// <summary>
    /// Creates a <see cref="OpenArrow"/>.
    /// </summary>
    /// <param name="cr">the <see cref="CairoContext"/></param>
    /// <param name="length">length of the arrow head</param>
    /// <param name="angleInDegrees">half arrow head opening angle in degrees</param>
    /// <remarks>
    /// The absolut values are used, no exception will be thrown if either <paramref name="length"/>
    /// or <paramref name="angleInDegrees"/> is negative.
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="cr"/> is <c>null</c></exception>
    public OpenArrow(CairoContext cr, double length = 0.05, double angleInDegrees = 10) : base(cr, length, angleInDegrees) { }

    /// <inheritdoc/>
    protected override void DrawArrowHead(CairoContext cr)
    {
        cr.MoveTo(_arrowPoints[0]);
        cr.LineTo(_arrowPoints[1]);

        cr.MoveTo(_arrowPoints[0]);
        cr.LineTo(_arrowPoints[2]);

        // Linie die sonst nicht dargestellt wird ergänzen
        cr.MoveTo(_arrowPoints[0]);
        cr.LineTo(-_length, 0);

        cr.Stroke();
    }
}

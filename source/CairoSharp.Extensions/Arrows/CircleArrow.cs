// (c) gfoidl, all rights reserved

namespace Cairo.Extensions.Arrows;

/// <summary>
/// An arrow with a circle head.
/// </summary>
public sealed class CircleArrow : Arrow
{
    /// <summary>
    /// Creates a <see cref="CircleArrow"/>.
    /// </summary>
    /// <param name="cr">the <see cref="CairoContext"/></param>
    /// <param name="radius">the radius of the arrow head's circle</param>
    /// <remarks>
    /// The absolut value is used, no exception is thrown on negative values.
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="cr"/> is <c>null</c></exception>
    public CircleArrow(CairoContext cr, double radius = 0.5) : base(cr, radius, Math.Tau) { }

    /// <inheritdoc/>
    protected override void DrawArrowHead(CairoContext cr)
    {
        cr.Arc(0, 0, _length, 0, Math.Tau);
        cr.Fill();
    }
}

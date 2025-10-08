// (c) gfoidl, all rights reserved

namespace Cairo.Surfaces;

/// <summary>
/// Base class for page size, e.g. A4
/// </summary>
public abstract class PageSize
{
    private const double MillimetersPerInch = 25.4;
    private const double InchPerMillimeter  = 1d / MillimetersPerInch;
    private const double PointsPerInch      = 72;

    public abstract double WidthInMillimeters  { get; }
    public abstract double HeightInMillimeters { get; }

    public double WidthInPoints  => this.WidthInMillimeters * InchPerMillimeter * PointsPerInch;
    public double HeightInPoints => this.HeightInMillimeters * InchPerMillimeter * PointsPerInch;

    /// <summary>
    /// A4 page size
    /// </summary>
    public static PageSize A4 { get; } = new A4PageSize();

    /// <summary>
    /// A4 landscape page size
    /// </summary>
    public static PageSize A4Landscape { get; } = new A4LandscapePageSize();
}


internal sealed class A4PageSize : PageSize
{
    public override double WidthInMillimeters  => 210;
    public override double HeightInMillimeters => 297;
}

internal sealed class A4LandscapePageSize : PageSize
{
    public override double WidthInMillimeters  => 297;
    public override double HeightInMillimeters => 210;
}

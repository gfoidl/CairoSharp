// (c) gfoidl, all rights reserved

namespace Cairo.Extensions.Shapes;

/// <summary>
/// A base implementation of a <see cref="Shape"/> that is centered at (0, 0).
/// </summary>
/// <remarks>
/// The <see cref="Drawing.Path.Path"/> of the <see cref="Shape"/> is stored internally,
/// and can be re-used by drawing operations. So when done using the <see cref="Shape"/>
/// finish it by calling <see cref="Dispose"/>.
/// <para>
/// One-off usage of <see cref="Shape"/> may be too heavy, but for tesselation tasks it may
/// be ideal, as all the computation and layout of the <see cref="Shape"/> needs only be done
/// once, then can be re-used.
/// </para>
/// </remarks>
public abstract class Shape(CairoContext cr) : IDisposable
{
    private readonly CairoContext _cr = cr;
    private Path?                 _path;

    public void Dispose()
    {
        _path?.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// The <see cref="Drawing.Path.Path"/> for this <see cref="Shape"/>.
    /// </summary>
    protected internal Path Path => _path ??= this.GetPath();

    /// <summary>
    /// Draws the oultine of the shape.
    /// </summary>
    /// <param name="cr">The <see cref="CairoContext"/></param>
    /// <param name="centerX">The x-coordinate of the center</param>
    /// <param name="centerY">The y-coordinate of the center</param>
    public void Draw(CairoContext cr, double centerX, double centerY)
    {
        ArgumentNullException.ThrowIfNull(cr);

        using (cr.Save())
        {
            cr.Translate(centerX, centerY);
            cr.AppendPath(this.Path);
            cr.Stroke();
        }
    }

    /// <summary>
    /// Draws the oultine of the shape with the given <see cref="Color" />.
    /// </summary>
    /// <param name="cr">The <see cref="CairoContext"/></param>
    /// <param name="centerX">The x-coordinate of the center</param>
    /// <param name="centerY">The y-coordinate of the center</param>
    /// <param name="color">
    /// The colour of the outline. Components must be given in the range [0,1]
    /// </param>
    public void Draw(CairoContext cr, double centerX, double centerY, Color color)
    {
        cr.Color = color;
        this.Draw(cr, centerX, centerY);
    }

    /// <summary>
    /// Fills the shape.
    /// </summary>
    /// <param name="cr">The <see cref="CairoContext"/></param>
    /// <param name="centerX">The x-coordinate of the center</param>
    /// <param name="centerY">The y-coordinate of the center</param>
    public void Fill(CairoContext cr, double centerX, double centerY)
    {
        ArgumentNullException.ThrowIfNull(cr);

        using (cr.Save())
        {
            cr.Translate(centerX, centerY);
            cr.AppendPath(this.Path);
            cr.Fill();
        }
    }

    /// <summary>
    /// Fills the shape with the given <see cref="Color" />.
    /// </summary>
    /// <param name="cr">The <see cref="CairoContext"/></param>
    /// <param name="centerX">The x-coordinate of the center</param>
    /// <param name="centerY">The y-coordinate of the center</param>
    /// <param name="color">
    /// The colour of the fill. Components must be given in the range [0,1]
    /// </param>
    public void Fill(CairoContext cr, double centerX, double centerY, Color color)
    {
        cr.Color = color;
        this.Fill(cr, centerX, centerY);
    }

    private Path GetPath()
    {
        _cr.NewPath();

        this.CreatePath(_cr);
        Path path = _cr.CopyPath();

        _cr.NewPath();

        return path;
    }

    /// <summary>
    /// Creates the <see cref="Drawing.Path.Path"/> for the shape.
    /// </summary>
    /// <param name="cr">The <see cref="CairoContext"/></param>
    /// <remarks>
    /// When this method is called, <see cref="CairoContext"/> has no current point.
    /// After this method the current point will be cleared too.
    /// </remarks>
    protected internal abstract void CreatePath(CairoContext cr);
}

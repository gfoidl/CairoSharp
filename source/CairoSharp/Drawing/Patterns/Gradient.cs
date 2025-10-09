// (c) gfoidl, all rights reserved

using static Cairo.Drawing.Patterns.PatternNative;

namespace Cairo.Drawing.Patterns;

/// <summary>
/// <see cref="Gradient"/> is an abstract base class from which other <see cref="Pattern"/> classes derive.
/// </summary>
public abstract unsafe class Gradient(void* handle, bool owner) : Pattern(handle, owner)
{
    /// <summary>
    /// Adds an opaque color stop to a gradient pattern. The offset specifies the location along
    /// the gradient's control vector. For example, a linear gradient's control vector is from
    /// (x0,y0) to (x1,y1) while a radial gradient's control vector is from any point on the start
    /// circle to the corresponding point on the end circle.
    /// </summary>
    /// <param name="offset">an offset in the range [0.0 .. 1.0]</param>
    /// <param name="red">red component of color</param>
    /// <param name="green">green component of color</param>
    /// <param name="blue">blue component of color</param>
    /// <remarks>
    /// The color is specified in the same way as in <see cref="CairoContext.SetSourceRgb(double, double, double)"/>.
    /// <para>
    /// If two (or more) stops are specified with identical offset values, they will be sorted according to the order
    /// in which the stops are added, (stops added earlier will compare less than stops added later). This can
    /// be useful for reliably making sharp color transitions instead of the typical blend.
    /// </para>
    /// <para>
    /// Note: If the pattern is not a gradient pattern, (eg. a linear or radial pattern), then the pattern will
    /// be put into an error status with a status of <see cref="Status.PatternTypeMismatch"/>.
    /// </para>
    /// </remarks>
    public void AddColorStopRgb(double offset, double red, double green, double blue)
    {
        this.CheckDisposed();
        cairo_pattern_add_color_stop_rgb(this.Handle, offset, red, green, blue);
    }

    /// <summary>
    /// Adds a translucent color stop to a gradient pattern. The offset specifies the location along the
    /// gradient's control vector. For example, a linear gradient's control vector is from (x0,y0) to
    /// (x1,y1) while a radial gradient's control vector is from any point on the start circle to the
    /// corresponding point on the end circle.
    /// </summary>
    /// <param name="offset">an offset in the range [0.0 .. 1.0]</param>
    /// <param name="red">red component of color</param>
    /// <param name="green">green component of color</param>
    /// <param name="blue">blue component of color</param>
    /// <param name="alpha">alpha component of color</param>
    /// <remarks>
    /// The color is specified in the same way as in <see cref="CairoContext.SetSourceRgba(double, double, double, double)"/>.
    /// <para>
    /// If two (or more) stops are specified with identical offset values, they will be sorted according to
    /// the order in which the stops are added, (stops added earlier will compare less than stops added later).
    /// This can be useful for reliably making sharp color transitions instead of the typical blend.
    /// </para>
    /// <para>
    /// Note: If the pattern is not a gradient pattern, (eg. a linear or radial pattern), then the pattern will
    /// be put into an error status with a status of <see cref="Status.PatternTypeMismatch"/>.
    /// </para>
    /// </remarks>
    public void AddColorStopRgba(double offset, double red, double green, double blue, double alpha)
    {
        this.CheckDisposed();
        cairo_pattern_add_color_stop_rgba(this.Handle, offset, red, green, blue, alpha);
    }

    /// <summary>
    /// Adds a translucent color stop to a gradient pattern. The offset specifies the location along the
    /// gradient's control vector. For example, a linear gradient's control vector is from (x0,y0) to
    /// (x1,y1) while a radial gradient's control vector is from any point on the start circle to the
    /// corresponding point on the end circle.
    /// </summary>
    /// <param name="offset">an offset in the range [0.0 .. 1.0]</param>
    /// <param name="color">color</param>
    /// <remarks>
    /// The color is specified in the same way as in <see cref="CairoContext.SetSourceColor(Color)"/>.
    /// <para>
    /// If two (or more) stops are specified with identical offset values, they will be sorted according to
    /// the order in which the stops are added, (stops added earlier will compare less than stops added later).
    /// This can be useful for reliably making sharp color transitions instead of the typical blend.
    /// </para>
    /// <para>
    /// Note: If the pattern is not a gradient pattern, (eg. a linear or radial pattern), then the pattern will
    /// be put into an error status with a status of <see cref="Status.PatternTypeMismatch"/>.
    /// </para>
    /// </remarks>
    public void AddColorStop(double offset, Color color)
        => this.AddColorStopRgba(offset, color.Red, color.Green, color.Blue, color.Alpha);

    /// <summary>
    /// Gets the number of color stops specified in the given gradient pattern.
    /// </summary>
    public int ColorStopCount
    {
        get
        {
            this.CheckDisposed();

            Status status = cairo_pattern_get_color_stop_count(this.Handle, out int count);
            status.ThrowIfStatus(Status.PatternTypeMismatch);

            return count;
        }
    }

    /// <summary>
    /// Gets the color and offset information at the given index for a gradient pattern. Values of index
    /// range from 0 to n-1 where n is the number returned by <see cref="ColorStopCount"/>.
    /// </summary>
    /// <param name="index">index of the stop to return data for</param>
    /// <param name="offset">return value for the offset of the stop</param>
    /// <param name="red">return value for red component of color</param>
    /// <param name="green">return value for green component of color</param>
    /// <param name="blue">return value for blue component of color</param>
    /// <param name="alpha">return value for alpha component of color</param>
    /// <returns>
    /// <c>true</c> on success, <c>false</c> otherwise.
    /// </returns>
    /// <remarks>
    /// Note that the color and alpha values are not premultiplied.
    /// </remarks>
    public bool TryGetColorStopRgba(int index, out double offset, out double red, out double green, out double blue, out double alpha)
    {
        this.CheckDisposed();

        fixed (double* offsetNative = &offset)
        fixed (double* redNative    = &red)
        fixed (double* greenNative  = &green)
        fixed (double* blueNative   = &blue)
        fixed (double* alphaNative  = &alpha)
        {
            Status status = cairo_pattern_get_color_stop_rgba(this.Handle, index, offsetNative, redNative, greenNative, blueNative, alphaNative);

            if (status == Status.InvalidIndex)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, $"Index must be in the range [0, {this.ColorStopCount}).");
            }

            status.ThrowIfNotSuccess();

            if (offsetNative is null || redNative is null || greenNative is null || blueNative is null || alphaNative is null)
            {
                return false;
            }
        }

        return true;
    }
}

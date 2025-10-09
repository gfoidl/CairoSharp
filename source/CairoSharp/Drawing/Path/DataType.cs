// (c) gfoidl, all rights reserved

namespace Cairo.Drawing.Path;

/// <summary>
/// <see cref="PathData"/> is used to describe the type of one portion of a path when represented
/// as a <see cref="Path"/>.
/// </summary>
/// <remarks>
/// See <see cref="PathData"/> for details.
/// </remarks>
public enum DataType
{
    /// <summary>
    /// A move-to operation, since 1.0
    /// </summary>
    MoveTo,

    /// <summary>
    /// A line-to operation, since 1.0
    /// </summary>
    LineTo,

    /// <summary>
    /// A curve-to operation, since 1.0
    /// </summary>
    CurveTo,

    /// <summary>
    /// A close-path operation, since 1.0
    /// </summary>
    ClosePath
}

public static class DataTypeExtensions
{
    extension(DataType dataType)
    {
        /// <summary>
        /// Gets the number of data points for the <see cref="DataType"/>.
        /// </summary>
        public int PointCount
        {
            get
            {
                return dataType switch
                {
                    DataType.MoveTo    => 1,
                    DataType.LineTo    => 1,
                    DataType.CurveTo   => 3,
                    DataType.ClosePath => 0,
                    _                  => throw new InvalidOperationException()
                };
            }
        }
    }
}

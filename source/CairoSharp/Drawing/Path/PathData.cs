// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices;

namespace Cairo.Drawing.Path;

/*
 * In the cairo documentation the types aren't given, but in the cairo source they look like:

typedef enum _cairo_path_data_type {
    CAIRO_PATH_MOVE_TO,
    CAIRO_PATH_LINE_TO,
    CAIRO_PATH_CURVE_TO,
    CAIRO_PATH_CLOSE_PATH
} cairo_path_data_type_t;


typedef union _cairo_path_data_t cairo_path_data_t;
union _cairo_path_data_t {
    struct {
        cairo_path_data_type_t type;
        int length;
    } header;
    struct {
        double x, y;
    } point;
};
 */

/// <summary>
/// <see cref="PathData"/> is used to represent the path data inside a <see cref="Path"/>.
/// </summary>
/// <remarks>
/// The data structure is designed to try to balance the demands of efficiency and ease-of-use.
/// A path is represented as an array of <see cref="PathData"/>, which is a union of headers and points.
/// <para>
/// See <a href="https://www.cairographics.org/manual/cairo-Paths.html#cairo-path-data-t">cairo docs</a>
/// for further information.
/// </para>
/// </remarks>
[StructLayout(LayoutKind.Explicit)]
public struct PathData
{
    [FieldOffset(0)]
    public HeaderRaw Header;

    [FieldOffset(0)]
    public PointRaw Point;

    [StructLayout(LayoutKind.Sequential)]
    public struct HeaderRaw
    {
        public DataType Type;
        public int Length;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct PointRaw
    {
        public double X;
        public double Y;
    }
}

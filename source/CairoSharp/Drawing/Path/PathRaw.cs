// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices;

namespace Cairo.Drawing.Path;

/// <summary>
/// A data structure for holding a path. This data structure serves as the return value for
/// <see cref="PathExtensions.CopyPath(CairoContext)"/> and <see cref="PathExtensions.CopyPathFlat(CairoContext)"/>
/// as well the input value for <see cref="PathExtensions.AppendPath(CairoContext, Path)"/>.
/// </summary>
/// <remarks>
/// See <see cref="PathData"/> for hints on how to iterate over the actual data within the path.
/// <para>
/// The <see cref="Count"/> member gives the number of elements in the data array. This number is
/// larger than the number of independent path portions (defined in <see cref="PathData"/>), since the
/// data includes both headers and coordinates for each portion.
/// </para>
/// </remarks>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct PathRaw
{
    /// <summary>
    /// the current error status
    /// </summary>
    public Status Status;

    /// <summary>
    /// the elements in the path
    /// </summary>
    public PathData* Data;

    /// <summary>
    /// the number of elements in the data array
    /// </summary>
    public int Count;
}

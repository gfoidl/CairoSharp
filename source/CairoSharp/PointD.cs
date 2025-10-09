// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices;

namespace Cairo;

[StructLayout(LayoutKind.Sequential)]
public readonly record struct PointD(double X, double Y);

/// <summary>
/// Specified as a center coordinate and a radius.
/// </summary>
/// <param name="X">x coordinate of the center</param>
/// <param name="Y">y coordinate of the center</param>
/// <param name="Radius">radius of the circle</param>
[StructLayout(LayoutKind.Sequential)]
public readonly record struct PointDWithRadius(double X, double Y, double Radius);

// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices;

namespace Cairo;

/// <summary>
/// Distance.
/// </summary>
/// <param name="Dx">the X offset</param>
/// <param name="Dy">the Y offset</param>
[StructLayout(LayoutKind.Sequential)]
public readonly record struct Distance(double Dx, double Dy);

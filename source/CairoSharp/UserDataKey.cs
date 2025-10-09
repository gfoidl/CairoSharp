// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices;

namespace Cairo;

/// <summary>
/// <see cref="UserDataKey"/> is used for attaching user data to cairo data structures.
/// The actual contents of the struct is never used, and there is no need to initialize
/// the object; only the unique address of a <see cref="UserDataKey"/> object is used.
/// Typically, you would just use the address of a static <see cref="UserDataKey"/> object.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct UserDataKey
{
    /// <summary>
    /// not used; ignore.
    /// </summary>
    public int Unused;
}

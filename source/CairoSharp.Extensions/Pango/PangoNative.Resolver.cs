// (c) gfoidl, all rights reserved

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Cairo.Extensions.Pango;

static partial class PangoNative
{
    [DisallowNull]
    public static DllImportResolver? DllImportResolver { get; set; }
}

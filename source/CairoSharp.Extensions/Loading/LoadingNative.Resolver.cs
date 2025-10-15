// (c) gfoidl, all rights reserved

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Cairo.Extensions.Loading;

static partial class LoadingNative
{
    [DisallowNull]
    public static DllImportResolver? DllImportResolver { get; set; }
}

// (c) gfoidl, all rights reserved

using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Gtk4.Extensions;

internal static unsafe partial class Native
{
    [SupportedOSPlatform("windows")]
    internal static partial class Windows
    {
        public const string User32Dll = "user32.dll";

        [LibraryImport(LibGtkName)]
        [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        [SuppressGCTransition]
        [return: MarshalAs(UnmanagedType.I4)]
        internal static partial bool gdk_win32_surface_is_win32(GdkSurface* surface);

        [LibraryImport(LibGtkName)]
        [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        [SuppressGCTransition]
        internal static partial nint gdk_win32_surface_get_handle(GdkSurface* surface);

        [LibraryImport(User32Dll)]
        [return: MarshalAs(UnmanagedType.I4)]
        internal static partial bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [LibraryImport(User32Dll)]
        [return: MarshalAs(UnmanagedType.I4)]
        internal static partial bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [StructLayout(LayoutKind.Sequential)]
        internal readonly record struct RECT(int Left, int Top, int Right, int Bottom);
    }
}

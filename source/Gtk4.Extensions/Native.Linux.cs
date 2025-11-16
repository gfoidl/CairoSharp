// (c) gfoidl, all rights reserved

using X11NativeWindow = System.Runtime.InteropServices.CULong;
using Display         = Gtk4.Extensions.Native.LinuxX11._XDisplay;

using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Gtk4.Extensions;

internal static unsafe partial class Native
{
    [SupportedOSPlatform("linux")]
    internal static partial class LinuxX11
    {
        public const string LibX11Name = "libX11.so.6";

        [LibraryImport(LibGtkName)]
        [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial X11NativeWindow gdk_x11_surface_get_xid(GdkSurface* surface);

        internal enum _XDisplay;

        [LibraryImport(LibGtkName)]
        [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial Display* gdk_x11_display_get_xdisplay(GdkDisplay* display);

        // https://tronche.com/gui/x/xlib/display/display-macros.html
        [LibraryImport(LibX11Name)]
        [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial X11NativeWindow XRootWindow(Display* display, int screen_number);

        // https://tronche.com/gui/x/xlib/window-information/XGetGeometry.html
        [LibraryImport(LibX11Name)]
        [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        [return: MarshalAs(UnmanagedType.I4)]
        internal static partial bool XGetGeometry(
            Display* display,
            X11NativeWindow d,
            X11NativeWindow* root_return,
            out int x_return,
            out int y_return,
            out uint width_return,
            out uint height_return,
            out uint border_width_return,
            out uint depth_return);

        // https://tronche.com/gui/x/xlib/window-information/XTranslateCoordinates.html
        [LibraryImport(LibX11Name)]
        [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        [return: MarshalAs(UnmanagedType.I4)]
        internal static partial bool XTranslateCoordinates(
            Display* display,
            X11NativeWindow src_w,
            X11NativeWindow dest_w,
            int src_x,
            int src_y,
            out int dest_x_return,
            out int dest_y_return,
            X11NativeWindow* child_return);

        // https://tronche.com/gui/x/xlib/window/XMoveWindow.html
        [LibraryImport(LibX11Name)]
        [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
        internal static partial void XMoveWindow(Display* display, X11NativeWindow w, int x, int y);
    }
}

// (c) gfoidl, all rights reserved

using System.Runtime.Versioning;
using Gtk;
using X11NativeWindow = System.Runtime.InteropServices.CULong;
using Display         = Gtk4.Extensions.Native.LinuxX11._XDisplay;
using System.Diagnostics;

namespace Gtk4.Extensions;

internal static unsafe class WindowPositionHelper
{
    private const int Spacing = 0;

    [SupportedOSPlatform("linux")]
    public static bool HintAlignToParentWayland(GdkSurface* surface, Window window, Window parent, AlignPosition alignPosition)
    {
        // Wayland does not support global screen coordinates for security reasons (sniffing on other window positions).
        // So there's nothing one can do here (except just returning false ;-)).

        return false;
    }

    [SupportedOSPlatform("linux")]
    public static bool HintAlignToParentX11(GdkSurface* surface, Window window, Window parent, AlignPosition alignPosition)
    {
        // Only AlignPosition.Right is implemented
        if (alignPosition != AlignPosition.Right)
        {
            return false;
        }

        X11NativeWindow parentX11Window = Native.LinuxX11.gdk_x11_surface_get_xid(parent.GetSurfacePointer());
        GdkDisplay* parentGdkDisplay    = (GdkDisplay*)Gtk.Internal.Widget.GetDisplay(parent.Handle.DangerousGetHandle()).ToPointer();
        Display* parentX11Display       = Native.LinuxX11.gdk_x11_display_get_xdisplay(parentGdkDisplay);

        // The whole screen, see also https://en.wikipedia.org/wiki/Root_window
        X11NativeWindow rootWindow;

        if (!Native.LinuxX11.XGetGeometry(
            parentX11Display,
            parentX11Window,
            &rootWindow,
            out int  parentX,
            out int  parentY,
            out uint parentWidth,
            out uint parentHeight,
            out uint parentBorderWidth,
            out uint parentDepth))
        {
            return false;
        }

#if DEBUG
        X11NativeWindow root = Native.LinuxX11.XRootWindow(parentX11Display, 0);
        Debug.Assert(root.Value == rootWindow.Value);
#endif

        // https://stackoverflow.com/a/23940869/347870
        X11NativeWindow child;
        if (!Native.LinuxX11.XTranslateCoordinates(
            parentX11Display,
            parentX11Window,
            rootWindow,
            parentX,
            parentY,
            out int screenParentX,
            out int screenParentY,
            &child))
        {
            return false;
        }

        parentX = screenParentX;
        parentY = screenParentY;

        // https://discourse.gnome.org/t/set-absolut-window-position-in-gtk4/8552

        X11NativeWindow windowX11Window = Native.LinuxX11.gdk_x11_surface_get_xid(surface);
        GdkDisplay* windowGdkDisplay    = (GdkDisplay*)Gtk.Internal.Widget.GetDisplay(window.Handle.DangerousGetHandle()).ToPointer();
        Display* windowX11Display       = Native.LinuxX11.gdk_x11_display_get_xdisplay(windowGdkDisplay);

        // The width and height members are set to the inside size of the window, not including the border.
        // (cf. https://tronche.com/gui/x/xlib/window-information/XGetWindowAttributes.html).
        // For move it's like:
        // Specify the x and y coordinates, which define the new location of the top-left pixel of the window's
        // border or the window itself if it has no border. 
        int x = parentX + (int)parentWidth + (int)(2 * parentBorderWidth);
        int y = parentY - (int)parentBorderWidth;

#if DEBUG
        Console.WriteLine($"Parent: (x, y) = ({parentX}, {parentY}), (w x h) = ({parentWidth} x {parentHeight})");
#endif

        Native.LinuxX11.XMoveWindow(windowX11Display, windowX11Window, x, y);
        return true;
    }

    [SupportedOSPlatform("windows")]
    public static bool HintAlignToParentWindows(GdkSurface* surface, Window window, Window parent, AlignPosition alignPosition)
    {
        nint windowHwnd = Native.Windows.gdk_win32_surface_get_handle(surface);
        nint parentHwnd = Native.Windows.gdk_win32_surface_get_handle(parent.GetSurfacePointer());

        if (!Native.Windows.GetWindowRect(windowHwnd, out Native.Windows.RECT windowRect))
        {
            return false;
        }

        if (!Native.Windows.GetWindowRect(parentHwnd, out Native.Windows.RECT parentRect))
        {
            return false;
        }

        int windowWidth  = windowRect.Right  - windowRect.Left;
        int windowHeight = windowRect.Bottom - windowRect.Top;

        int x = windowRect.Left;
        int y = windowRect.Top;

        switch (alignPosition)
        {
            case AlignPosition.Left:
            {
                x = parentRect.Left - Spacing - windowWidth;
                y = parentRect.Top;

                break;
            }
            case AlignPosition.Top:
            {
                x = parentRect.Left;
                y = parentRect.Top - Spacing - windowHeight;

                break;
            }
            case AlignPosition.Right:
            {
                x = parentRect.Right + Spacing;
                y = parentRect.Top;

                break;
            }
            case AlignPosition.Bottom:
            {
                x = parentRect.Left;
                y = parentRect.Bottom + Spacing;

                break;
            }
        }

        return Native.Windows.SetWindowPos(windowHwnd, parentHwnd, x, y, windowWidth, windowHeight, 0);
    }
}

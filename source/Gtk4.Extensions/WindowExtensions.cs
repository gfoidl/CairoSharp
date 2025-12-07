// (c) gfoidl, all rights reserved

using Gdk;
using Gtk;

namespace Gtk4.Extensions;

/// <summary>
/// Enumeration for align position in relation to a parent window.
/// </summary>
public enum AlignPosition
{
    Left,
    Top,
    Right,
    Bottom
}

/// <summary>
/// Extensions for <see cref="Window"/>.
/// </summary>
public static unsafe class WindowExtensions
{
    extension(Window window)
    {
        /// <summary>
        /// Tries to dock the window to a <paramref name="parent"/> on the side given by
        /// <paramref name="alignPosition"/>.
        /// </summary>
        /// <param name="parent">The parent window</param>
        /// <param name="alignPosition">The side on which to align in relation to the parent</param>
        /// <returns>
        /// <c>false</c> when the dock hint could not be executed. <c>true</c> when the hint
        /// can be placed, but this still does not mean that the dock is done for sure. Thus
        /// it's only a hint.
        /// </returns>
        /// <remarks>
        /// The implementation is done via native window managers. That is depending on the operating
        /// system and display server different code paths are used. At the moment Wayland, X11, and
        /// Microsoft Windows are supported.
        /// </remarks>
        public bool HintAlignToParent(Window parent, AlignPosition alignPosition = AlignPosition.Right)
        {
            ArgumentNullException.ThrowIfNull(parent);

            GdkSurface* surface = window.GetSurfacePointer();

            if (surface is null)
            {
                return false;
            }

            if (OperatingSystem.IsLinux())
            {
                if (Display.GetDefault()?.GetName().Contains("wayland", StringComparison.OrdinalIgnoreCase) ?? false)
                {
                    return WindowPositionHelper.HintAlignToParentWayland(surface, window, parent, alignPosition);
                }
                else
                {
                    return WindowPositionHelper.HintAlignToParentX11(surface, window, parent, alignPosition);
                }
            }
            else if (OperatingSystem.IsWindows())
            {
                if (Native.Windows.gdk_win32_surface_is_win32(surface))
                {
                    return WindowPositionHelper.HintAlignToParentWindows(surface, window, parent, alignPosition);
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the <see cref="Surface"/> associated with the window.
        /// </summary>
        /// <returns>The surface for the window</returns>
        public Surface? GetSurface()
        {
            // Bug in GirCore, cf. https://github.com/gircore/gir.core/issues/1307
            //Surface? surface = window.GetSurface();
            GdkSurface* surface = window.GetSurfacePointer();

            if (surface is null)
            {
                return null;
            }

            return new Surface(new Gdk.Internal.SurfaceHandle((nint)surface, ownsHandle: false));
        }

        internal GdkSurface* GetSurfacePointer()
        {
            nint surfacePointer = Gtk.Internal.Native.GetSurface(window.Handle.DangerousGetHandle());
            return (GdkSurface*)surfacePointer.ToPointer();
        }
    }
}

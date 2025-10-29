// (c) gfoidl, all rights reserved

global using unsafe GtkDrawingAreaDrawFunc = delegate*<Cairo.Extensions.Gtk4.GtkDrawingArea*, Cairo.cairo_t*, int, int, void*, void>;
global using unsafe GDestroyNotify         = delegate*<void*, void>;

#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
global using unsafe gpointer = void*;
#pragma warning restore CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.

using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Cairo.Extensions.Gtk4;

internal struct GtkDrawingArea;

internal static unsafe partial class Native
{
    public const string LibGtkName = "GTK";

    [LibraryImport(LibGtkName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void gtk_drawing_area_set_draw_func(GtkDrawingArea* self, GtkDrawingAreaDrawFunc draw_func, gpointer user_data, GDestroyNotify destroy);

    public static void Destroy(gpointer userData)
    {
        if (userData is null)
        {
            return;
        }

        GCHandle gcHandle = GCHandle.FromIntPtr(new IntPtr(userData));
        Debug.Assert(gcHandle.IsAllocated);

        gcHandle.Free();
    }
}

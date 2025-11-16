// (c) gfoidl, all rights reserved

global using unsafe GtkDrawingAreaDrawFunc = delegate*<Gtk4.Extensions.GtkDrawingArea*, Cairo.cairo_t*, int, int, void*, void>;
global using unsafe GDestroyNotify         = delegate*<void*, void>;
global using unsafe GClosureNotify         = delegate*<void*, void*, void>;

#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
global using unsafe gpointer = void*;
global using        gboolean = int;
#pragma warning restore CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.

using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Gtk4.Extensions;

internal struct GtkDrawingArea;
internal struct GdkSurface;
internal struct GdkDisplay;

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

    [LibraryImport(LibGtkName)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial IntPtr gtk_cclosure_expression_new(
        nuint            value_type,
        void*            marshal,
        uint             n_params,
        void**           @params,
        PropertyAccessor callback_func,
        void*            user_data,
        GClosureNotify   user_destroy);

    public delegate string PropertyAccessor(IntPtr obj);

    public static void ClosureDestroy(void* data, void* closure)
    {
        GCHandle gcHandle = GCHandle.FromIntPtr(new IntPtr(data));
        Debug.Assert(gcHandle.IsAllocated);

        gcHandle.Free();
    }
}

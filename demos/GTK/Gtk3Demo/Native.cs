// (c) gfoidl, all rights reserved

using System.Reflection;
using System.Runtime.InteropServices;
using Cairo;
using IOPath = System.IO.Path;

namespace Gtk3Demo;

internal struct GtkApplication;
internal struct GtkWidget;

internal static unsafe partial class Native
{
    private const string LibGLibName    = "libglib-2.0.so.0";
    private const string LibGObjectName = "libgobject-2.0.so.0";
    private const string LibGioName     = "libgio-2.0.so.0";
    private const string LibGtkName     = "libgtk-3.so.0";

    static Native()
    {
        if (OperatingSystem.IsWindows())
        {
            NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), static (string libraryName, Assembly assembly, DllImportSearchPath? searchPath) =>
            {
                string? path = libraryName switch
                {
                    // For simplicity we re-use the DLLs from Inkscape.
                    LibGLibName    => IOPath.Combine(@"C:\Program Files\Inkscape\bin", "libglib-2.0-0.dll"),
                    LibGObjectName => IOPath.Combine(@"C:\Program Files\Inkscape\bin", "libgobject-2.0-0.dll"),
                    LibGioName     => IOPath.Combine(@"C:\Program Files\Inkscape\bin", "libgio-2.0-0.dll"),
                    LibGtkName     => IOPath.Combine(@"C:\Program Files\Inkscape\bin", "libgtk-3-0.dll"),
                    _              => null
                };

                if (path is not null && NativeLibrary.TryLoad(path, out nint handle))
                {
                    return handle;
                }

                return default;
            });
        }
    }

    public const int True  = 1;
    public const int False = 0;

    [LibraryImport(LibGObjectName)]
    internal static partial void g_object_unref(void* @object);

    [LibraryImport(LibGObjectName, StringMarshalling = StringMarshalling.Utf8)]
    private static partial void g_signal_connect_data(void* instance, string detailed_signal, void* c_handler, void* data, void* destroy_data, int connect_flags);

    internal static void g_signal_connect(void* instance, string detailed_signal, delegate* unmanaged[Cdecl]<void*, void*, void> c_handler, void* data)
        => g_signal_connect_data(instance, detailed_signal, c_handler, data, null, 0);

    internal static void g_signal_connect(void* instance, string detailed_signal, delegate* unmanaged[Cdecl]<void> c_handler, void* data)
        => g_signal_connect_data(instance, detailed_signal, c_handler, data, null, 0);

    internal static void g_signal_connect(void* instance, string detailed_signal, delegate* unmanaged[Cdecl]<GtkWidget*, cairo_t*, void*, int> c_handler, void* data)
        => g_signal_connect_data(instance, detailed_signal, c_handler, data, null, 0);

    [LibraryImport(LibGioName, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial int g_application_run(GtkApplication* application, int argc, ReadOnlySpan<string> argv);

    [LibraryImport(LibGtkName, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial GtkApplication* gtk_application_new(string schema, int flags);

    [LibraryImport(LibGtkName)]
    internal static partial GtkWidget* gtk_application_window_new(GtkApplication* application);

    [LibraryImport(LibGtkName, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial void gtk_window_set_title(void* window, string title);

    [LibraryImport(LibGtkName)]
    internal static partial void gtk_window_set_default_size(void* window, int width, int height);

    [LibraryImport(LibGtkName)]
    internal static partial void gtk_container_set_border_width(void* container, int border_width);

    [LibraryImport(LibGtkName, StringMarshalling = StringMarshalling.Utf8)]
    internal static partial GtkWidget* gtk_frame_new(string? label);

    [LibraryImport(LibGtkName)]
    internal static partial void gtk_container_add(void* container, GtkWidget* widget);

    [LibraryImport(LibGtkName)]
    internal static partial void gtk_frame_set_shadow_type(void* frame, int type);

    [LibraryImport(LibGtkName)]
    internal static partial GtkWidget* gtk_drawing_area_new();

    [LibraryImport(LibGtkName)]
    internal static partial void gtk_widget_set_size_request(GtkWidget* widget, int width, int height);

    [LibraryImport(LibGtkName)]
    internal static partial void gtk_widget_show_all(GtkWidget* widget);
}

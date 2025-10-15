// (c) gfoidl, all rights reserved

global using unsafe csi_destroy_func_t        = delegate*<void*, void*, void>;
global using unsafe csi_surface_create_func_t = delegate*<void*, Cairo.Surfaces.Content, double, double, System.Runtime.InteropServices.CLong, Cairo.Surfaces.cairo_surface_t*>;
global using unsafe csi_context_create_func_t = delegate*<void*, Cairo.Surfaces.cairo_surface_t*, Cairo.cairo_t*>;
global using unsafe csi_show_page_func_t      = delegate*<void*, Cairo.cairo_t*, void>;
global using unsafe csi_copy_page_func_t      = delegate*<void*, Cairo.cairo_t*, void>;
global using unsafe csi_create_source_image_t = delegate*<void*, Cairo.Surfaces.Format, int, int, System.Runtime.InteropServices.CLong, Cairo.Surfaces.cairo_surface_t*>;

using System.Runtime.InteropServices;

namespace Cairo.Extensions.Loading.Script;

internal struct cairo_script_interpreter_t;

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct cairo_script_interpreter_hooks_t
{
    public void*                     closure;
    public csi_surface_create_func_t surface_create;
    public csi_destroy_func_t        surface_destroy;
    public csi_context_create_func_t context_create;
    public csi_destroy_func_t        context_destroy;
    public csi_show_page_func_t      show_page;
    public csi_copy_page_func_t      copy_page;
    public csi_create_source_image_t create_source_image;
}

internal unsafe partial class ScriptNative
{
    public const string LibCairoScriptInterpreter = "cairo-script-interpreter";

    [LibraryImport(LibCairoScriptInterpreter)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_script_interpreter_t* cairo_script_interpreter_create();

    [LibraryImport(LibCairoScriptInterpreter, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial void cairo_script_interpreter_install_hooks(cairo_script_interpreter_t* ctx, cairo_script_interpreter_hooks_t* hooks);

    [LibraryImport(LibCairoScriptInterpreter, StringMarshalling = StringMarshalling.Utf8)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial Status cairo_script_interpreter_run(cairo_script_interpreter_t* ctx, string filename);

    [LibraryImport(LibCairoScriptInterpreter)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial uint cairo_script_interpreter_get_line_number(cairo_script_interpreter_t* ctx);

    [LibraryImport(LibCairoScriptInterpreter)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial cairo_script_interpreter_t* cairo_script_interpreter_reference(cairo_script_interpreter_t* ctx);

    [LibraryImport(LibCairoScriptInterpreter)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial Status cairo_script_interpreter_finish(cairo_script_interpreter_t* ctx);

    [LibraryImport(LibCairoScriptInterpreter)]
    [UnmanagedCallConv(CallConvs = new Type[] { typeof(System.Runtime.CompilerServices.CallConvCdecl) })]
    internal static partial Status cairo_script_interpreter_destroy(cairo_script_interpreter_t* ctx);
}

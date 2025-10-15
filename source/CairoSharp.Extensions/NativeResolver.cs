// (c) gfoidl, all rights reserved

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Cairo.Extensions.Fonts.FreeType;
using Cairo.Extensions.Loading;
using Cairo.Extensions.Loading.Script;

namespace Cairo.Extensions;

internal static class NativeResolver
{
#pragma warning disable CA2255      // The 'ModuleInitializer' attribute should not be used in libraries
    // The module initializer is needed as every "feature" has it's own native class.
    // Therefore w/o this initializer the DllImportResolver would not be set.
    [ModuleInitializer]
    public static void ModuleInitializer()
    {
        NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), static (libraryName, assembly, searchPath) =>
        {
            return libraryName switch
            {
                FreeTypeNative.LibFreeType             => FreeTypeNative.Resolver(FreeTypeNative.LibFreeType            , assembly, searchPath),
                ScriptNative.LibCairoScriptInterpreter => ScriptNative  .Resolver(ScriptNative.LibCairoScriptInterpreter, assembly, searchPath),
                _                                      => LoadingNative.DllImportResolver?.Invoke(libraryName           , assembly, searchPath) ?? default
            };
        });
    }
#pragma warning restore CA2255      // The 'ModuleInitializer' attribute should not be used in libraries
}

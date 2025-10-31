// (c) gfoidl, all rights reserved

// This won't work due to "Fatal error. Invalid Program: attempted to call a UnmanagedCallersOnly method from managed code."
// Most likely because some signal in GirCore is managed.
//#define USE_NATIVE_DIRECT

#if USE_NATIVE_DIRECT
using System.Diagnostics;
using System.Runtime.InteropServices;
#else
using GtkCairo = Cairo.Context;
#endif

using Gtk;
using Cairo;

namespace Gtk4.Extensions;

public delegate void DrawingAreaDrawFunc(DrawingArea drawingArea, CairoContext cr, int width, int height);

public static unsafe class DrawingAreaExtensions
{
    extension(DrawingArea drawingArea)
    {
        public void SetDrawFunc(DrawingAreaDrawFunc drawFunc)
        {
            ArgumentNullException.ThrowIfNull(drawFunc);

#if USE_NATIVE_DIRECT
            GCHandle gcHandle    = GCHandle.Alloc(drawFunc, GCHandleType.Normal);
            GtkDrawingArea* self = (GtkDrawingArea*)drawingArea.Handle.DangerousGetHandle().ToPointer();

            Native.gtk_drawing_area_set_draw_func(self, &DrawFunc, GCHandle.ToIntPtr(gcHandle).ToPointer(), &Native.Destroy);
#else
            drawingArea.SetDrawFunc((DrawingArea drawingArea, GtkCairo gtkCairo, int width, int height) =>
            {
                using CairoContext cr = new(gtkCairo.Handle.DangerousGetHandle());
                drawFunc(drawingArea, cr, width, height);
            });
#endif
        }
    }

#if USE_NATIVE_DIRECT
    private static void DrawFunc(GtkDrawingArea* drawingArea, cairo_t* cr, int width, int height, gpointer userData)
    {
        GCHandle gcHandle = GCHandle.FromIntPtr(new IntPtr(userData));
        Debug.Assert(gcHandle.IsAllocated);

        using CairoContext context   = new(cr, isOwnedByCairo: false, needsDestroy: false);
        DrawingArea da               = new(new Gtk.Internal.DrawingAreaHandle(new IntPtr(drawingArea), ownsHandle: false));
        DrawingAreaDrawFunc drawFunc = (gcHandle.Target as DrawingAreaDrawFunc)!;

        drawFunc(da, context, width, height);
    }
#endif
}

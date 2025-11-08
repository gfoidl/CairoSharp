// (c) gfoidl, all rights reserved

// This won't work due to "Fatal error. Invalid Program: attempted to call a UnmanagedCallersOnly method from managed code."
// Most likely because some signal in GirCore is managed.
//#define USE_NATIVE_DIRECT

#if USE_NATIVE_DIRECT
using System.Runtime.InteropServices;
#else
using GtkCairo = Cairo.Context;
#endif

using Gtk;
using Cairo;
using System.Diagnostics;

namespace Gtk4.Extensions;

public delegate void DrawingAreaDrawFunc(DrawingArea drawingArea, CairoContext cr, int width, int height);

/// <summary>
/// Extension methods for GTK4's <see cref="DrawingArea"/>.
/// </summary>
public static class DrawingAreaExtensions
{
    extension(DrawingArea drawingArea)
    {
        /// <summary>
        /// Sets the draw function for the <see cref="DrawingArea"/>.
        /// </summary>
        /// <param name="drawFunc">
        /// Callback that lets you draw the drawing area’s contents with <see cref="Cairo"/>.
        /// </param>
        /// <remarks>
        /// Setting a draw function is the main thing you want to do when using a drawing area.
        /// <para>
        /// The draw function is called whenever GTK needs to draw the contents of the drawing area to the screen.
        /// </para>
        /// <para>
        /// The draw function will be called during the drawing stage of GTK. In the drawing stage it is not
        /// allowed to change properties of any GTK widgets or call any functions that would cause any properties
        /// to be changed. You should restrict yourself exclusively to drawing your contents in the draw function.
        /// </para>
        /// <para>
        /// If what you are drawing does change, call <see cref="Widget.QueueDraw"/> on the drawing area. This
        /// will cause a redraw and will call <paramref name="drawFunc"/> again.
        /// </para>
        /// </remarks>
        public unsafe void SetDrawFunc(DrawingAreaDrawFunc drawFunc)
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

        /// <summary>
        /// Saves the <see cref="DrawingArea"/> as PNG.
        /// </summary>
        /// <param name="fileName">The filename of the PNG that should be created.</param>
        /// <returns>
        /// <c>true</c> if saving succeeded, <c>false</c> otherwise.
        /// </returns>
        public bool SaveAsPng(string fileName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(fileName);

            // Based on https://discourse.gnome.org/t/gtk4-screenshot-with-gtksnapshot/27981/3

            using Snapshot snapshot = Snapshot.New();
            //drawingArea.Snapshot(snapshot);
            // The above is a vfunc in GTK and a) not available in GirCore, and b) couldn't be used
            // from outside the object anyway. Thus we use a workaround.
            drawingArea.Parent!.SnapshotChild(drawingArea, snapshot);

            Gsk.RenderNode? renderNode = snapshot.FreeToNode();
            Debug.Assert(renderNode is not null);

            try
            {
                // Just for fun
                SaveRenderNode(fileName, renderNode);

                using Gsk.Renderer? renderer = drawingArea.GetNative()?.GetRenderer();
                Debug.Assert(renderer is not null);

                // Don't dispose the texture (it leads to a segfault on Linux).
                Gdk.Texture texture = renderer.RenderTexture(renderNode, null);
                return texture.SaveToPng(fileName);
            }
            finally
            {
                // IMO a Dispose method should be exposed by GirCore
                renderNode.Unref();
            }

            [Conditional("DEBUG")]
            static void SaveRenderNode(string fileName, Gsk.RenderNode renderNode)
            {
                string path = System.IO.Path.ChangeExtension(fileName, ".node");
                renderNode.WriteToFile(path);
            }
        }

        /// <summary>
        /// Saves the <see cref="DrawingArea"/> as PNG.
        /// </summary>
        /// <param name="parent">The parent window</param>
        /// <param name="initialName">
        /// The initial filename (may be overwritten by the user in the dialog) without extension.
        /// </param>
        /// <returns>A task that completes when the file is chosen in the dialog, and the PNG is saved.</returns>
        /// <remarks>
        /// A "file save" dialog will be shown to allow selecting the filename.
        /// </remarks>
        public async Task<bool> SaveAsPngWithFileDialog(Window? parent = null, string? initialName = null)
        {
            using FileFilter fileFilter = FileFilter.New();
            fileFilter.AddSuffix("png");

            using FileDialog fileDialog = FileDialog.New();
            fileDialog.DefaultFilter    = fileFilter;

            if (initialName is not null)
            {
                fileDialog.InitialName = $"{initialName}.png";
            }

            try
            {
                using Gio.File? file = await fileDialog.SaveAsync(parent);

                if (file?.GetPath() is string path)
                {
                    return SaveAsPng(drawingArea, path);
                }
            }
            catch (GLib.GException ex) when (ex.Message == "Dismissed by user")
            {
                // ignore
            }

            return false;
        }
    }

#if USE_NATIVE_DIRECT
    private static unsafe void DrawFunc(GtkDrawingArea* drawingArea, cairo_t* cr, int width, int height, gpointer userData)
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

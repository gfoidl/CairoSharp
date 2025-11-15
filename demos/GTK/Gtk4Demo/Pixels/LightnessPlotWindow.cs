// (c) gfoidl, all rights reserved

extern alias CairoSharp;

using Cairo.Extensions.Colors;
using Cairo.Extensions.Colors.ColorMaps;
using CairoSharp::Cairo;
using CairoSharp::Cairo.Surfaces.Recording;
using Gtk;
using Gtk4.Extensions;

namespace Gtk4Demo.Pixels;

public sealed class LightnessPlotWindow : Window
{
    private readonly DrawingArea _drawingArea;
    private ColorMap?            _colorMap;

    public LightnessPlotWindow(Window? parent = null)
    {
        this.Title       = "Lightness characteristics";
        this.HideOnClose = true;

        DrawingArea drawingArea   = DrawingArea.New();
        drawingArea.WidthRequest  = 400;
        drawingArea.HeightRequest = 400;
        drawingArea.AddCssClass("lightness-drawing-area");

        drawingArea.SetDrawFunc(this.Draw);
        drawingArea.OnResize += static (drawingArea, args) => drawingArea.QueueDraw();

        this.Child = _drawingArea = drawingArea;

        if (parent is not null)
        {
            // When the parent is destroyed, this window will also be destroyed.
            this.DestroyWithParent = true;

            // Tries to center the window on the parent.
            //this.SetTransientFor(parent);

            // Realize is a tad too early, so Map is fine.
            this.OnMap += (s, e) =>
            {
                bool success = this.HintAlignToParent(parent, AlignPosition.Right);
                Console.WriteLine($"Hint to align to parent success = {success}");
            };
        }
    }

    public void SetColorMapAndPresent(ColorMap colorMap)
    {
        _colorMap = colorMap;
        _drawingArea.QueueDraw();
        this.Present();
    }

    private void Draw(DrawingArea drawingArea, CairoContext cr, int width, int height)
    {
        if (_colorMap is null)
        {
            return;
        }

        using RecordingSurface recordingSurface = _colorMap.PlotLightnessCharacteristics();
        Rectangle inkExtents                    = recordingSurface.GetInkExtents();

        double xScale = width  / inkExtents.Width;
        double yScale = height / inkExtents.Height;
        double scale  = Math.Min(xScale, yScale);

        double scaledInkWidth  = inkExtents.Width  * scale;
        double scaledInkHeight = inkExtents.Height * scale;

        double xOffsetForCentered = (width  - scaledInkWidth)  / 2;
        double yOffsetForCentered = (height - scaledInkHeight) / 2;

        cr.Translate(xOffsetForCentered, yOffsetForCentered);
        cr.Scale(scale, scale);

        cr.Color = KnownColors.White;
        cr.Rectangle(0, 0, inkExtents.Width, inkExtents.Height);
        cr.Fill();

        // Need to set at (-x, -y)
        cr.SetSourceSurface(recordingSurface, -inkExtents.X, -inkExtents.Y);
        cr.Paint();
    }
}

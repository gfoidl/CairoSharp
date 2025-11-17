// (c) gfoidl, all rights reserved

extern alias CairoSharp;

using System.Diagnostics;
using Cairo.Extensions.Colors;
using Cairo.Extensions.Colors.ColorMaps;
using Cairo.Fonts.FreeType;
using CairoSharp::Cairo;
using CairoSharp::Cairo.Fonts;
using CairoSharp::Cairo.Fonts.FreeType;
using CairoSharp::Cairo.Surfaces.Recording;
using Gtk;
using Gtk4.Extensions;
using static Gtk4.Extensions.Gtk4Constants;

namespace Gtk4Demo.Pixels;

public sealed class LightnessPlotWindow : Window
{
    private readonly DrawingArea _drawingArea;
    private ColorMap?            _colorMap;
    private FontFace?            _fontFace;

    public LightnessPlotWindow(Window? parent = null)
    {
        this.Title       = "Lightness characteristics";
        this.HideOnClose = true;
        //this.AddCssClass("lightness");

        DrawingArea drawingArea   = DrawingArea.New();
        drawingArea.WidthRequest  = 400;        // min width and height (can't be smaller, but can be larger)
        drawingArea.HeightRequest = 400;
        drawingArea.AddCssClass("drawing-area-lightness");

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

        this.DrawingAreaAddContextMenu();
        this.OnUnrealize += (s, e) => this.OnUnrealizeHandler();

#if USE_FREE_TYPE_FONT
        using Stream fontStream = typeof(LightnessPlotWindow).Assembly.GetManifestResourceStream("Gtk4Demo.fonts.SplineSans.ttf")!;
        _fontFace               = FreeTypeFont.LoadFromStream(fontStream);
#endif
    }

    private void DrawingAreaAddContextMenu()
    {
        Popover popover = Popover.New();
        popover.SetParent(_drawingArea);

        GestureClick clickGesture = GestureClick.New();
        clickGesture.Button       = GdkButtonSecondary;
        clickGesture.OnPressed   += (GestureClick gesture, GestureClick.PressedSignalArgs signalArgs) =>
        {
            Debug.Assert(gesture.GetCurrentButton() == GdkButtonSecondary);

            popover.SetPointingTo(new Gdk.Rectangle()
            {
                X      = (int)signalArgs.X,
                Y      = (int)signalArgs.Y,
                Width  = 1,
                Height = 1
            });

            popover.Popup();
        };

        _drawingArea.AddController(clickGesture);

        Button fontFaceChooserButton = Button.NewWithLabel("Choose font");
        fontFaceChooserButton.AddCssClass("popover-font-chooser");
        popover.SetChild(fontFaceChooserButton);

        fontFaceChooserButton.OnClicked += async (s, e) =>
        {
            try
            {
                using FontDialog fontDialog = FontDialog.New();
                Pango.FontFace? fontFace    = await fontDialog.ChooseFaceAsync(this, null);

                if (fontFace is not null)
                {
                    Console.WriteLine();
                    Console.WriteLine($"""
                        face name:   {fontFace.GetFaceName()}
                        font family: {fontFace.GetFamily().Name} (is monospace: {fontFace.GetFamily().IsMonospace}, is variable: {fontFace.GetFamily().IsVariable})
                        """);
                }
            }
            catch (GLib.GException ex) when (ex.Message == "Dismissed by user")
            {
                // ignore
            }
        };
    }

    private void OnUnrealizeHandler() => _fontFace?.Dispose();

    public override void Dispose()
    {
        this.OnUnrealizeHandler();
        base.Dispose();
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

        using RecordingSurface recordingSurface = _colorMap.PlotLightnessCharacteristics(fontFace: _fontFace);
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

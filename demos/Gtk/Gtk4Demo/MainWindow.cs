// (c) gfoidl, all rights reserved

extern alias CairoSharp;

using System.Diagnostics;
using CairoSharp::Cairo;
using CairoSharp::Cairo.Drawing;
using CairoSharp::Cairo.Drawing.Path;
using CairoSharp::Cairo.Drawing.Patterns;
using CairoSharp::Cairo.Drawing.Text;
using CairoSharp::Cairo.Fonts;
using CairoSharp::Cairo.Fonts.Scaled;
using CairoSharp::Cairo.Surfaces;
using CairoSharp::Cairo.Surfaces.Images;
using CairoSharp::Cairo.Surfaces.Recording;
using Gtk;
using Gtk4.Extensions;
using Path = CairoSharp::Cairo.Drawing.Path.Path;

namespace Gtk4Demo;

public sealed class MainWindow : ApplicationWindow
{
    // The should be exposed by GirCore ideally.
    private const uint GdkButtonAll       = 0;
    private const uint GdkButtonPrimary   = 1;
    private const uint GdkButtonMiddle    = 2;
    private const uint GdkButtonSecondary = 3;

    private string?               _lastSelectedDemo;
    private Action<CairoContext>? _onDrawAction;
    private readonly byte[]       _pngData = File.ReadAllBytes("romedalen.png");
    private Path?                 _hitPath;
    private bool                  _isInHitArea;

#pragma warning disable CS0649 // field is never assigned to
    [Connect] private readonly DrawingArea _drawingArea;
#pragma warning restore CS0649

    public MainWindow(Application app) : this(app, new Builder("demo.4.ui"), "mainWindow") { }

    private MainWindow(Application app, Builder builder, string name)
        : base(new Gtk.Internal.ApplicationWindowHandle(builder.GetPointer(name), ownsHandle: false))
    {
        this.Application = app;

        Gio.MenuModel? mainMenu = builder.GetObject("mainMenu") as Gio.MenuModel;
        Debug.Assert(mainMenu is not null);
        app.SetMenubar(mainMenu);

        builder.Connect(this);
        builder.Dispose();

        this.AddMenuActions();

        Debug.Assert(_drawingArea is not null);
        _drawingArea.SetDrawFunc(this.Draw);

        GestureClick clickGesture = GestureClick.New();
        clickGesture.Button       = GdkButtonAll;
        clickGesture.OnPressed   += this.DrawingAreaClicked;
        _drawingArea.AddController(clickGesture);
    }

    private void AddMenuActions()
    {
        this.AddAction("saveAsPng"           , this.SaveAsPng);
        this.AddAction("drawArc"             , this.DrawArc);
        this.AddAction("drawArcNegative"     , this.DrawArcNegative);
        this.AddAction("drawClip"            , this.DrawClip);
        this.AddAction("drawClipImage"       , this.DrawClipImage);
        this.AddAction("drawCurvedRectangle" , this.DrawCurvedRectangle);
        this.AddAction("drawRoundedRectangle", this.DrawRoundedRectangle);
        this.AddAction("drawCurveTo"         , this.DrawCurveTo);
        this.AddAction("drawDash"            , this.DrawDash);
        this.AddAction("drawFillAndStroke"   , this.DrawFillAndStroke);
        this.AddAction("drawFillStyle"       , this.DrawFillStyle);
        this.AddAction("drawGradient"        , this.DrawGradient);
        this.AddAction("drawImage"           , this.DrawImage);
        this.AddAction("drawImagePattern"    , this.DrawImagePattern);
        this.AddAction("drawMultiSegmentCaps", this.DrawMultiSegmentCaps);
        this.AddAction("drawLineCap"         , this.DrawLineCap);
        this.AddAction("drawLineJoin"        , this.DrawLineJoin);
        this.AddAction("drawText"            , this.DrawText);
        this.AddAction("drawTextCenter"      , this.DrawTextCenter);
        this.AddAction("drawTextExtents"     , this.DrawTextExtents);
        this.AddAction("drawGlyphs"          , this.DrawGlyphs);
        this.AddAction("drawGlyphExtents"    , this.DrawGlyphExtents);
        this.AddAction("hitTest"             , this.DrawHitTest);
    }

    private async Task SaveAsPng()
    {
        using FileFilter fileFilter = FileFilter.New();
        fileFilter.AddSuffix("png");

        using FileDialog fileDialog = FileDialog.New();
        fileDialog.InitialName      = $"{_lastSelectedDemo}.png";
        fileDialog.DefaultFilter    = fileFilter;

        try
        {
            using Gio.File? file = await fileDialog.SaveAsync(this);

            if (file?.GetPath() is string path)
            {
                // Based on https://discourse.gnome.org/t/gtk4-screenshot-with-gtksnapshot/27981/3

                using Snapshot snapshot = Snapshot.New();
                // _drawingArea.Snapshot(snapshot);
                // The above isn't available in GirCore, so use this workaround:
                _drawingArea.Parent!.SnapshotChild(_drawingArea, snapshot);

                Gsk.RenderNode? renderNode = snapshot.FreeToNode();
                Debug.Assert(renderNode is not null);

                try
                {
                    // Just for fun
                    renderNode.WriteToFile($"{path}.node");

                    using Gsk.Renderer? renderer = _drawingArea.GetNative()?.GetRenderer();
                    Debug.Assert(renderer is not null);

                    using Gdk.Texture texture = renderer.RenderTexture(renderNode, null);
                    texture.SaveToPng(path);
                }
                finally
                {
                    // IMO a Dispose method should be exposed by GirCore
                    renderNode.Unref();
                }
            }
        }
        catch (GLib.GException ex) when (ex.Message == "Dismissed by user")
        {
            // ignore
        }
    }

    private void Draw(DrawingArea drawingArea, CairoContext cr, int width, int height)
    {
        if (_lastSelectedDemo != "hit path" && _hitPath is not null)
        {
            _hitPath.Dispose();
        }

        using (cr.Save())
        {
            cr.Rectangle(0, 0, drawingArea.ContentWidth, drawingArea.ContentHeight);
            cr.LineWidth = 1d;
            cr.Stroke();
        }

        _onDrawAction?.Invoke(cr);
    }

    private void DrawArc()
    {
        _lastSelectedDemo = "arc";
        _onDrawAction     = static cr =>
        {
            const double Xc     = 128.0;
            const double Yc     = 128.0;
            const double Radius = 100.0;
            double angle1       = 45d.DegreesToRadians();
            double angle2       = 180d.DegreesToRadians();

            cr.LineWidth = 10.0;
            cr.Arc(Xc, Yc, Radius, angle1, angle2);
            cr.Stroke();

            // draw helping lines 
            cr.SetSourceRgba(1, 0.2, 0.2, 0.6);
            cr.LineWidth = 6.0;

            cr.Arc(Xc, Yc, 10.0, 0, 2 * Math.PI);
            cr.Fill();

            cr.Arc(Xc, Yc, Radius, angle1, angle1);
            cr.LineTo(Xc, Yc);

            cr.Arc(Xc, Yc, Radius, angle2, angle2);
            cr.LineTo(Xc, Yc);

            cr.Stroke();
        };

        _drawingArea.QueueDraw();
    }

    private void DrawArcNegative()
    {
        _lastSelectedDemo = "arc_negative";
        _onDrawAction     = static cr =>
        {
            const double Xc     = 128.0;
            const double Yc     = 128.0;
            const double Radius = 100.0;
            double angle1       = 45d.DegreesToRadians();
            double angle2       = 180d.DegreesToRadians();

            cr.LineWidth = 10.0;
            cr.ArcNegative(Xc, Yc, Radius, angle1, angle2);
            cr.Stroke();

            // draw helping lines
            cr.SetSourceRgba(1, 0.2, 0.2, 0.6);
            cr.LineWidth = 6.0;

            cr.Arc(Xc, Yc, 10.0, 0, 2 * Math.PI);
            cr.Fill();

            cr.Arc(Xc, Yc, Radius, angle1, angle1);
            cr.LineTo(Xc, Yc);
            cr.Arc(Xc, Yc, Radius, angle2, angle2);
            cr.LineTo(Xc, Yc);
            cr.Stroke();
        };

        _drawingArea.QueueDraw();
    }

    private void DrawClip()
    {
        _lastSelectedDemo = "clip";
        _onDrawAction     = static cr =>
        {
            cr.Arc(128.0, 128.0, 76.8, 0, 2 * Math.PI);
            cr.Clip();

            cr.NewPath();   // current path is not consumed by cairo_clip()

            cr.Rectangle(0, 0, 256, 256);
            cr.Fill();

            cr.SetSourceRgb(0, 1, 0);
            cr.MoveTo(0, 0);
            cr.LineTo(256, 256);
            cr.MoveTo(256, 0);
            cr.LineTo(0, 256);
            cr.LineWidth = 10.0;
            cr.Stroke();
        };

        _drawingArea.QueueDraw();
    }

    private void DrawClipImage()
    {
        _lastSelectedDemo = "clip_image";
        _onDrawAction     = cr =>
        {
            cr.Arc(128.0, 128.0, 76.8, 0, 2 * Math.PI);
            cr.Clip();

            using ImageSurface image = new(_pngData);
            int w = image.Width;
            int h = image.Height;

            cr.Scale(256d / w, 256d / h);

            cr.SetSourceSurface(image, 0, 0);
            cr.Paint();
        };

        _drawingArea.QueueDraw();
    }

    private void DrawCurvedRectangle()
    {
        _lastSelectedDemo = "curved_rectangle";
        _onDrawAction     = static cr =>
        {
            // a custom shape that could be wrapped in a function
            double x0         = 25.6,       // parameters like cairo_rectangle
                   y0         = 25.6,
                   rectWidth  = 204.8,
                   rectHeight = 204.8,
                   radius     = 102.4;      // and an approximate curvature radius

            double x1 = x0 + rectWidth;
            double y1 = y0 + rectHeight;

            if (rectWidth / 2 < radius)
            {
                if (rectHeight / 2 < radius)
                {
                    cr.MoveTo(x0, (y0 + y1) / 2);
                    cr.CurveTo(x0, y0, x0, y0, (x0 + x1) / 2, y0);
                    cr.CurveTo(x1, y0, x1, y0, x1, (y0 + y1) / 2);
                    cr.CurveTo(x1, y1, x1, y1, (x1 + x0) / 2, y1);
                    cr.CurveTo(x0, y1, x0, y1, x0, (y0 + y1) / 2);
                }
                else
                {
                    cr.MoveTo(x0, y0 + radius);
                    cr.CurveTo(x0, y0, x0, y0, (x0 + x1) / 2, y0);
                    cr.CurveTo(x1, y0, x1, y0, x1, y0 + radius);
                    cr.LineTo(x1, y1 - radius);
                    cr.CurveTo(x1, y1, x1, y1, (x1 + x0) / 2, y1);
                    cr.CurveTo(x0, y1, x0, y1, x0, y1 - radius);
                }
            }
            else
            {
                if (rectHeight / 2 < radius)
                {
                    cr.MoveTo(x0, (y0 + y1) / 2);
                    cr.CurveTo(x0, y0, x0, y0, x0 + radius, y0);
                    cr.LineTo(x1 - radius, y0);
                    cr.CurveTo(x1, y0, x1, y0, x1, (y0 + y1) / 2);
                    cr.CurveTo(x1, y1, x1, y1, x1 - radius, y1);
                    cr.LineTo(x0 + radius, y1);
                    cr.CurveTo(x0, y1, x0, y1, x0, (y0 + y1) / 2);
                }
                else
                {
                    cr.MoveTo(x0, y0 + radius);
                    cr.CurveTo(x0, y0, x0, y0, x0 + radius, y0);
                    cr.LineTo(x1 - radius, y0);
                    cr.CurveTo(x1, y0, x1, y0, x1, y0 + radius);
                    cr.LineTo(x1, y1 - radius);
                    cr.CurveTo(x1, y1, x1, y1, x1 - radius, y1);
                    cr.LineTo(x0 + radius, y1);
                    cr.CurveTo(x0, y1, x0, y1, x0, y1 - radius);
                }
            }
            cr.ClosePath();

            cr.SetSourceRgb(0.5, 0.5, 1);
            cr.FillPreserve();
            cr.SetSourceRgba(0.5, 0, 0, 0.5);
            cr.LineWidth = 10.0;
            cr.Stroke();
        };

        _drawingArea.QueueDraw();
    }

    private void DrawRoundedRectangle()
    {
        _lastSelectedDemo = "rounded_rectangle";
        _onDrawAction     = static cr =>
        {
            // a custom shape that could be wrapped in a function
            double x            = 25.6,             // parameters like cairo_rectangle
                   y            = 25.6,
                   width        = 204.8,
                   height       = 204.8,
                   aspect       = 1.0,              // aspect ratio
                   cornerRadius = height / 10.0;    // and corner curvature radius

            double radius = cornerRadius / aspect;
            double degrees = Math.PI / 180.0;

            cr.NewSubPath();
            cr.Arc(x + width - radius, y + radius, radius, -90 * degrees, 0 * degrees);
            cr.Arc(x + width - radius, y + height - radius, radius, 0 * degrees, 90 * degrees);
            cr.Arc(x + radius, y + height - radius, radius, 90 * degrees, 180 * degrees);
            cr.Arc(x + radius, y + radius, radius, 180 * degrees, 270 * degrees);
            cr.ClosePath();

            cr.SetSourceRgb(0.5, 0.5, 1);
            cr.FillPreserve();
            cr.SetSourceRgba(0.5, 0, 0, 0.5);
            cr.LineWidth = 10.0;
            cr.Stroke();
        };

        _drawingArea.QueueDraw();
    }

    private void DrawCurveTo()
    {
        _lastSelectedDemo = "curve_to";
        _onDrawAction     = static cr =>
        {
            double  x = 25.6,   y = 128.0;
            double x1 = 102.4, y1 = 230.4,
                   x2 = 153.6, y2 = 25.6,
                   x3 = 230.4, y3 = 128.0;

            cr.MoveTo(x, y);
            cr.CurveTo(x1, y1, x2, y2, x3, y3);

            cr.LineWidth = 10.0;
            cr.Stroke();

            cr.SetSourceRgba(1, 0.2, 0.2, 0.6);
            cr.LineWidth = 6.0;
            cr.MoveTo(x, y); cr.LineTo(x1, y1);
            cr.MoveTo(x2, y2); cr.LineTo(x3, y3);
            cr.Stroke();
        };

        _drawingArea.QueueDraw();
    }

    private void DrawDash()
    {
        _lastSelectedDemo = "dash";
        _onDrawAction     = static cr =>
        {
            ReadOnlySpan<double> dashes =
            [
                50.0,   // ink
                10.0,   // skip
                10.0,   // ink
                10.0    // skip
            ];
            const double Offset = -50.0;

            cr.SetDash(dashes, Offset);
            cr.LineWidth = 10.0;

            cr.MoveTo(128.0, 25.6);
            cr.LineTo(230.4, 230.4);
            cr.RelLineTo(-102.4, 0.0);
            cr.CurveTo(51.2, 230.4, 51.2, 128.0, 128.0, 128.0);

            cr.Stroke();
        };

        _drawingArea.QueueDraw();
    }

    private void DrawFillAndStroke()
    {
        _lastSelectedDemo = "fill_and_stroke";
        _onDrawAction     = static cr =>
        {
            cr.MoveTo(128.0, 25.6);
            cr.LineTo(230.4, 230.4);
            cr.RelLineTo(-102.4, 0.0);
            cr.CurveTo(51.2, 230.4, 51.2, 128.0, 128.0, 128.0);
            cr.ClosePath();

            cr.MoveTo(64.0, 25.6);
            cr.RelLineTo(51.2, 51.2);
            cr.RelLineTo(-51.2, 51.2);
            cr.RelLineTo(-51.2, -51.2);
            cr.ClosePath();

            cr.LineWidth = 10.0;
            cr.SetSourceRgb(0, 0, 1);
            cr.FillPreserve();
            cr.SetSourceRgb(0, 0, 0);
            cr.Stroke();
        };

        _drawingArea.QueueDraw();
    }

    private void DrawFillStyle()
    {
        _lastSelectedDemo = "fill_style";
        _onDrawAction     = static cr =>
        {
            cr.LineWidth = 6;

            cr.Rectangle(12, 12, 232, 70);
            cr.NewSubPath(); cr.Arc(64, 64, 40, 0, 2 * Math.PI);
            cr.NewSubPath(); cr.ArcNegative(192, 64, 40, 0, -2 * Math.PI);

            cr.FillRule = FillRule.EvenOdd;
            cr.SetSourceRgb(0, 0.7, 0); cr.FillPreserve();
            cr.SetSourceRgb(0, 0, 0); cr.Stroke();

            cr.Translate(0, 128);
            cr.Rectangle(12, 12, 232, 70);
            cr.NewSubPath(); cr.Arc(64, 64, 40, 0, 2 * Math.PI);
            cr.NewSubPath(); cr.ArcNegative(192, 64, 40, 0, -2 * Math.PI);

            cr.FillRule = FillRule.Winding;
            cr.SetSourceRgb(0, 0, 0.9); cr.FillPreserve();
            cr.SetSourceRgb(0, 0, 0); cr.Stroke();
        };

        _drawingArea.QueueDraw();
    }

    private void DrawGradient()
    {
        _lastSelectedDemo = "gradient";
        _onDrawAction     = static cr =>
        {
            using (Gradient pat = new LinearGradient(0.0, 0.0, 0.0, 256.0))
            {
                pat.AddColorStopRgba(0, 1, 1, 1, 1);
                pat.AddColorStopRgba(1, 0, 0, 0, 1);

                cr.Rectangle(0, 0, 256, 256);
                cr.SetSource(pat);
                cr.Fill();
            }

            using (Gradient pat = new RadialGradient(115.2, 102.4, 25.6,
                                                     102.4, 102.4, 128.0))
            {
                pat.AddColorStopRgba(0, 1, 1, 1, 1);
                pat.AddColorStopRgba(1, 0, 0, 0, 1);

                cr.SetSource(pat);
                cr.Arc(128.0, 128.0, 76.8, 0, 2 * Math.PI);
                cr.Fill();
            }
        };

        _drawingArea.QueueDraw();
    }

    private void DrawImage()
    {
        _lastSelectedDemo = "image";
        _onDrawAction     = cr =>
        {
            using ImageSurface image = new(_pngData);
            int w = image.Width;
            int h = image.Height;

            cr.Translate(128.0, 128.0);
            cr.Rotate(45.DegreesToRadians());
            cr.Scale(256.0 / w, 256.0 / h);
            cr.Translate(-0.5 * w, -0.5 * h);

            cr.SetSource(image, 0, 0);
            cr.Paint();
        };

        _drawingArea.QueueDraw();
    }

    private void DrawImagePattern()
    {
        _lastSelectedDemo = "image_pattern";
        _onDrawAction     = cr =>
        {
            using ImageSurface image = new(_pngData);
            int w = image.Width;
            int h = image.Height;

            using SurfacePattern pattern = new(image);
            pattern.Extend               = Extend.Repeat;

            cr.Translate(128.0, 128.0);
            cr.Rotate(Math.PI / 4);
            cr.Scale(1 / Math.Sqrt(2), 1 / Math.Sqrt(2));
            cr.Translate(-128.0, -128.0);

            Matrix matrix = default;
            matrix.InitScale(w / 256.0 * 5.0, h / 256.0 * 5.0);
            pattern.SetMatrix(ref matrix);

            cr.SetSource(pattern);

            cr.Rectangle(0, 0, 256.0, 256.0);
            cr.Fill();
        };

        _drawingArea.QueueDraw();
    }

    private void DrawMultiSegmentCaps()
    {
        _lastSelectedDemo = "multi_segment_caps";
        _onDrawAction     = static cr =>
        {
            cr.MoveTo(50.0, 75.0);
            cr.LineTo(200.0, 75.0);

            cr.MoveTo(50.0, 125.0);
            cr.LineTo(200.0, 125.0);

            cr.MoveTo(50.0, 175.0);
            cr.LineTo(200.0, 175.0);

            cr.LineWidth = 30.0;
            cr.LineCap   = LineCap.Round;
            cr.Stroke();
        };

        _drawingArea.QueueDraw();
    }

    private void DrawLineCap()
    {
        _lastSelectedDemo = "line_cap";
        _onDrawAction     = static cr =>
        {
            cr.LineWidth = 30.0;

            cr.LineCap = LineCap.Butt;  // default
            cr.MoveTo(64.0, 50.0); cr.LineTo(64.0, 200.0);
            cr.Stroke();

            cr.LineCap = LineCap.Round;
            cr.MoveTo(128.0, 50.0); cr.LineTo(128.0, 200.0);
            cr.Stroke();

            cr.LineCap = LineCap.Square;
            cr.MoveTo(192.0, 50.0); cr.LineTo(192.0, 200.0);
            cr.Stroke();

            // draw helping lines
            cr.LineCap = LineCap.Butt;
            cr.SetSourceRgb(1, 0.2, 0.2);
            cr.LineWidth = 2.56;
            cr.MoveTo( 64.0, 50.0); cr.LineTo( 64.0, 200.0);
            cr.MoveTo(128.0, 50.0); cr.LineTo(128.0, 200.0);
            cr.MoveTo(192.0, 50.0); cr.LineTo(192.0, 200.0);
            cr.Stroke();
        };

        _drawingArea.QueueDraw();
    }

    private void DrawLineJoin()
    {
        _lastSelectedDemo = "line_join";
        _onDrawAction     = static cr =>
        {
            cr.LineWidth = 40.96;

            cr.LineJoin = LineJoin.Miter;   // default
            cr.MoveTo(76.8, 84.48);
            cr.RelLineTo(51.2, -51.2);
            cr.RelLineTo(51.2, 51.2);
            cr.Stroke();

            cr.LineJoin = LineJoin.Bevel;
            cr.MoveTo(76.8, 161.28);
            cr.RelLineTo(51.2, -51.2);
            cr.RelLineTo(51.2, 51.2);
            cr.Stroke();

            cr.LineJoin = LineJoin.Round;
            cr.MoveTo(76.8, 238.08);
            cr.RelLineTo(51.2, -51.2);
            cr.RelLineTo(51.2, 51.2);
            cr.Stroke();
        };

        _drawingArea.QueueDraw();
    }

    private void DrawText()
    {
        _lastSelectedDemo = "text";
        _onDrawAction     = static cr =>
        {
            cr.SelectFontFace("Sans", FontSlant.Normal, FontWeight.Bold);
            cr.SetFontSize(90d);

            cr.MoveTo(10.0, 135.0);
            cr.ShowText("Hello");

            cr.MoveTo(70.0, 165.0);
            cr.TextPath("void");
            cr.SetSourceRgb(0.5, 0.5, 1);
            cr.FillPreserve();
            cr.SetSourceRgb(0, 0, 0);
            cr.LineWidth = 2.56;
            cr.Stroke();

            // draw helping lines
            cr.SetSourceRgba(1, 0.2, 0.2, 0.6);

            cr.Arc(10.0, 135.0, 5.12, 0, 2 * Math.PI);
            cr.ClosePath();

            cr.Arc(70.0, 165.0, 5.12, 0, 2 * Math.PI);
            cr.Fill();
        };

        _drawingArea.QueueDraw();
    }

    private void DrawTextCenter()
    {
        _lastSelectedDemo = "text_center";
        _onDrawAction     = static cr =>
        {
            const string Text = "cairo";

            cr.SelectFontFace("Sans", FontSlant.Normal, FontWeight.Normal);
            cr.SetFontSize(52);

            cr.TextExtents(Text, out TextExtents extents);

            double x = 128.0 - (extents.Width  / 2 + extents.XBearing);
            double y = 128.0 - (extents.Height / 2 + extents.YBearing);

            cr.MoveTo(x, y);
            cr.ShowText(Text);

            // draw helping lines
            cr.SetSourceRgba(1, 0.2, 0.2, 0.6);
            cr.LineWidth = 6.0;

            cr.Arc(x, y, 10.0, 0, 2 * Math.PI);
            cr.Fill();

            cr.MoveTo(128.0, 0);
            cr.RelLineTo(0, 256);
            cr.MoveTo(0, 128.0);
            cr.RelLineTo(256, 0);
            cr.Stroke();
        };

        _drawingArea.QueueDraw();
    }

    private void DrawTextExtents()
    {
        _lastSelectedDemo = "text_extents";
        _onDrawAction     = static cr =>
        {
            const string Text = "cairo";

            cr.SelectFontFace("Sans", FontSlant.Normal, FontWeight.Normal);
            cr.SetFontSize(100);

            cr.TextExtents(Text, out TextExtents extents);

            const double X = 25.0;
            const double Y = 150.0;

            cr.MoveTo(X, Y);
            cr.ShowText(Text);

            // draw helping lines
            cr.SetSourceRgba(1, 0.2, 0.2, 0.6);
            cr.LineWidth = 6.0;

            cr.Arc(X, Y, 10.0, 0, 2 * Math.PI);
            cr.Fill();

            cr.MoveTo(X, Y);
            cr.RelLineTo(0, -extents.Height);
            cr.RelLineTo(extents.Width, 0);
            cr.RelLineTo(extents.XBearing, -extents.YBearing);
            cr.Stroke();
        };

        _drawingArea.QueueDraw();
    }

    private void DrawGlyphs()
    {
        _lastSelectedDemo = "glyphs";
        _onDrawAction     = static cr =>
        {
            const string Text = "cairo";

            cr.SelectFontFace("Sans", FontSlant.Normal, FontWeight.Normal);
            cr.SetFontSize(100);

            const double X = 25.0;
            const double Y = 150.0;

            cr.MoveTo(X, Y);

            cr.ShowTextGlyphs(Text);
        };

        _drawingArea.QueueDraw();
    }

    private void DrawGlyphExtents()
    {
        _lastSelectedDemo = "glyph_extents";
        _onDrawAction     = static cr =>
        {
            const string Text = "cairo";

            cr.SelectFontFace("Sans", FontSlant.Normal, FontWeight.Normal);
            cr.SetFontSize(100);

            const double X =  25.0;
            const double Y = 150.0;

            using ScaledFont scaledFont = cr.ScaledFont;
            using GlyphArray glyphs     = scaledFont.TextToGlyphs(X, Y, Text, out TextClusterArray? clusters, out ClusterFlags clusterFlags);
            try
            {
                cr.ShowGlyphs(glyphs);

                // draw helping lines
                scaledFont.GlyphExtents(glyphs.Span, out TextExtents extents);

                cr.SetSourceRgba(1, 0.2, 0.2, 0.6);
                cr.LineWidth = 6.0;

                cr.Arc(X, Y, 10.0, 0, 2 * Math.PI);
                cr.Fill();

                cr.MoveTo(X, Y);
                cr.RelLineTo(0, -extents.Height);
                cr.RelLineTo(extents.Width, 0);
                cr.RelLineTo(extents.XBearing, -extents.YBearing);
                cr.Stroke();
            }
            finally
            {
                clusters?.Dispose();
            }
        };

        _drawingArea.QueueDraw();
    }

    private void DrawHitTest()
    {
        _lastSelectedDemo = "hit_test";
        _onDrawAction     = cr =>
        {
            using (cr.Save())
            {
                cr.Translate(100, 50);
                cr.Scale(2, 2);

                const int X  = 10;
                const int Y  = 10;
                const int Sx = 50;
                const int Sy = 50;

                cr.MoveTo(X, Y);
                cr.LineTo(X + Sx, Y);
                cr.LineTo(X + Sx, Y + Sy);
                cr.LineTo(X + (Sx / 2), Y + Sy);
                cr.LineTo(X + (Sx / 2), Y + (Sy / 2));
                cr.LineTo(X, Y + (Sy / 2));
                cr.LineTo(X, Y + Sy);
                cr.LineTo(X - Sx, Y + Sy);
                cr.ClosePath();
            }

            _hitPath = cr.CopyPath();   // record the path to use as a hit area

            cr.SetSourceRgba(1, 0, 0, 0.5);
            cr.FillPreserve();
            cr.SetSourceRgb(1, 0, 0);

            if (!_isInHitArea)
            {
                cr.Stroke();
            }
            else
            {
                cr.StrokePreserve();

                cr.LineWidth = 5;
                cr.SetSourceRgb(0, 1, 0);
                cr.Stroke();
            }
        };

        _drawingArea.QueueDraw();
    }

    private void DrawingAreaClicked(GestureClick gesture, GestureClick.PressedSignalArgs eventArgs)
    {
        uint clickButton = gesture.GetCurrentButton();

        if (clickButton == GdkButtonSecondary)
        {
            _isInHitArea = false;
            _drawingArea.QueueDraw();
            return;
        }

        if (_hitPath is null || clickButton != GdkButtonPrimary)
        {
            return;
        }

        foreach (Path.PathElement pathElement in _hitPath)
        {
            Debug.WriteLine($"Path item: {pathElement.DataType}");
            foreach (PointD point in pathElement.Points)
            {
                Debug.WriteLine($"\t{point}");
            }
        }

        // Just any CairoContext is needed.
        using RecordingSurface surface = new(Content.Alpha);
        using CairoContext cr          = new(surface);

        cr.AppendPath(_hitPath);
        bool isInHitArea = cr.InFill(eventArgs.X, eventArgs.Y);

        if (_isInHitArea != isInHitArea)
        {
            _drawingArea.QueueDraw();
        }

        _isInHitArea = isInHitArea;

        Console.WriteLine($"click at ({eventArgs.X:N3}, {eventArgs.Y:N3}), is in hit area: {_isInHitArea}");
    }
}

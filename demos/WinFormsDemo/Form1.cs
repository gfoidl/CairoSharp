// (c) gfoidl, all rights reserved

using System.Diagnostics;
using Cairo;
using Cairo.Drawing;
using Cairo.Drawing.Patterns;
using Cairo.Drawing.Text;
using Cairo.Fonts;
using Cairo.Fonts.Scaled;
using Cairo.Surfaces.Images;
using Cairo.Surfaces.Win32;
using Cairo.Utilities;

namespace WinFormsDemo;

public partial class Form1 : Form
{
    private string?               _lastSelectedDemo;
    private Action<CairoContext>? _onPaintAction;
    private readonly byte[]       _pngData = File.ReadAllBytes("romedalen.png");

    public Form1()
    {
        InitializeComponent();

        this.Width = 10 + 256 + 10 + 20;                        // don't know why 20 is needed, but it is so
        this.Height = menuStrip1.Height + 10 + 256 + 10 + 45;   // the same for the 45, maybe pixel <-> points
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        using Graphics graphics    = e.Graphics;
        using Win32Surface surface = new(graphics.GetHdc());
        using CairoContext context = new(surface);

        context.Translate(10, menuStrip1.Height + 10);

        using (context.Save())
        {
            context.Rectangle(0, 0, 256, 256);
            context.LineWidth = 1d;
            context.Stroke();
        }

        if (_onPaintAction is null)
        {
            return;
        }

        _onPaintAction(context);
    }

    private void saveAsPNGToolStripMenuItem_Click(object sender, EventArgs e)
    {
        using SaveFileDialog saveFileDialog = new();
        saveFileDialog.Filter               = "PNG|*.png";
        saveFileDialog.FileName             = _lastSelectedDemo;

        if (saveFileDialog.ShowDialog() == DialogResult.OK)
        {
            using Graphics graphics    = this.CreateGraphics();
            using Win32Surface surface = new(graphics.GetHdc());
            using CairoContext context = new(surface);

            surface.WriteToPng(saveFileDialog.FileName);
        }
    }

    private static string GetMenuText(object sender)
    {
        Debug.Assert(sender is ToolStripMenuItem);
        ToolStripMenuItem menuItem = (sender as ToolStripMenuItem)!;
        return menuItem.Text!;
    }

    private void arcToolStripMenuItem_Click(object sender, EventArgs e)
    {
        _lastSelectedDemo = GetMenuText(sender);
        _onPaintAction    = static cr =>
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

        this.Invalidate();
    }

    private void arcNegativeToolStripMenuItem_Click(object sender, EventArgs e)
    {
        _lastSelectedDemo = GetMenuText(sender);
        _onPaintAction    = static cr =>
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

        this.Invalidate();
    }

    private void clipToolStripMenuItem_Click(object sender, EventArgs e)
    {
        _lastSelectedDemo = GetMenuText(sender);
        _onPaintAction    = static cr =>
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

        this.Invalidate();
    }

    private void clipImageToolStripMenuItem_Click(object sender, EventArgs e)
    {
        _lastSelectedDemo = GetMenuText(sender);
        _onPaintAction    = cr =>
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

        this.Invalidate();
    }

    private void curvedRectangleToolStripMenuItem_Click(object sender, EventArgs e)
    {
        _lastSelectedDemo = GetMenuText(sender);
        _onPaintAction    = static cr =>
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

        this.Invalidate();
    }

    private void roundedRectangleToolStripMenuItem_Click(object sender, EventArgs e)
    {
        _lastSelectedDemo = GetMenuText(sender);
        _onPaintAction    = static cr =>
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

        this.Invalidate();
    }

    private void curveToToolStripMenuItem_Click(object sender, EventArgs e)
    {
        _lastSelectedDemo = GetMenuText(sender);
        _onPaintAction    = static cr =>
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

        this.Invalidate();
    }

    private void dashToolStripMenuItem_Click(object sender, EventArgs e)
    {
        _lastSelectedDemo = GetMenuText(sender);
        _onPaintAction    = static cr =>
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

        this.Invalidate();
    }

    private void fillAndStroke2ToolStripMenuItem_Click(object sender, EventArgs e)
    {
        _lastSelectedDemo = GetMenuText(sender);
        _onPaintAction    = static cr =>
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

        this.Invalidate();
    }

    private void fillStyleToolStripMenuItem_Click(object sender, EventArgs e)
    {
        _lastSelectedDemo = GetMenuText(sender);
        _onPaintAction    = static cr =>
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

        this.Invalidate();
    }

    private void gradientToolStripMenuItem_Click(object sender, EventArgs e)
    {
        _lastSelectedDemo = GetMenuText(sender);
        _onPaintAction    = static cr =>
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

        this.Invalidate();
    }

    private void imageToolStripMenuItem_Click(object sender, EventArgs e)
    {
        _lastSelectedDemo = GetMenuText(sender);
        _onPaintAction    = cr =>
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

        this.Invalidate();
    }

    private void imagePatternToolStripMenuItem_Click(object sender, EventArgs e)
    {
        _lastSelectedDemo = GetMenuText(sender);
        _onPaintAction    = cr =>
        {
            using ImageSurface image = new ImageSurface(_pngData);
            int w = image.Width;
            int h = image.Height;

            using Pattern pattern = new SurfacePattern(image);
            pattern.Extend        = Extend.Repeat;

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

        this.Invalidate();
    }

    private void multiSegmentCapsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        _lastSelectedDemo = GetMenuText(sender);
        _onPaintAction    = static cr =>
        {
            cr.MoveTo(50.0, 75.0);
            cr.LineTo(200.0, 75.0);

            cr.MoveTo(50.0, 125.0);
            cr.LineTo(200.0, 125.0);

            cr.MoveTo(50.0, 175.0);
            cr.LineTo(200.0, 175.0);

            cr.LineWidth = 30.0;
            cr.LineCap = LineCap.Round;
            cr.Stroke();
        };

        this.Invalidate();
    }

    private void setLineCapToolStripMenuItem_Click(object sender, EventArgs e)
    {
        _lastSelectedDemo = GetMenuText(sender);
        _onPaintAction    = static cr =>
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
            cr.MoveTo(64.0, 50.0); cr.LineTo(64.0, 200.0);
            cr.MoveTo(128.0, 50.0); cr.LineTo(128.0, 200.0);
            cr.MoveTo(192.0, 50.0); cr.LineTo(192.0, 200.0);
            cr.Stroke();
        };

        this.Invalidate();
    }

    private void setLineJoinToolStripMenuItem_Click(object sender, EventArgs e)
    {
        _lastSelectedDemo = GetMenuText(sender);
        _onPaintAction    = static cr =>
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

        this.Invalidate();
    }

    private void textToolStripMenuItem_Click(object sender, EventArgs e)
    {
        _lastSelectedDemo = GetMenuText(sender);
        _onPaintAction    = static cr =>
        {
            cr.SelectFontFace("Microsoft Sans Serif", FontSlant.Normal, FontWeight.Bold);
            cr.FontSize = 90d;

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

        this.Invalidate();
    }

    private void textAlignCenterToolStripMenuItem_Click(object sender, EventArgs e)
    {
        _lastSelectedDemo = GetMenuText(sender);
        _onPaintAction    = static cr =>
        {
            const string Text = "cairo";

            cr.SelectFontFace("Microsoft Sans Serif", FontSlant.Normal, FontWeight.Normal);
            cr.FontSize = 52;

            cr.TextExtents(Text, out TextExtents extents);

            double x = 128.0 - (extents.Width / 2 + extents.XBearing);
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

        this.Invalidate();
    }

    private void textExtentsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        _lastSelectedDemo = GetMenuText(sender);
        _onPaintAction    = static cr =>
        {
            const string Text = "cairo";

            cr.SelectFontFace("Microsoft Sans Serif", FontSlant.Normal, FontWeight.Normal);
            cr.FontSize = 100;

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

        this.Invalidate();
    }

    private void glyphsToolStripMenuItem1_Click(object sender, EventArgs e)
    {
        _lastSelectedDemo = GetMenuText(sender);
        _onPaintAction    = static cr =>
        {
            const string Text = "cairo";

            cr.SelectFontFace("Microsoft Sans Serif", FontSlant.Normal, FontWeight.Normal);
            cr.FontSize = 100;

            const double X = 25.0;
            const double Y = 150.0;

            cr.MoveTo(X, Y);

            cr.ShowTextGlyphs(Text);
        };

        this.Invalidate();
    }

    private void glyphsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        _lastSelectedDemo = GetMenuText(sender);
        _onPaintAction    = static cr =>
        {
            const string Text = "cairo";

            cr.SelectFontFace("Microsoft Sans Serif", FontSlant.Normal, FontWeight.Normal);
            cr.FontSize = 100;

            const double X = 25.0;
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

        this.Invalidate();
    }
}

// (c) gfoidl, all rights reserved

#define USE_PIXEL_ROW_ACCESSOR

extern alias CairoSharp;

using System.Diagnostics;
using Cairo.Extensions.Pixels;
using CairoSharp::Cairo;
using CairoSharp::Cairo.Surfaces;
using CairoSharp::Cairo.Surfaces.Images;
using Gtk;
using Gtk4.Extensions;

namespace Gtk4Demo;

public sealed class PixelWindow : Window
{
    private readonly string _funcName;

#pragma warning disable CS0649 // field is never assigned to
    [Connect] private readonly Picture     _equationImage;
    [Connect] private readonly DrawingArea _drawingAreaPixelWindow;
    [Connect] private readonly Button      _pixelSaveAsPngButton;
#pragma warning restore CS0649

    private PixelWindow(string funcName) : this(funcName, Builder.NewFromResource("/at/gfoidl/cairo/gtk4/demo/demo.ui"), "pixelWindow") { }

    private PixelWindow(string funcName, Builder builder, string name)
        : base(new Gtk.Internal.WindowHandle(builder.GetPointer(name), ownsHandle: false))
    {
        _funcName = funcName;

        builder.Connect(this);
        builder.Dispose();

        Debug.Assert(_drawingAreaPixelWindow  is not null);
        Debug.Assert(_equationImage           is not null);
        Debug.Assert(_pixelSaveAsPngButton    is not null);

        _drawingAreaPixelWindow.SetDrawFunc(this.Draw);
        this.PrepareFunction();

        _pixelSaveAsPngButton.OnClicked += async (Button sender, EventArgs args)
            => await _drawingAreaPixelWindow.SaveAsPngWithFileDialog(this, _funcName);
    }

    public static void Show(string funcName)
    {
        using PixelWindow pixelWindow = new(funcName);
        pixelWindow.Show();
    }

    private void PrepareFunction()
    {
        switch (_funcName)
        {
            case "funcPeaks":
            {
                this.Title = "Peaks function";
                this.DisplayEquation("peaks.svg");
                break;
            }
            case "funcMexican":
            {
                this.Title = "Mexican hat function";
                this.DisplayEquation("mexican.svg");
                break;
            }
            default:
                throw new NotSupportedException($"Function {_funcName} is not supported");
        }
    }

    private void DisplayEquation(string fileName)
    {
        // That's why the resource got registered in Main().
        _equationImage.SetResource($"/at/gfoidl/cairo/gtk4/demo/function/{fileName}");
    }

    private void Draw(DrawingArea drawingArea, CairoContext cr, int width, int height)
    {
        // They must not be equal.
        width  = drawingArea.ContentWidth;
        height = drawingArea.ContentHeight;

        using (cr.Save())
        {
            cr.Rectangle(0, 0, width, height);
            cr.LineWidth = 1d;
            cr.Stroke();
        }

        ImageSurface imageSurface = cr.Target.MapToImage();
        try
        {
            switch (_funcName)
            {
                case "funcPeaks":
                {
                    DrawFunction<PeaksFunction>(imageSurface, width, height);
                    break;
                }
                case "funcMexican":
                {
                    DrawFunction<MexicanHatFunction>(imageSurface, width, height);
                    break;
                }
                default:
                    throw new NotSupportedException($"Function {_funcName} is not supported");
            }
        }
        finally
        {
            cr.Target.UnmapImage(imageSurface);
        }
    }

    private static void DrawFunction<TFunction>(ImageSurface imageSurface, int width, int height) where TFunction : IFunction
    {
        double[][] data = GetData(width, height);

        // Scale to [0, 255] for colormaps.
        Scale(data, 0, 255);

        using PixelAccessor pixelAccessor = imageSurface.GetPixelAccessor();

#if !USE_PIXEL_ROW_ACCESSOR
        for (int /* i */ y = 0; y < data.Length; ++y)
        {
            double[] data_y = data[y];

            for (int /* j */ x = 0; x < data_y.Length; ++x)
            {
                const double OneBy255 = 1 / 255d;

                double zForColor    = data_y[x] * OneBy255;
                pixelAccessor[x, y] = new Color(zForColor, zForColor, zForColor);
            }
        }
#else
        for (int /* i */ y = 0; y < data.Length; ++y)
        {
            double[] data_y                   = data[y];
            PixelRowAccessor pixelRowAccessor = pixelAccessor.GetRowAccessor(y);

            for (int /* j */ x = 0; x < data_y.Length; ++x)
            {
                const double OneBy255 = 1 / 255d;

                double zForColor    = data_y[x] * OneBy255;
                pixelRowAccessor[x] = new Color(zForColor, zForColor, zForColor);
            }
        }
#endif

        // Implementation is naive regarding easy readability.
        // In real production code parts could be collapsed and vectorized.
        static double[][] GetData(int width, int height)
        {
            double[][] data = new double[height][];

            double widthInv  = 1d / (width  - 1);
            double heightInv = 1d / (height - 1);

            for (int i = 0; i < data.Length; ++i)
            {
                double[] data_i = data[i] = new double[width];
                double y_i      = i * heightInv;

                for (int j = 0; j < data_i.Length; ++j)
                {
                    // x, y in [0, 1]:
                    double x = j * widthInv;
                    double y = y_i;

                    // x, y, in [-3, 3]:
                    x = 6 * x - 3;
                    y = 6 * y - 3;

                    data_i[j] = TFunction.Calculate(x, y);
                }
            }

            return data;
        }

        static void Scale(double[][] data, double start, double end)
        {
            // Linear scaling [c,d] -> [a,b]
            // x in [a,b], t in [c,d] => x = m.t + o
            // a = m.c + o
            // b = m.d + o
            // => m = (b-a)/(d-c), o = (a.d-c.b)/(d-c)

            double c = double.MaxValue;
            double d = double.MinValue;

            // Max / min
            for (int i = 0; i < data.Length; ++i)
            {
                double[] data_i = data[i];

                for (int j = 0; j < data_i.Length; ++j)
                {
                    double dij = data_i[j];

                    if (dij < c)
                    {
                        c = dij;
                    }

                    if (dij > d)
                    {
                        d = dij;
                    }
                }
            }

            double a = start;
            double b = end;

            double divisorInv = 1d / (d - c);

            double m = (b - a)         * divisorInv;
            double o = (a * d - b * c) * divisorInv;

            for (int i = 0; i < data.Length; ++i)
            {
                double[] data_i = data[i];

                for (int j = 0; j < data_i.Length; ++j)
                {
                    data_i[j] = m * data_i[j] + o;
                }
            }
        }
    }

    private interface IFunction
    {
        static abstract double Calculate(double x, double y);
    }

    private struct PeaksFunction : IFunction
    {
        public static double Calculate(double x, double y)
        {
            //  z = f(x, y) = 3 (1 - x)^2 \cdot e^{-x^2 - (y+1)^2} - 10 \left( \frac{x}{5} - x^3 - y^5 \right) \cdot e^{-x^2 - y^2} - \frac{1}{3} \cdot e^{-(x+1)^2 - y^2}

            double z =
                3 * (1 - x) * (1 - x) *
                Math.Exp(-x * x) -
                (y + 1) * (y + 1) -
                10 * (x / 5 - x * x * x - y * y * y * y * y) *
                Math.Exp(-x * x - y * y) -
                1 / 3 * Math.Exp(-(x + 1) * (x + 1) - y * y);

            return z;
        }
    }

    private struct MexicanHatFunction : IFunction
    {
        public static double Calculate(double x, double y)
        {
            // z = f(x, y) = \frac{1}{\pi \sigma^4} \left( 1 - \frac{x^2 + y^2}{\sigma^2} \right) \cdot e^{-\frac{x^2 + y^2}{2\sigma^2}}

            const double Sigma = 0.75;

            double z =
                1d / (Math.PI * Sigma * Sigma * Sigma * Sigma) *
                (1 - (x * x + y * y) / (Sigma * Sigma)) *
                Math.Exp(-(x * x + y * y) / (2 * Sigma * Sigma));

            return z;
        }
    }
}

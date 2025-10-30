// (c) gfoidl, all rights reserved

extern alias CairoSharp;

using System.Diagnostics;
using CairoSharp::Cairo;
using Gtk;
using Gtk4.Extensions;

namespace Gtk4Demo;

public sealed class PixelWindow : Window
{
    private readonly string? _funcName;

#pragma warning disable CS0649 // field is never assigned to
    [Connect] private readonly Picture     _equationImage;
    [Connect] private readonly DrawingArea _drawingAreaPixelWindow;
#pragma warning restore CS0649

    private PixelWindow(string? funcName) : this(funcName, Builder.NewFromResource("/at/gfoidl/cairo/gtk4/demo/demo.ui"), "pixelWindow") { }

    private PixelWindow(string? funcName, Builder builder, string name)
        : base(new Gtk.Internal.WindowHandle(builder.GetPointer(name), ownsHandle: false))
    {
        _funcName = funcName;

        builder.Connect(this);
        builder.Dispose();

        Debug.Assert(_drawingAreaPixelWindow  is not null);
        Debug.Assert(_equationImage           is not null);

        _drawingAreaPixelWindow.SetDrawFunc(this.Draw);

        this.PrepareFunction();
    }

    public static void Show(string? funcName)
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

        switch (_funcName)
        {
            case "funcPeaks":
            {
                this.DrawFunction<PeaksFunction>(cr, width, height);
                break;
            }
            case "funcMexican":
            {
                this.DrawFunction<MexicanHatFunction>(cr, width, height);
                break;
            }
            default:
                throw new NotSupportedException($"Function {_funcName} is not supported");
        }
    }

    private void DrawFunction<TFunction>(CairoContext cr, int width, int height) where TFunction : IFunction
    {
        double[][] data = new double[width][];
        for (int j = 0; j < data.Length; ++j)
        {
            data[j] = new double[height];
            for (int i = 0; i < data[j].Length; ++i)
            {
                // x, y in [0, 1]:
                double x = (double)j / (width - 1);
                double y = (double)i / (height - 1);

                // x, y, in [-3, 3]:
                x = 6 * x - 3;
                y = 6 * y - 3;

                data[j][i] = TFunction.Calculate(x, y);
            }
        }

        Scale(data, 0, 255);

        for (int x = 0; x < data.Length; ++x)
        {
            for (int y = 0; y < data[x].Length; ++y)
            {
                double z         = data[x][y];
                double zForColor = z / 255;
                cr.Color         = new Color(zForColor, zForColor, zForColor);

                cr.Rectangle(x, y, 1, 1);
                cr.Fill();
            }
        }

        static void Scale(double[][] data, double start, double end)
        {
            double c = double.MaxValue;
            double d = double.MinValue;

            // Max / min
            for (int j = 0; j < data.Length; ++j)
            {
                for (int i = 0; i < data[j].Length; ++i)
                {
                    if (data[j][i] < c)
                    {
                        c = data[j][i];
                    }

                    if (data[j][i] > d)
                    {
                        d = data[j][i];
                    }
                }
            }

            double a = start;
            double b = end;

            double m = (b - a)         / (d - c);
            double o = (a * d - b * c) / (d - c);

            for (int j = 0; j < data.Length; ++j)
            {
                for (int i = 0; i < data[j].Length; ++i)
                {
                    data[j][i] = m * data[j][i] + o;
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

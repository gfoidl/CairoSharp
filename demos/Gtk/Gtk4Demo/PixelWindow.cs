// (c) gfoidl, all rights reserved

#define DROPDOWN_SIMPLE_STRING_LIST
#define USE_PIXEL_ROW_ACCESSOR

extern alias CairoSharp;

using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Cairo.Extensions.Colors.ColorMaps;
using Cairo.Extensions.Pixels;
using CairoSharp::Cairo;
using CairoSharp::Cairo.Surfaces;
using CairoSharp::Cairo.Surfaces.Images;
using Gtk;
using Gtk4.Extensions;

namespace Gtk4Demo;

public sealed class PixelWindow : Window
{
    private readonly string           _funcName;
    private readonly Task<double[][]> _data;

    private ColorMap? _selectedColorMap;
    private string?   _selectedColorMapName;

#pragma warning disable CS0649 // field is never assigned to
    [Connect] private readonly DropDown    _colorMapsDropDown;
    [Connect] private readonly SpinButton  _colorMapsSpinButton;
    [Connect] private readonly CheckButton _colorMapInvertedCheckBox;
    [Connect] private readonly Button      _pixelSaveAsPngButton;
    [Connect] private readonly Picture     _equationImage;
    [Connect] private readonly Expander    _colorMapInfoExpander;
    [Connect] private readonly TextView    _colorMapInfoTextView;
    [Connect] private readonly DrawingArea _drawingAreaPixels;
#pragma warning restore CS0649

    private PixelWindow(string funcName) : this(funcName, Builder.NewFromResource("/at/gfoidl/cairo/gtk4/demo/demo.ui"), "pixelWindow") { }

    private PixelWindow(string funcName, Builder builder, string name)
        : base(new Gtk.Internal.WindowHandle(builder.GetPointer(name), ownsHandle: false))
    {
        _funcName = funcName;

        builder.Connect(this);
        builder.Dispose();

        Debug.Assert(_colorMapsDropDown        is not null);
        Debug.Assert(_colorMapsSpinButton      is not null);
        Debug.Assert(_colorMapInvertedCheckBox is not null);
        Debug.Assert(_pixelSaveAsPngButton     is not null);
        Debug.Assert(_equationImage            is not null);
        Debug.Assert(_colorMapInfoExpander     is not null);
        Debug.Assert(_colorMapInfoTextView     is not null);
        Debug.Assert(_drawingAreaPixels        is not null);

        _data = this.PrepareFunctionAsync();

        _drawingAreaPixels.SetDrawFunc(async (DrawingArea drawingArea, CairoContext cr, int width, int height)
            => await this.DrawAsync(drawingArea, cr, width, height));

        _pixelSaveAsPngButton.OnClicked += async (Button sender, EventArgs args)
            => await _drawingAreaPixels.SaveAsPngWithFileDialog(this, $"{_funcName}_{_selectedColorMapName}");

        _colorMapInvertedCheckBox.OnToggled += (CheckButton sender, EventArgs args)
            => _drawingAreaPixels.QueueDraw();

        this.SetupDropDown();
    }

    public static void Show(string funcName)
    {
        using PixelWindow pixelWindow = new(funcName);
#if DEBUG
        pixelWindow.Modal = false;
#endif
        pixelWindow.Show();
    }

    private Task<double[][]> PrepareFunctionAsync()
    {
        int width  = _drawingAreaPixels.ContentWidth;
        int height = _drawingAreaPixels.ContentHeight;

        switch (_funcName)
        {
            case "funcPeaks":
            {
                this.Title = "Peaks function";
                DisplayEquation("peaks.svg");
                return Task.Run(() => CalculateData<PeaksFunction>(width, height));
            }
            case "funcMexican":
            {
                this.Title = "Mexican hat function";
                DisplayEquation("mexican.svg");
                return Task.Run(() => CalculateData<MexicanHatFunction>(width, height));
            }
            default:
                throw new NotSupportedException($"Function {_funcName} is not supported");
        }

        void DisplayEquation(string fileName)
        {
            // That's why the resource got registered in Main().
            _equationImage.SetResource($"/at/gfoidl/cairo/gtk4/demo/function/{fileName}");
        }
    }

    private void SetupDropDown()
    {
#if DROPDOWN_SIMPLE_STRING_LIST
        string[] entries         = GetEntries();
        _colorMapsDropDown.Model = StringList.New(entries);

        static string[] GetEntries()
        {
            List<string> entries = [.. GetColorMapInfos()
                .OrderBy(ci => ci.Optimized)
                .ThenBy (ci => ci.Name!)
                .Select (ci => ci.Name!)
            ];

            // In Gtk 4.16.5 cannot prevent DropDown from preselecting the first item
            // Cf. https://gitlab.gnome.org/GNOME/gtk/-/issues/7168
            entries.Insert(0, "(none)");
            return [.. entries];
        }

        // Link the dropdown and the spinbutton (nice :-)).
        _colorMapsDropDown.BindProperty(
            DropDown.SelectedPropertyDefinition.UnmanagedName,
            _colorMapsSpinButton,
            SpinButton.ValuePropertyDefinition.UnmanagedName,
            GObject.BindingFlags.SyncCreate | GObject.BindingFlags.Bidirectional);

        _colorMapsSpinButton.SetRange(-1, entries.Length);
        _colorMapsSpinButton.SetIncrements(1, 2);

        _colorMapsDropDown.OnNotifySelected((DropDown sender, DropDownNotifySelectedArgs args) =>
        {
            Debug.Assert(args.SelectedItem is StringObject);
            StringObject stringObject = (args.SelectedItem as StringObject)!;

            if (stringObject.String is null or "(none)")
            {
                _colorMapInfoExpander.Visible = false;
                _selectedColorMap = null;
            }
            else
            {
                string colorMapName       = _selectedColorMapName = stringObject.String;
                string defaultTypeName    = $"Cairo.Extensions.Colors.ColorMaps.Default.{colorMapName}ColorMap";
                string optimizedTypeName  = $"Cairo.Extensions.Colors.ColorMaps.Optimized.{colorMapName}ColorMap";
                Assembly colorMapAssembly = typeof(ColorMap).Assembly;

                Type? colorMapType = colorMapAssembly.GetType(defaultTypeName) ?? colorMapAssembly.GetType(optimizedTypeName);
                Debug.Assert(colorMapType is not null);

                _selectedColorMap = Activator.CreateInstance(colorMapType) as ColorMap;
                Debug.Assert(_selectedColorMap is not null);

                // GTK adds a reference to the buffer once assigned to the TextView.
                using TextBuffer textBuffer = TextBuffer.New(table: null);
                textBuffer.SetText(_selectedColorMap.Description, -1);

                _colorMapInfoTextView.SetBuffer(textBuffer);
                _colorMapInfoExpander.Visible = true;
            }

            _drawingAreaPixels.QueueDraw();
        });
#endif
    }

    private async ValueTask DrawAsync(DrawingArea drawingArea, CairoContext cr, int width, int height)
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
            await this.DrawFunctionAsync(imageSurface);

#if DEBUG
            imageSurface.WriteToPng($"{_funcName}_{_selectedColorMapName}.png");
#endif
        }
        finally
        {
            cr.Target.UnmapImage(imageSurface);
        }
    }

    private async ValueTask DrawFunctionAsync(ImageSurface imageSurface)
    {
        double[][] data                   = await _data;
        using PixelAccessor pixelAccessor = imageSurface.GetPixelAccessor();

        ColorMap? colorMap   = _selectedColorMap;
        bool inverseColorMap = _colorMapInvertedCheckBox.Active;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        Color GetColor(double zForColor)
        {
            if (colorMap is not null)
            {
                return !inverseColorMap
                    ? colorMap.GetColor(zForColor)
                    : colorMap.GetColorInverted(zForColor);
            }
            else
            {
                return new Color(zForColor, zForColor, zForColor);
            }
        }

#if !USE_PIXEL_ROW_ACCESSOR
        for (int /* i */ y = 0; y < data.Length; ++y)
        {
            double[] data_y = data[y];

            for (int /* j */ x = 0; x < data_y.Length; ++x)
            {
                double zForColor    = data_y[x];
                pixelAccessor[x, y] = GetColor(zForColor);
            }
        }
#else
        for (int /* i */ y = 0; y < data.Length; ++y)
        {
            double[] data_y                   = data[y];
            PixelRowAccessor pixelRowAccessor = pixelAccessor.GetRowAccessor(y);

            for (int /* j */ x = 0; x < data_y.Length; ++x)
            {
                double zForColor    = data_y[x];
                pixelRowAccessor[x] = GetColor(zForColor);
            }
        }
#endif
    }

    // Implementation is naive regarding easy readability.
    // In real production code parts could be collapsed and vectorized.
    private static double[][] CalculateData<TFunction>(int width, int height) where TFunction : IFunction
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

        // [0,1] for color maps
        Scale(data, 0, 1);

        return data;

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

    private static IEnumerable<ColorMapInfo> GetColorMapInfos()
    {
        Assembly assembly = typeof(ColorMap).Assembly;

        foreach (Type type in assembly.GetTypes())
        {
            if (type.IsClass)
            {
                switch (type.Namespace)
                {
                    case "Cairo.Extensions.Colors.ColorMaps.Default":
                    {
                        yield return new ColorMapInfo(type.Name.Replace("ColorMap", null), Optimized: false, type);
                        break;
                    }
                    case "Cairo.Extensions.Colors.ColorMaps.Optimized":
                    {
                        yield return new ColorMapInfo(type.Name.Replace("ColorMap", null), Optimized: true, type);
                        break;
                    }
                }
            }
        }
    }

    private record ColorMapInfo(string Name, bool Optimized, Type Type);
}

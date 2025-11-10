// (c) gfoidl, all rights reserved

//#define DROPDOWN_SIMPLE_STRING_LIST
//#define DROPDOWN_USE_SEPARATOR        // With the spin button this can't work nicely, otherwise of course
#define DROPDOWN_ENABLE_SEARCH
#define DROPDOWN_BIND_TO_SPIN_VIA_UI
//#define DROPDOWN_ITEM_SET_GET_DATA_HELPER
#define USE_PIXEL_ROW_ACCESSOR

// Doesn't work for templates at the moment with Gir.Core. For BuilderListItemFactory there's an example
// in https://github.com/gircore/gir.core/blob/f15dc086bd70c2e0878ba686f750128b0eda00b7/src/Samples/Gtk-4.0/ListView/TemplateListViewWindow.cs#L20,
// but the property expression doesn't work with our managed objects. Something like https://developer.gnome.org/documentation/tutorials/widget-templates.html
// for Vala would be really cool to have.
// Here we use it with multiple builders (one for each instance).
#define DROPDOWN_ITEM_VIA_BUILDER

extern alias CairoSharp;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Cairo.Extensions.Colors;
using Cairo.Extensions.Colors.ColorMaps;
using Cairo.Extensions.Pixels;
using CairoSharp::Cairo;
using CairoSharp::Cairo.Surfaces;
using CairoSharp::Cairo.Surfaces.Images;
using Gtk;
using Gtk4.Extensions;
using static Gtk4.Extensions.Gtk4Constants;

namespace Gtk4Demo;

public sealed partial class PixelWindow : Window
{
    private readonly string           _funcName;
    private readonly Task<double[][]> _data;

    private ColorMap?      _selectedColorMap;
    private string?        _selectedColorMapName;
    private GrayScaleMode? _grayScaleMode;

    private Gio.SimpleAction _invertColorMapMenuAction;
    private Gio.SimpleAction _grayscaleMenuAction;

#pragma warning disable CS0649 // field is never assigned to
    [Connect] private readonly DropDown    _colorMapsDropDown;
    [Connect] private readonly SpinButton  _colorMapsSpinButton;
    [Connect] private readonly CheckButton _colorMapInvertedCheckButton;
    [Connect] private readonly Button      _pixelSaveAsPngButton;
    [Connect] private readonly CheckButton _grayscaleCheckButton;
    [Connect] private readonly DropDown    _grayscaleModeDropDown;
    [Connect] private readonly Picture     _equationImage;
    [Connect] private readonly Expander    _colorMapInfoExpander;
    [Connect] private readonly TextView    _colorMapInfoTextView;
    [Connect] private readonly DrawingArea _drawingAreaPixels;
    [Connect] private readonly PopoverMenu _drawingAreaPixelsPopoverMenu;
#pragma warning restore CS0649

    private PixelWindow(string funcName, Builder builder)
        : base(new Gtk.Internal.WindowHandle(builder.GetPointer("pixelWindow"), ownsHandle: false))
    {
        _funcName = funcName;

        builder.Connect(this);

        Debug.Assert(_colorMapsDropDown            is not null);
        Debug.Assert(_colorMapsSpinButton          is not null);
        Debug.Assert(_colorMapInvertedCheckButton  is not null);
        Debug.Assert(_pixelSaveAsPngButton         is not null);
        Debug.Assert(_grayscaleCheckButton         is not null);
        Debug.Assert(_grayscaleModeDropDown        is not null);
        Debug.Assert(_equationImage                is not null);
        Debug.Assert(_colorMapInfoExpander         is not null);
        Debug.Assert(_colorMapInfoTextView         is not null);
        Debug.Assert(_drawingAreaPixels            is not null);
        Debug.Assert(_drawingAreaPixelsPopoverMenu is not null);

        _data = this.PrepareFunctionAsync();

        _drawingAreaPixels.SetDrawFunc(async (DrawingArea drawingArea, CairoContext cr, int width, int height)
            => await this.DrawAsync(drawingArea, cr, width, height));

        _pixelSaveAsPngButton.OnClicked += async (Button sender, EventArgs args)
            => await _drawingAreaPixels.SaveAsPngWithFileDialog(this, this.GetPngFileName());

        // These two check buttons could also have an action (e.g. like the _invertColorMapMenuAction,
        // so this signal handler isn't needed and can be omitted.
        _colorMapInvertedCheckButton.OnToggled += (CheckButton sender, EventArgs args)
            => CheckButtonToggled(sender, _invertColorMapMenuAction);

        _grayscaleCheckButton.OnToggled += (CheckButton sender, EventArgs args)
            => CheckButtonToggled(sender, _grayscaleMenuAction);

        void CheckButtonToggled(CheckButton sender, Gio.Action? action)
        {
            Debug.Assert(action is not null);
            action.ChangeState(GLib.Variant.NewBoolean(sender.Active));

            _drawingAreaPixels.QueueDraw();
        }

        this.SetupColorMapDropDown();
        this.SetupGrayscaleDropDown();
        this.DrawingAreaAddContextMenu();
        this.AddShortcuts();
    }

    [MemberNotNull(nameof(_invertColorMapMenuAction), nameof(_grayscaleMenuAction))]
    private void DrawingAreaAddContextMenu()
    {
        GestureClick clickGesture = GestureClick.New();
        clickGesture.Button       = GdkButtonSecondary;
        clickGesture.OnPressed   += (GestureClick gesture, GestureClick.PressedSignalArgs eventArgs) =>
        {
            Debug.Assert(gesture.GetCurrentButton() == GdkButtonSecondary);

            _drawingAreaPixelsPopoverMenu.SetPointingTo(new Gdk.Rectangle()
            {
                X      = (int)eventArgs.X,
                Y      = (int)eventArgs.Y,
                Width  = 1,
                Height = 1
            });

            _drawingAreaPixelsPopoverMenu.Popup();
        };
        _drawingAreaPixels.AddController(clickGesture);
        // Not needed, as in the ui-file it's defined as child.
        //_drawingAreaPixelsPopoverMenu.SetParent(_drawingAreaPixels);

        Gio.SimpleActionGroup actionGroup = Gio.SimpleActionGroup.New();
        _invertColorMapMenuAction = actionGroup.AddAction("colorMapInvert"   , _colorMapInvertedCheckButton.Active, () => _colorMapInvertedCheckButton.Active = !_colorMapInvertedCheckButton.Active);
        _grayscaleMenuAction      = actionGroup.AddAction("colorMapGrayscale", _grayscaleCheckButton       .Active, () => _grayscaleCheckButton.Active        = !_grayscaleCheckButton       .Active);
        this.InsertActionGroup("winPix", actionGroup);

        actionGroup.AddAction("grayScaleLightness"           , () => _grayscaleModeDropDown.Selected = 0);
        actionGroup.AddAction("grayScaleAverage"             , () => _grayscaleModeDropDown.Selected = 1);
        actionGroup.AddAction("grayScaleLuminosity"          , () => _grayscaleModeDropDown.Selected = 2);
        actionGroup.AddAction("grayScaleCieLab"              , () => _grayscaleModeDropDown.Selected = 3);
        actionGroup.AddAction("grayScaleGammaExpandedAverage", () => _grayscaleModeDropDown.Selected = 4);
    }

    private void AddShortcuts()
    {
        ShortcutController shortcutController = ShortcutController.New();
        this.AddController(shortcutController);

        shortcutController.AddShortcut(Shortcut.New(
            ShortcutTrigger.ParseString("<Ctrl>i"),
            // See https://docs.gtk.org/gtk4/ctor.ShortcutAction.parse_string.html
            ShortcutAction.ParseString("action(winPix.colorMapInvert)")));

        shortcutController.AddShortcut(Shortcut.New(
            ShortcutTrigger.ParseString("<Ctrl>g"),
            ShortcutAction.ParseString("action(winPix.colorMapGrayscale)")));
    }

    public static void Show(string funcName, Builder builder)
    {
        using PixelWindow pixelWindow = new(funcName, builder);
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

    private void SetupColorMapDropDown()
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

        LinkDropDownAndSpinButton(entries.Length);

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

                DisplayDescriptionOfSelectedColorMap();
            }

            _drawingAreaPixels.QueueDraw();
        });
#else
        ColorMapInfo[] entries = GetEntries();

        static ColorMapInfo[] GetEntries()
        {
            List<ColorMapInfo> entries = [..GetColorMapInfos()
                .OrderBy(ci => ci.Optimized)
                .ThenBy(ci => ci.Name)
            ];

            // In Gtk 4.16.5 cannot prevent DropDown from preselecting the first item
            // Cf. https://gitlab.gnome.org/GNOME/gtk/-/issues/7168
            entries.Insert(0, new ColorMapInfo("(none)", default, default));

#if DROPDOWN_USE_SEPARATOR
            // Insert a separator
            int idx = entries.FindIndex(ci => ci.Optimized);
            Debug.Assert(idx >= 0);
            entries.Insert(idx, new ColorMapInfo("(separator)", default, default));
            entries.Insert(1  , new ColorMapInfo("(separator)", default, default));
#endif

            return [.. entries];
        }

#if DROPDOWN_ENABLE_SEARCH
        _colorMapsDropDown.EnableSearch = true;

        // The expression must be set before the factory, otherwise the factory / factories will
        // be overwritten, and only the string is shown.

        // PropertyExpression can't be used here, as ColorMapInfo has not unmanaged property.
        // Gtk-CRITICAL **: Type `Gtk4DemoPixelWindowColorMapInfo` does not have a property named `Name`
        //_colorMapsDropDown.Expression = Expression.CreateForProperty(ColorMapInfo.GetGType(), nameof(ColorMapInfo.Name));

        // This throws 'Type Gtk.CClosureExpression is not supported as a value type', but why?
        //_colorMapsDropDown.Expression = Expression.CreateForClosure(static (ColorMapInfo colorMapInfo) => colorMapInfo.Name);

        // That's the discrepancy with the native objects...the managed objects can't be recreated easily.
        // Thus a mapping is used.
        _colorMapsDropDown.SetExpression<ColorMapInfo>(static (IntPtr nativeObjHandle) =>
        {
            if (ColorMapInfo.s_handleMap.TryGetValue(nativeObjHandle, out ColorMapInfo? colorMapInfo))
            {
                return colorMapInfo.Name;
            }

            throw new InvalidOperationException();
        });

        // In https://discourse.gnome.org/t/example-of-gtk-dropdown-with-search-enabled-without-gtk-expression/12748
        // there's another way on how to search w/o expressions.
#endif

        // Factory must be set first, otherwise GTK doesn't know how to handle the model.
        _colorMapsDropDown.Factory = CreateFactory();
        _colorMapsDropDown.Model   = CreateModel(entries);

        static Gio.ListModel CreateModel(ColorMapInfo[] entries)
        {
            Gio.ListStore listStore = Gio.ListStore.New(ColorMapInfo.GetGType());

            foreach (ColorMapInfo entry in entries)
            {
                listStore.Append(entry);
            }

            return listStore;
        }

        static ListItemFactory CreateFactory()
        {
            SignalListItemFactory factory = SignalListItemFactory.New();

            // See https://docs.gtk.org/gtk4/class.SignalListItemFactory.html for description of the signals.

            factory.OnSetup += (SignalListItemFactory factory, SignalListItemFactory.SetupSignalArgs args) =>
            {
                Debug.Assert(args.Object is ListItem);

                ListItem listItem = (args.Object as ListItem)!;
                listItem.Child    = new ColorMapDropDownItem(listItem);
            };

            factory.OnBind += (SignalListItemFactory factory, SignalListItemFactory.BindSignalArgs args) =>
            {
                Debug.Assert(args.Object is ListItem);
                ListItem listItem = (args.Object as ListItem)!;

                ColorMapInfo? colorMapInfo = listItem.GetItem() as ColorMapInfo;
                Debug.Assert(colorMapInfo is not null);

                (Image image, Label label) = ColorMapDropDownItem.GetChildren(listItem);

                label.SetText(colorMapInfo.Name);

                if (colorMapInfo.Name == "(none)")
                {
                    image.Visible = false;
                }
                else if (colorMapInfo.Name == "(separator)")
                {
                    listItem.Child      = Separator.New(Orientation.Horizontal);
                    listItem.Selectable = false;
                }
                else
                {
                    Debug.Assert(colorMapInfo.Type is not null);

                    // Default GTK icons (found via IconLibrary).
                    // They're OS dependent icons, so they're only available on Linux.
                    if (OperatingSystem.IsLinux())
                    {
                        string iconName = colorMapInfo.Optimized
                            ? "favorite-new"
                            : "edit-tag";

                        image.SetFromIconName(iconName);
                    }
                    else
                    {
                        // They're the same as on linux, just renamed in the gresource by me.
                        string iconName = colorMapInfo.Optimized
                            ? "colormap_optimized.svg"
                            : "colormap_default.svg";

                        image.SetFromResource($"/at/gfoidl/cairo/gtk4/demo/icons/{iconName}");
                    }
                }
            };

            return factory;
        }

        LinkDropDownAndSpinButton(entries.Length);

        _colorMapsDropDown.OnNotifySelected((DropDown sender, DropDownNotifySelectedArgs args) =>
        {
            Debug.Assert(args.SelectedItem is ColorMapInfo);
            ColorMapInfo colorMapInfo = (args.SelectedItem as ColorMapInfo)!;

            if (colorMapInfo.Name == "(none)")
            {
                _colorMapInfoExpander.Visible = false;
                _selectedColorMap             = null;
            }
            else if (colorMapInfo.Name == "(separator)")
            {
                return;
            }
            else
            {
                Debug.Assert(colorMapInfo.Type is not null);
                _selectedColorMap     = colorMapInfo.ColorMap;
                _selectedColorMapName = colorMapInfo.Name;

                DisplayDescriptionOfSelectedColorMap();
            }

            _drawingAreaPixels.QueueDraw();
        });
#endif

        void LinkDropDownAndSpinButton(int entriesCount)
        {
            // Link the dropdown and the spinbutton (nice :-)).
#if !DROPDOWN_BIND_TO_SPIN_VIA_UI
            _colorMapsDropDown.BindProperty(
                DropDown.SelectedPropertyDefinition.UnmanagedName,
                _colorMapsSpinButton,
                SpinButton.ValuePropertyDefinition.UnmanagedName,
                GObject.BindingFlags.SyncCreate | GObject.BindingFlags.Bidirectional);
#endif
            _colorMapsSpinButton.SetRange(-1, entriesCount + 1);

            // Is set via the ui-file (`GtkAdjustment` in Cambalache).
            //_colorMapsSpinButton.SetIncrements(1, 2);
        }

        void DisplayDescriptionOfSelectedColorMap()
        {
            Debug.Assert(_selectedColorMap is not null);

            TextBuffer? textBuffer = _colorMapInfoTextView.GetBuffer();

            if (textBuffer is null)
            {
                textBuffer = TextBuffer.New(table: null);

                // GTK adds a reference to the buffer once assigned to the TextView.
                _colorMapInfoTextView.SetBuffer(textBuffer);
            }

            textBuffer.SetText(_selectedColorMap.Description, -1);
            _colorMapInfoExpander.Visible = true;
        }
    }

    private void SetupGrayscaleDropDown()
    {
        string[] entries             = Enum.GetNames(typeof(GrayScaleMode));
        _grayscaleModeDropDown.Model = StringList.New(entries);
        _grayscaleModeDropDown.OnNotifySelected((DropDown sender, DropDownNotifySelectedArgs args) =>
        {
            Debug.Assert(args.SelectedItem is StringObject);
            StringObject stringObject = (args.SelectedItem as StringObject)!;

            if (stringObject.String is not null)
            {
                _grayScaleMode = Enum.Parse<GrayScaleMode>(stringObject.String);

                _drawingAreaPixels.QueueDraw();
            }
        });
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
            Directory.CreateDirectory("images");
            imageSurface.WriteToPng($"images/{this.GetPngFileName()}.png");
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

        ColorMap? colorMap          = _selectedColorMap;
        bool inverseColorMap        = _colorMapInvertedCheckButton.Active;
        bool grayScale              = _grayscaleCheckButton.Active;
        GrayScaleMode grayScaleMode = _grayScaleMode.GetValueOrDefault();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        Color GetColor(double zForColor)
        {
            if (colorMap is not null)
            {
                Color color = !inverseColorMap
                    ? colorMap.GetColor(zForColor)
                    : colorMap.GetColorInverted(zForColor);

                if (grayScale)
                {
                    color = color.ToGrayScale(grayScaleMode);
                }

                return color;
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

    private string GetPngFileName()
    {
        ReadOnlySpan<char> funcName = _funcName.AsSpan("func".Length);
        StringBuilder sb            = new();

        sb.Append($"{funcName}");

        if (_selectedColorMapName is not null)
        {
            sb.Append($"_{_selectedColorMapName}");
        }

        if (_colorMapInvertedCheckButton.Active)
        {
            sb.Append("_inverted");
        }

        if (_grayscaleCheckButton.Active)
        {
            sb.Append($"_grayscale_{_grayScaleMode}");
        }

        return sb.ToString();
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
                        yield return new ColorMapInfo(type.Name.Replace("ColorMap", null), optimized: false, type);
                        break;
                    }
                    case "Cairo.Extensions.Colors.ColorMaps.Optimized":
                    {
                        yield return new ColorMapInfo(type.Name.Replace("ColorMap", null), optimized: true, type);
                        break;
                    }
                }
            }
        }
    }

    // https://gircore.github.io/docs/faq.html#how-to-create-subclasses-of-a-gobject-based-class
    [GObject.Subclass<GObject.Object>]
    [DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
    private sealed partial class ColorMapInfo
    {
        public string Name        { get; } = "";
        public bool Optimized     { get; }
        public Type? Type         { get; }
        public ColorMap? ColorMap { get; }

        public ColorMapInfo(string name, bool optimized, Type? type) : this()
        {
            this.Name      = name;
            this.Optimized = optimized;
            this.Type      = type;

            if (type is not null)
            {
                this.ColorMap = Activator.CreateInstance(type) as ColorMap;
            }

#if DROPDOWN_ENABLE_SEARCH
            s_handleMap.TryAdd(this.Handle.DangerousGetHandle(), this);
#endif
        }

        private string GetDebuggerDisplay() => $"{this.Name} (optimized: {this.Optimized})";

#if DROPDOWN_ENABLE_SEARCH
        internal static readonly Dictionary<nint, ColorMapInfo> s_handleMap = [];
#endif
    }

    private sealed class ColorMapDropDownItem : Box
    {
#if !DROPDOWN_ITEM_VIA_BUILDER
        public ColorMapDropDownItem(ListItem listItem)
        {
            this.SetOrientation(Orientation.Horizontal);
            this.SetSpacing(10);

            // These could also be stored in fields for easier access.
            Image image = Image.New();
            Label label = Label.New(str: null);

            label.Xalign = 0f;

            this.Append(image);
            this.Append(label);

#if DROPDOWN_ITEM_SET_GET_DATA_HELPER
            listItem.SetData("image", image.Handle.DangerousGetHandle());
            listItem.SetData("label", label.Handle.DangerousGetHandle());
#endif
        }

        public static (Image image, Label label) GetChildren(ListItem listItem)
        {
#if DROPDOWN_ITEM_SET_GET_DATA_HELPER
            // Are set by SetData in OnSetup above.
            using Image image = new(new Gtk.Internal.ImageHandle(listItem.GetData("image"), ownsHandle: false));
            using Label label = new(new Gtk.Internal.LabelHandle(listItem.GetData("label"), ownsHandle: false));
#else
            Box? box = listItem.Child as Box;
            Debug.Assert(box is not null);

            Image? image = box.GetFirstChild() as Image;
            Debug.Assert(image is not null);

            Label? label = image.GetNextSibling() as Label;
            Debug.Assert(label is not null);
#endif
            return(image, label);
        }
    }
#else
#pragma warning disable CS0649 // field is never assigned to
        [Connect] private readonly Image _image;
        [Connect] private readonly Label _label;
#pragma warning restore CS0649

        public ColorMapDropDownItem(ListItem listItem) : this(Builder.NewFromResource("/at/gfoidl/cairo/gtk4/demo/ui/colormapdropdownitem.ui"), "ColorMapDropDownItem", listItem) { }

        private ColorMapDropDownItem(Builder builder, string name, ListItem listItem)
            : base(new Gtk.Internal.BoxHandle(builder.GetPointer(name), ownsHandle: false))
        {
            builder.Connect(this);
            builder.Dispose();

            Debug.Assert(_image is not null);
            Debug.Assert(_label is not null);

#if DROPDOWN_ITEM_SET_GET_DATA_HELPER
            listItem.SetData("image", _image.Handle.DangerousGetHandle());
            listItem.SetData("label", _label.Handle.DangerousGetHandle());
#endif
        }

        public static (Image image, Label label) GetChildren(ListItem listItem)
        {
#if DROPDOWN_ITEM_SET_GET_DATA_HELPER
            // Are set by SetData in OnSetup above.
            using Image image = new(new Gtk.Internal.ImageHandle(listItem.GetData("image"), ownsHandle: false));
            using Label label = new(new Gtk.Internal.LabelHandle(listItem.GetData("label"), ownsHandle: false));
#else
            ColorMapDropDownItem? cmddi = listItem.Child as ColorMapDropDownItem;
            Debug.Assert(cmddi is not null);

            Image image = cmddi._image;
            Label label = cmddi._label;
#endif

            return (image, label);
        }
    }
#endif
}

// (c) gfoidl, all rights reserved

using System.Windows;
using System.Windows.Controls;
using Cairo;
using Cairo.Surfaces;
using Cairo.Surfaces.Images;

namespace WpfDemo;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void Image_Loaded(object sender, RoutedEventArgs e)
    {
        Image image = (sender as Image)!;

        using (ImageSurface surface = new(Format.Argb32, (int)image.Width, (int)image.Height))
        using (CairoContext context = new(surface))
        {
            context.Rectangle(10, 10, 100, 100);
            context.Color = new Color(1, 0.5, 0.6);
            context.Fill();

            context.MoveTo(140.0, 110.0);
            context.SetFontSize(32);
            context.SetSourceColor(new Color(0.0, 0.0, 0.8, 1.0));
            context.ShowText("Hello Cairo!");

            surface.Flush();

            image.Source = new RgbaBitmapSource(surface.Data, surface.Width);
        }
    }
}

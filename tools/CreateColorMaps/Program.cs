// (c) gfoidl, all rights reserved

using System.Globalization;
using CreateColorMaps;

Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

IEnumerable<ColorMapGenerator> generators = GetDefaultCsvColorMapGenerators()
    .Concat(GetDefaultCodeColorMapGenerators())
    .Concat(GetDefaultBitmapColorMapGenerators())
    .Concat(GetOptimizedCsvColorMapGenerators())
    .Concat(GetOptimizedCodeColorMapGenerators());

foreach (ColorMapGenerator generator in generators)
{
    string outputFile = generator.Run();
    Console.WriteLine($"Color map for {generator.Name} created in {outputFile}");
}

Console.WriteLine("bye.");
//-----------------------------------------------------------------------------
static IEnumerable<ColorMapGenerator> GetDefaultCsvColorMapGenerators()
{
    yield return new DefaultCsvColorMapGenerator("data/default/bone.csv"  , "Bone"  , "Bone is a grayscale colormap with a higher value for the blue component. This colormap is useful for adding an 'electronic' look to grayscale images.");
    yield return new DefaultCsvColorMapGenerator("data/default/cool.csv"  , "Cool"  , "Cool consists of colors that are shades of cyan and magenta. It varies smoothly from cyan to magenta.");
    yield return new DefaultCsvColorMapGenerator("data/default/copper.csv", "Copper", "Copper varies smoothly from black to bright copper.");
    yield return new DefaultCsvColorMapGenerator("data/default/hot.csv"   , "Hot"   , "Hot varies smoothly from black through shades of red, orange, and yellow, to white.");
    yield return new DefaultCsvColorMapGenerator("data/default/hsv.csv"   , "Hsv"   , "HSV varies the hue component of the hue-saturation-value color model. The colors begin with red, pass through yellow, green, cyan, blue, magenta, and return to red. The colormap is particularly appropriate for displaying periodic functions.");
    yield return new DefaultCsvColorMapGenerator("data/default/jet.csv"   , "Jet"   , "Jet ranges from blue to red, and passes through the colors cyan, yellow, and orange. It is a variation of the HSV colormap. The jet colormap is associated with an astrophysical fluid jet simulation from the National Center for Supercomputer Applications.");
    yield return new DefaultCsvColorMapGenerator("data/default/pink.csv"  , "Pink"  , "Pink contains pastel shades of pink. The pink colormap provides sepia tone colorization of grayscale photographs.");
    yield return new DefaultCsvColorMapGenerator("data/default/spring.csv", "Spring", "Spring consists of colors that are shades of magenta and yellow.");
    yield return new DefaultCsvColorMapGenerator("data/default/summer.csv", "Summer", "Summer consists of colors that are shades of green and yellow.");
    yield return new DefaultCsvColorMapGenerator("data/default/winter.csv", "Winter", "Winter consists of colors that are shades of blue and green.");
}
//-----------------------------------------------------------------------------
static IEnumerable<ColorMapGenerator> GetDefaultCodeColorMapGenerators()
{
    yield return new DefaultCodeColorMapGenerator("Autumn" , "Autumn varies smoothly from red, through orange, to yellow.");
    yield return new DefaultCodeColorMapGenerator("Gray"   , "Gray represents gray-scales. Can be used invert the grayscales.");
    yield return new DefaultCodeColorMapGenerator("Rainbow", "Rainbow consists of color red, green, blue and varies them like a rainbow.");
    yield return new DefaultCodeColorMapGenerator("Sine"   , "Sine varies the color components sine-wavy.");
}
//-----------------------------------------------------------------------------
static IEnumerable<ColorMapGenerator> GetDefaultBitmapColorMapGenerators()
{
    yield return new DefaultBitmapColorMapGenerator("data/default/heat.bmp", "Heat", "Heat varies the color from blue throught skyblue, green, yellow, red to white.");
}
//-----------------------------------------------------------------------------
static IEnumerable<ColorMapGenerator> GetOptimizedCsvColorMapGenerators()
{
    yield return new OptimizedCsvColorMapGenerator("data/optimized/fast-table-float-0512.csv"              , "Fast"             , "Fast is a diverging (double-ended) color map with a smooth transition in the middle to prevent artifacts at the midpoint. This colormap is designed to be used on 3D surfaces, so it avoids getting too dark at the ends (although it does get somewhat dark to extend discriminability).");
    yield return new OptimizedCsvColorMapGenerator("data/optimized/smooth-cool-warm-table-float-0512.csv"  , "SmoothCoolWarm"   , "SmoothCoolWarm is a diverging (double-ended) color map with a smooth transition in the middle to prevent artifacts at the midpoint. Although not isoluminant, this color map avoids dark colors to allow shading cues throughout.");
    yield return new OptimizedCsvColorMapGenerator("data/optimized/bent-cool-warm-table-float-0512.csv"    , "BentCoolWarm"     , "BentCoolWarm is a similar color map to SmoothCoolWarm except that the luminance is interpolated linearly with a sharp bend in the middle. This makes for less washed out colors in the middle, but also creates an artifact at the midpoint.");
    yield return new OptimizedCsvColorMapGenerator("data/optimized/viridis-table-float-0512.csv"           , "Viridis"          , "Viridis is a perceptually uniform color map with monotonically increasing luminance and a pleasant smooth arc through blue, green, and yellow hues. Although none of the colors in viridis reach black, the bottom end of the scale does get dark, so map might need to be shortened for some 3D applications. Although viridis is very perceptually smooth and monotonically increasing, it does not have as much discrimination as other color maps.");
    yield return new OptimizedCsvColorMapGenerator("data/optimized/plasma-table-float-0512.csv"            , "Plasma"           , "Plasma is a perceptually uniform color map with monotonically increasing luminance and a pleasant smooth arc through blue, purple, and yellow hues. Although none of the colors in plasma reach black, the bottom end of the scale does get dark, so map might need to be shortened for some 3D applications. Inferno is a similar color map that extends the lower range all the way to black for 2D heat maps.");
    yield return new OptimizedCsvColorMapGenerator("data/optimized/black-body-table-float-0512.csv"        , "BlackBody"        , "The black body color map based on the colors of black body radiation. Although the colors are inspired by the wavelengths of light from black body radiation, the actual colors used are designed to be perceptually uniform. Colors of the desired brightness and hue are chosen, and then the colors are adjusted such that the luminance is perceptually linear (according to the CIELAB color space).");
    yield return new OptimizedCsvColorMapGenerator("data/optimized/inferno-table-float-0512.csv"           , "Inferno"          , "Inferno is a perceptually uniform color map with monotonically increasing luminance. It is similar to black body but also adds some purple hues for a more appealing display.");
    yield return new OptimizedCsvColorMapGenerator("data/optimized/kindlmann-table-float-0512.csv"         , "Kindlmann"        , "Kindlmann is basically the rainbow color map with the luminance adjusted such that it monotonically changes, making it much more perceptually viable.");
    yield return new OptimizedCsvColorMapGenerator("data/optimized/extended-kindlmann-table-float-0512.csv", "ExtendedKindlmann", "The extended Kindlmann color map uses the colors from Kindlmann but also adds more hues by doing a more than 360 degree loop around the hues. This works because the endpoints have low saturation and very different brightness.");
}
//-----------------------------------------------------------------------------
static IEnumerable<ColorMapGenerator> GetOptimizedCodeColorMapGenerators()
{
    yield return new OptimizedCodeColorMapGenerator("Turbo", "Turbo, a new colormap that has the desirable properties of Jet while also addressing some of its shortcomings, such as false detail, banding and color blindness ambiguity. Turbo was hand-crafted and fine-tuned to be effective for a variety of visualization tasks.");
}

## Gallery of color maps

The gallery shows a modified _peaks function_ colored by the [provided color maps](../../../source/CairoSharp.Extensions/Colors/ColorMaps/ReadMe.md), and in addition
* the inverted color map
* [grayscale variants](../../../source/CairoSharp.Extensions/Colors/GrayScaleMode.cs)
  * Lightness
  * Average
  * Luminosity
  * CieLab
  * GammaExpandedAverage

$$
z = f(x, y) = 3 (1 - x)^2 \cdot e^{-x^2} - (y+1)^2 - 10 \left( \frac{x}{5} - x^3 - y^5 \right) \cdot e^{-x^2 - y^2} - \frac{1}{3} \cdot e^{-(x+1)^2 - y^2}
$$

![](./peaks_function.png)

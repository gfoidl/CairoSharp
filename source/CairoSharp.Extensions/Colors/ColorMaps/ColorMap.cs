// (c) gfoidl, all rights reserved

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Cairo.Extensions.Colors.ColorMaps;

/// <summary>
/// Base implementation of a color map.
/// </summary>
public abstract class ColorMap
{
    protected abstract int Entries               { get; }
    protected abstract double ScaleFactor        { get; }   // (Entries - 1) * 3, just pre-computed
    protected abstract ReadOnlySpan<double> Data { get; }

    public virtual string? Description { get; }

    /// <summary>
    /// Gets the color for <paramref name="value"/> according the underlying color map.
    /// </summary>
    /// <param name="value">
    /// The value in the interval [0,1] for which the color is chosen.
    /// <para>
    /// No validation is done whether the value is in the interval [0,1] or not.
    /// </para>
    /// </param>
    /// <returns>The <see cref="Color"/> according the color map</returns>
    public Color GetColor(double value) => GetColor<DefaultIndex>(value);

    /// <summary>
    /// Gets the color for <paramref name="value"/> according the underlying color map,
    /// which will be inverted, that is the highest entry becomes the lowest entry and
    /// vice versa.
    /// </summary>
    /// <param name="value">
    /// The value in the interval [0,1] for which the color is chosen.
    /// <para>
    /// No validation is done whether the value is in the interval [0,1] or not.
    /// </para>
    /// <returns>The <see cref="Color"/> according the color map</returns>
    public Color GetColorInverted(double value) => GetColor<InvertedIndex>(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Color GetColor<TIndex>(double value) where TIndex : IIndex
    {
        Debug.Assert(this.Entries * 3 == this.Data.Length);

        // value is [0,1], so wee need to scale to [0, entries] and keep in mind
        // that each entry consists of 3 doubles.

        int index = TIndex.GetIndex(value, this.ScaleFactor, this.Entries);

        Debug.Assert(index + 2 < this.Data.Length);

        ref double colorMap = ref MemoryMarshal.GetReference(this.Data);
        colorMap            = ref Unsafe.Add(ref colorMap, (uint)index);

        double red   = Unsafe.Add(ref colorMap, 0);
        double green = Unsafe.Add(ref colorMap, 1);
        double blue  = Unsafe.Add(ref colorMap, 2);

        return new Color(red, green, blue);
    }

    private interface IIndex
    {
        static abstract int GetIndex(double value, double scaleFactor, int entries);
    }

    private struct DefaultIndex : IIndex
    {
        public static int GetIndex(double value, double scaleFactor, int entries)
            => (int)(value * scaleFactor);
    }

    private struct InvertedIndex : IIndex
    {
        public static int GetIndex(double value, double scaleFactor, int entries)
            => (int)((1 - value) * scaleFactor);
    }
}

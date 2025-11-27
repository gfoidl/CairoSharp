// (c) gfoidl, all rights reserved

using System.Numerics.Tensors;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using Gtk4Demo.Pixels;

namespace Gtk4FunctionPlotDemo;

internal static class Calculator
{
    private const int Size       = MainWindow.Size;
    private const double SizeInv = MainWindow.SizeInv;
    //-------------------------------------------------------------------------
    public static double[][] CalculateData<TFunction>(out double funcMin, out double funcMax) where TFunction : IFunction
    {
        double[][] data = new double[Size][];
        double localMin = double.MaxValue;
        double localMax = double.MinValue;

        Parallel.For<(double min, double max)>(0, data.Length,
            () => (double.MaxValue, double.MinValue),
            (int i, ParallelLoopState _, (double Min, double Max) arg) =>
            {
                double localMin = arg.Min;
                double localMax = arg.Max;

                double[] data_i = data[i] = new double[Size];
                double y_i      = i * SizeInv;

                for (int j = 0; j < data_i.Length; ++j)
                {
                    // x, y in [0, 1]:
                    double x = j * SizeInv;
                    double y = y_i;

                    // x, y, in [-3, 3]:
                    if (Vector128.IsHardwareAccelerated)
                    {
                        Vector128<double> vec = Vector128.FusedMultiplyAdd(
                            Vector128.Create(x, y),
                            Vector128.Create(6d),
                            Vector128.Create(-3d));

                        x = vec[0];
                        y = vec[1];
                    }
                    else
                    {
                        x = 6 * x - 3;
                        y = 6 * y - 3;
                    }

                    double value = TFunction.Calculate(x, y);
                    data_i[j]    = value;

                    if (value < localMin) localMin = value;
                    if (value > localMax) localMax = value;
                }

                return (localMin, localMax);
            },
            ((double Min, double Max) arg) =>
            {
                lock (data)
                {
                    if (arg.Min < localMin) localMin = arg.Min;
                    if (arg.Max > localMax) localMax = arg.Max;
                }
            }
        );

        funcMin = localMin;
        funcMax = localMax;

        return data;
    }
    //-------------------------------------------------------------------------
    public static double[][] ScaleTo01Range(double[][] data, double funcMin, double funcMax)
    {
        double min      = funcMin;
        double max      = funcMax;
        double invScale = 1d / (max - min);

        double[][] res = new double[data.Length][];

        Parallel.For(0, res.Length, i =>
        {
            double[] data_i = data[i];
            double[] res_i  = res [i] = new double[data_i.Length];

            TensorPrimitives.Subtract(data_i, min, res_i);
            TensorPrimitives.Multiply(res_i, invScale, res_i);
        });

        return res;
    }
    //-------------------------------------------------------------------------
    public static bool TryGetCoordinatesAndFuncValue(double[][] funcData, DevicePosition devicePosition, out double x, out double y, out double z)
    {
        Unsafe.SkipInit(out x);
        Unsafe.SkipInit(out y);
        Unsafe.SkipInit(out z);

        int mouseX = double.ConvertToIntegerNative<int>(devicePosition.X);
        int mouseY = double.ConvertToIntegerNative<int>(devicePosition.Y);

        // Note: data.Length is a "never negative integer"
        if ((uint)mouseY >= funcData.Length) return false;

        double[] data_y = funcData[mouseY];

        if ((uint)mouseX >= (uint)data_y.Length) return false;

        z = data_y[mouseX];

        // Scale x, y to [-3, 3]
        if (Vector128.IsHardwareAccelerated)
        {
            Vector128<double> vec = Unsafe.BitCast<DevicePosition, Vector128<double>>(devicePosition);
            vec *= Vector128.Create(SizeInv);
            vec  = Vector128.FusedMultiplyAdd(vec, Vector128.Create(6d), Vector128.Create(-3d));

            x = vec[0];
            y = vec[1];
        }
        else
        {
            x = devicePosition.X * SizeInv;
            y = devicePosition.Y * SizeInv;

            x = 6 * x - 3;
            y = 6 * y - 3;
        }

        return true;
    }
}

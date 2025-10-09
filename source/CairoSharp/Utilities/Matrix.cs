// (c) gfoidl, all rights reserved

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;

namespace Cairo.Utilities;

/// <summary>
/// Generic matrix operations
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Matrix
{
    public double Xx; public double Yx;
    public double Xy; public double Yy;
    public double X0; public double Y0;

    /// <summary>
    /// Creates a new <see cref="Matrix"/> to be the affine transformation given by the parameters.
    /// </summary>
    /// <remarks>
    /// The transformation is given by:<br/>
    /// x_new = xx * x + xy * y + x0;<br/>
    /// y_new = yx * x + yy * y + y0;
    /// </remarks>
    public Matrix(
        double xx, double yx,
        double xy, double yy,
        double x0, double y0)
    {
        Xx = xx; Yx = yx;
        Xy = xy; Yy = yy;
        X0 = x0; Y0 = y0;
    }

    /// <summary>
    /// Creates a new <see cref="Matrix"/>, and sets it to <see cref="InitIdentity"/>.
    /// </summary>
    public Matrix() => MatrixNative.cairo_matrix_init_identity(ref this);

    /// <summary>
    /// Checks whether the <see cref="Matrix"/> is an identity matrix.
    /// </summary>
    public readonly bool IsIdentity => this == new Matrix();

    /// <summary>
    /// Sets <see cref="Matrix"/> to be the affine transformation given by the parameters.
    /// </summary>
    /// <param name="xx">xx component of the affine transformation</param>
    /// <param name="yx">yx component of the affine transformation</param>
    /// <param name="xy">xy component of the affine transformation</param>
    /// <param name="yy">yy component of the affine transformation</param>
    /// <param name="x0">X translation component of the affine transformation</param>
    /// <param name="y0">Y translation component of the affine transformation</param>
    /// <remarks>
    /// The transformation is given by:<br/>
    /// x_new = xx * x + xy * y + x0;<br/>
    /// y_new = yx * x + yy * y + y0;
    /// </remarks>
    public void Init(
       double xx, double yx,
       double xy, double yy,
       double x0, double y0)
        => MatrixNative.cairo_matrix_init(ref this, xx, yx, xy, yy, x0, y0);

    /// <summary>
    /// Modifies <see cref="Matrix"/> to be an identity transformation.
    /// </summary>
    public void InitIdentity()
    {
        // 1 0
        // 0 1
        // 0 0

        MatrixNative.cairo_matrix_init_identity(ref this);
    }

    /// <summary>
    /// Initializes <see cref="Matrix"/> to a transformation that translates by
    /// <paramref name="tx"/> and <paramref name="ty"/> in the X and Y dimensions, respectively.
    /// </summary>
    /// <param name="tx">amount to translate in the X direction</param>
    /// <param name="ty">amount to translate in the Y direction</param>
    public void InitTranslate(double tx, double ty)
    {
        //  1  0
        //  0  1
        // tx ty

        MatrixNative.cairo_matrix_init_translate(ref this, tx, ty);
    }

    /// <summary>
    /// Initializes <see cref="Matrix"/> to a transformation that scales by
    /// <paramref name="sx"/> and <paramref name="sy"/> in the X and Y dimensions, respectively.
    /// </summary>
    /// <param name="sx">scale factor in the X direction</param>
    /// <param name="sy">scale factor in the Y direction</param>
    public void InitScale(double sx, double sy)
    {
        // sx  0
        //  0 sy
        //  0  0

        MatrixNative.cairo_matrix_init_scale(ref this, sx, sy);
    }

    /// <summary>
    /// Initialized <see cref="Matrix"/> to a transformation that rotates by <paramref name="radians"/>.
    /// </summary>
    /// <param name="radians">
    /// angle of rotation, in radians. The direction of rotation is defined such that positive
    /// angles rotate in the direction from the positive X axis toward the positive Y axis.
    /// With the default axis orientation of cairo, positive angles rotate in a clockwise direction.
    /// </param>
    public void InitRotate(double radians)
    {
        /*
         * double sin, con
         * sin = Math.Sin(radians);
         * cos = Math.Cos(radians);
         * 
         *  cos sin
         * -sin cos
         *    0   0
         */

        MatrixNative.cairo_matrix_init_rotate(ref this, radians);
    }

    /// <summary>
    /// Applies a translation by <paramref name="tx"/>, <paramref name="ty"/> to the transformation
    /// in <see cref="Matrix"/>. The effect of the new transformation is to first translate the
    /// coordinates by <paramref name="tx"/> and <paramref name="ty"/>, then apply the original
    /// transformation to the coordinates.
    /// </summary>
    /// <param name="tx">amount to translate in the X direction</param>
    /// <param name="ty">amount to translate in the Y direction</param>
    public void Translate(double tx, double ty) => MatrixNative.cairo_matrix_translate(ref this, tx, ty);

    /// <summary>
    /// Applies scaling by <paramref name="sx"/>, <paramref name="sy"/> to the transformation in
    /// <see cref="Matrix"/>. The effect of the new transformation is to first scale the coordinates by
    /// <paramref name="sx"/> and <paramref name="sy"/>, then apply the original transformation to the coordinates.
    /// </summary>
    /// <param name="sx">scale factor in the X direction</param>
    /// <param name="sy">scale factor in the Y direction</param>
    public void Scale(double sx, double sy) => MatrixNative.cairo_matrix_scale(ref this, sx, sy);

    /// <summary>
    /// Applies rotation by <paramref name="radians"/> to the transformation in <see cref="Matrix"/>.
    /// The effect of the new transformation is to first rotate the coordinates by <paramref name="radians"/>,
    /// then apply the original transformation to the coordinates.
    /// </summary>
    /// <param name="radians">
    /// angle of rotation, in radians. The direction of rotation is defined such that positive angles
    /// rotate in the direction from the positive X axis toward the positive Y axis. With the default
    /// axis orientation of cairo, positive angles rotate in a clockwise direction.
    /// </param>
    public void Rotate(double radians) => MatrixNative.cairo_matrix_rotate(ref this, radians);

    /// <summary>
    /// Changes <see cref="Matrix"/> to be the inverse of its original value. Not all transformation matrices
    /// have inverses; if the matrix collapses points together (it is degenerate), then it has no
    /// inverse and this method will fail.
    /// </summary>
    /// <returns>
    /// If <see cref="Matrix"/> has an inverse, modifies <see cref="Matrix"/> to be the inverse matrix and
    /// returns <see cref="Status.Success"/>. Otherwise, returns <see cref="Status.InvalidMatrix"/>.
    /// </returns>
    public Status Invert() => MatrixNative.cairo_matrix_invert(ref this);

    /// <summary>
    /// Multiplies the affine transformations in <paramref name="a"/> and <paramref name="b"/> together and
    /// returns the result. The effect of the resulting transformation is to first apply the transformation
    /// in <paramref name="a"/> to the coordinates and then apply the transformation in <paramref name="b"/>
    /// to the coordinates.
    /// </summary>
    /// <param name="a">a <see cref="Matrix"/></param>
    /// <param name="b">a <see cref="Matrix"/></param>
    /// <returns>
    /// The multiplied <see cref="Matrix"/>
    /// </returns>
    public static Matrix Multiply(ref Matrix a, ref Matrix b)
    {
        MatrixNative.cairo_matrix_multiply(out Matrix result, ref a, ref b);
        return result;
    }

    public void Multiply(Matrix b)
    {
        // From https://www.cairographics.org/manual/cairo-cairo-matrix-t.html
        // It is allowable for result to be identical to either a or b.
        MatrixNative.cairo_matrix_multiply(out this, ref this, ref b);
    }

    /// <summary>
    /// Transforms the distance vector (<paramref name="dx"/>, <paramref name="dy"/>) by <see cref="Matrix"/>.
    /// This is similar to <see cref="TransformPoint(ref double, ref double)"/> except that the translation
    /// components of the transformation are ignored.
    /// </summary>
    /// <param name="dx">X component of a distance vector. An in/out parameter</param>
    /// <param name="dy">Y component of a distance vector. An in/out parameter</param>
    /// <remarks>
    /// The calculation of the returned vector is as follows:<br/>
    /// dx_new = xx * dx + xy * dy;<br/>
    /// dy_new = yx * dx + yy * dy;
    /// </remarks>
    public void TransformDistance(ref double dx, ref double dy) => MatrixNative.cairo_matrix_transform_distance(ref this, ref dx, ref dy);

    /// <summary>
    /// Transforms the point (<paramref name="x"/>, <paramref name="y"/>) by <see cref="Matrix"/>.
    /// </summary>
    /// <param name="x">X position. An in/out parameter</param>
    /// <param name="y">Y position. An in/out parameter</param>
    public void TransformPoint(ref double x, ref double y) => MatrixNative.cairo_matrix_transform_point(ref this, ref x, ref y);

    public override readonly string ToString()
    {
        return $"""
            {Xx:0:##0.0#} {Yx:0:##0.0#}
            {Xy:0:##0.0#} {Yy:0:##0.0#}
            {X0:0:##0.0#} {Y0:0:##0.0#}
            """;
    }

    public static bool operator ==(Matrix lhs, Matrix rhs)
    {
        if (Vector256.IsHardwareAccelerated)
        {
            ref double left = ref Unsafe.As<Matrix, double>(ref lhs);
            ref double right = ref Unsafe.As<Matrix, double>(ref rhs);

            Vector256<double> vl0 = Vector256.LoadUnsafe(ref left, 0);
            Vector256<double> vr0 = Vector256.LoadUnsafe(ref right, 0);

            Vector256<double> vl1 = Vector256.LoadUnsafe(ref left, 2);
            Vector256<double> vr1 = Vector256.LoadUnsafe(ref right, 2);

            return vl0 == vr0
                && vl1 == vr1;
        }

        if (Vector128.IsHardwareAccelerated)
        {
            ref double left = ref Unsafe.As<Matrix, double>(ref lhs);
            ref double right = ref Unsafe.As<Matrix, double>(ref rhs);

            Vector128<double> vl0 = Vector128.LoadUnsafe(ref left, 0);
            Vector128<double> vr0 = Vector128.LoadUnsafe(ref right, 0);

            Vector128<double> vl1 = Vector128.LoadUnsafe(ref left, 2);
            Vector128<double> vr1 = Vector128.LoadUnsafe(ref right, 2);

            Vector128<double> vl2 = Vector128.LoadUnsafe(ref left, 4);
            Vector128<double> vr2 = Vector128.LoadUnsafe(ref right, 4);

            return vl0 == vr0
                && vl1 == vr1
                && vl2 == vr2;
        }

        return lhs.Xx == rhs.Xx
            && lhs.Xy == rhs.Xy
            && lhs.Yx == rhs.Yx
            && lhs.Yy == rhs.Yy
            && lhs.X0 == rhs.X0
            && lhs.Y0 == rhs.Y0;
    }

    public static bool operator !=(Matrix lhs, Matrix rhs) => !(lhs == rhs);

    public override readonly bool Equals(object? o)
    {
        if (o is not Matrix m)
        {
            return false;
        }

        return m == this;
    }

    public override readonly int GetHashCode()
    {
        HashCode hc = new();
        hc.Add(Xx); hc.Add(Yx);
        hc.Add(Xy); hc.Add(Yy);
        hc.Add(X0); hc.Add(Y0);

        return hc.ToHashCode();
    }
}

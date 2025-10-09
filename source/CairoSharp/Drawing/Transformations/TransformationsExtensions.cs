// (c) gfoidl, all rights reserved

using Cairo.Utilities;
using static Cairo.Drawing.Transformations.TransformationsNative;

namespace Cairo;

/// <summary>
/// Transformations — Manipulating the current transformation matrix
/// </summary>
/// <remarks>
/// The current transformation matrix, ctm, is a two-dimensional affine transformation that maps all coordinates and other drawing instruments from the user space into the surface's canonical coordinate system, also known as the device space.
/// </remarks>
public static unsafe class TransformationsExtensions
{
    extension(CairoContext cr)
    {
        /// <summary>
        /// Modifies the current transformation matrix (CTM) by translating the user-space origin
        /// by (tx , ty ). This offset is interpreted as a user-space coordinate according to the
        /// CTM in place before the new call to <see cref="Translate(CairoContext, double, double)"/>.
        /// In other words, the translation of the user-space origin takes place after any
        /// existing transformation.
        /// </summary>
        /// <param name="tx">amount to translate in the X direction</param>
        /// <param name="ty">amount to translate in the Y direction</param>
        public void Translate(double tx, double ty)
        {
            cr.CheckDisposed();
            cairo_translate(cr.Handle, tx, ty);
        }

        /// <summary>
        /// Modifies the current transformation matrix (CTM) by scaling the X and Y user-space axes
        /// by sx and sy respectively. The scaling of the axes takes place after any existing
        /// transformation of user space.
        /// </summary>
        /// <param name="sx">scale factor for the X dimension</param>
        /// <param name="sy">scale factor for the Y dimension</param>
        public void Scale(double sx, double sy)
        {
            cr.CheckDisposed();
            cairo_scale(cr.Handle, sx, sy);
        }

        /// <summary>
        /// Modifies the current transformation matrix (CTM) by rotating the user-space axes by
        /// angle radians. The rotation of the axes takes places after any existing transformation
        /// of user space. The rotation direction for positive angles is from the positive X axis
        /// toward the positive Y axis.
        /// </summary>
        /// <param name="angle">angle (in radians) by which the user-space axes will be rotated</param>
        public void Rotate(double angle)
        {
            cr.CheckDisposed();
            cairo_rotate(cr.Handle, angle);
        }

        /// <summary>
        /// Modifies the current transformation matrix (CTM) by applying matrix as an additional
        /// transformation. The new transformation of user space takes place after any
        /// existing transformation.
        /// </summary>
        /// <param name="matrix">a transformation to be applied to the user-space axes</param>
        public void Transform(ref Matrix matrix)
        {
            cr.CheckDisposed();
            cairo_transform(cr.Handle, ref matrix);
        }

        /// <summary>
        /// Gets the current transformation matrix (CTM).
        /// </summary>
        public void GetMatrix(out Matrix matrix)
        {
            cr.CheckDisposed();
            cairo_get_matrix(cr.Handle, out matrix);
        }

        /// <summary>
        /// Sets the current transformation matrix (CTM).
        /// </summary>
        public void SetMatrix(ref Matrix matrix)
        {
            cr.CheckDisposed();
            cairo_set_matrix(cr.Handle, ref matrix);
        }

        /// <summary>
        /// Resets the current transformation matrix (CTM) by setting it equal to the identity
        /// matrix. That is, the user-space and device-space axes will be aligned and one
        /// user-space unit will transform to one device-space unit.
        /// </summary>
        public void IdentityMatrix()
        {
            cr.CheckDisposed();
            cairo_identity_matrix(cr.Handle);
        }

        /// <summary>
        /// Transform a coordinate from user space to device space by multiplying the
        /// given point by the current transformation matrix (CTM).
        /// </summary>
        /// <param name="x">X value of coordinate (in/out parameter)</param>
        /// <param name="y">Y value of coordinate (in/out parameter)</param>
        public void UserToDevice(ref double x, ref double y)
        {
            cr.CheckDisposed();
            cairo_user_to_device(cr.Handle, ref x, ref y);
        }

        /// <summary>
        /// Transform a distance vector from user space to device space.
        /// </summary>
        /// <param name="dx">X component of a distance vector (in/out parameter)</param>
        /// <param name="dy">Y component of a distance vector (in/out parameter)</param>
        /// <remarks>
        ///  This method is similar to <see cref="UserToDevice(CairoContext, ref double, ref double)"/>
        ///  except that the translation components of the CTM will be ignored when transforming (dx ,dy ).
        /// </remarks>
        public void UserToDeviceDistance(ref double dx, ref double dy)
        {
            cr.CheckDisposed();
            cairo_user_to_device_distance(cr.Handle, ref dx, ref dy);
        }

        /// <summary>
        /// Transform a coordinate from device space to user space by multiplying the given
        /// point by the inverse of the current transformation matrix (CTM).
        /// </summary>
        /// <param name="x">X value of coordinate (in/out parameter)</param>
        /// <param name="y">Y value of coordinate (in/out parameter)</param>
        public void DeviceToUser(ref double x, ref double y)
        {
            cr.CheckDisposed();
            cairo_device_to_user(cr.Handle, ref x, ref y);
        }

        /// <summary>
        /// Transform a distance vector from device space to user space.
        /// </summary>
        /// <param name="dx">X component of a distance vector (in/out parameter)</param>
        /// <param name="dy">Y component of a distance vector (in/out parameter)</param>
        /// <remarks>
        /// This method is similar to <see cref="DeviceToUser(CairoContext, ref double, ref double)"/>
        /// except that the translation components of the inverse CTM will be ignored when
        /// transforming (dx ,dy ).
        /// </remarks>
        public void DeviceToUserDistance(ref double dx, ref double dy)
        {
            cr.CheckDisposed();
            cairo_device_to_user_distance(cr.Handle, ref dx, ref dy);
        }
    }
}

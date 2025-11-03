// (c) gfoidl, all rights reserved

using System.Diagnostics;
using GLib.Internal;
using Gtk;

namespace Gtk4.Extensions;

public static class PropertyExpressionExtensions
{
    extension(PropertyExpression)
    {
        public static PropertyExpression Create(GObject.Type type, string propertyName)
        {
            ArgumentNullException.ThrowIfNull(propertyName);

            IntPtr handle = Gtk.Internal.PropertyExpression.New(type, IntPtr.Zero, NonNullableUtf8StringOwnedHandle.Create(propertyName));

            Debug.Assert(handle is not 0);

            return new PropertyExpression(handle);
        }
    }
}

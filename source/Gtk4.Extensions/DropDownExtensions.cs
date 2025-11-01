// (c) gfoidl, all rights reserved

//#define SELECTED_VIA_PROP_FILTER
//#define SELECTED_VIA_CONNECT_AND_DETAIL
#define SELECTED_VIA_NOTIFY

using System.Diagnostics;
using Gtk;
using static GObject.Object;

namespace Gtk4.Extensions;

/// <summary>
/// Handler that is invoked when the <see cref="DropDown.Selected"/> property is changed.
/// </summary>
/// <param name="dropDown">The dropdown</param>
/// <param name="args">
/// Contains information about the <see cref="DropDown.Selected"/>, and the
/// <see cref="DropDown.SelectedItem"/> values.
/// </param>
public delegate void DropDownNotifySelectedSignalHandler(DropDown dropDown, DropDownNotifySelectedArgs args);
public readonly record struct DropDownNotifySelectedArgs(uint Selected, GObject.Object? SelectedItem);

/// <summary>
/// Extension methods for <see cref="DropDown"/>.
/// </summary>
public static class DropDownExtensions
{
    extension(DropDown dropDown)
    {
        /// <summary>
        /// Registers a signal that is triggered when the <see cref="DropDown.Selected"/>
        /// property is changed.
        /// </summary>
        /// <param name="signalHandler">The signal handler</param>
        public void OnNotifySelected(DropDownNotifySelectedSignalHandler signalHandler)
        {
            // Cf. https://gircore.github.io/docs/faq.html#property-changed-notifications

            Debug.Assert(DropDown.SelectedPropertyDefinition.UnmanagedName == "selected");

#if SELECTED_VIA_PROP_FILTER
            dropDown.OnNotify += (GObject.Object sender, NotifySignalArgs args) =>
            {
                string name = args.Pspec.GetName();

                if (name == DropDown.SelectedPropertyDefinition.UnmanagedName)
                {
                    Handler(sender, args);
                }
            };
#elif SELECTED_VIA_CONNECT_AND_DETAIL
            NotifySignal.Connect(
                sender       : dropDown,
                signalHandler: Handler,
                detail       : DropDown.SelectedPropertyDefinition.UnmanagedName);
#elif SELECTED_VIA_NOTIFY
            // This is just a convenience method for the former approach.
            DropDown.SelectedPropertyDefinition.Notify(
                sender       : dropDown,
                signalHandler: Handler);
#endif

            void Handler(GObject.Object sender, NotifySignalArgs args)
            {
                Debug.Assert(args.Pspec.GetName() == DropDown.SelectedPropertyDefinition.UnmanagedName);
                Debug.Assert(sender is DropDown);

                DropDown dropDown = (sender as DropDown)!;

                uint selected                = dropDown.Selected;
                GObject.Object? selectedItem = dropDown.SelectedItem;

                signalHandler(dropDown, new DropDownNotifySelectedArgs(selected, selectedItem));
            }
        }
    }
}

// (c) gfoidl, all rights reserved

extern alias CairoSharp;

using CairoSharp::Cairo;
using Gio;
using Action   = System.Action;
using GtkCairo = Cairo.Context;
using Task     = System.Threading.Tasks.Task;

namespace Gtk4Demo;

internal static class GtkExtensions
{
    extension(GtkCairo gtkCairo)
    {
        public CairoContext ToCairoSharp() => new(gtkCairo.Handle.DangerousGetHandle());
    }

    extension(ActionMap map)
    {
        public SimpleAction AddAction(string name, Action onActivateCallback)
        {
            SimpleAction simpleAction = SimpleAction.New(name, null);
            simpleAction.OnActivate  += (sa, args) => onActivateCallback();

            map.AddAction(simpleAction);
            return simpleAction;
        }

        public SimpleAction AddAction(string name, Action<string?> onActivateCallback)
        {
            SimpleAction simpleAction = SimpleAction.New(name, null);
            simpleAction.OnActivate  += (sa, args) => onActivateCallback(sa.Name);

            map.AddAction(simpleAction);
            return simpleAction;
        }

        public SimpleAction AddAction(string name, Func<Task> onActivateCallback)
        {
            SimpleAction simpleAction = SimpleAction.New(name, null);
            simpleAction.OnActivate  += async (sa, args) => await onActivateCallback();

            map.AddAction(simpleAction);
            return simpleAction;
        }

        public SimpleAction AddAction(string name, Func<string?, Task> onActivateCallback)
        {
            SimpleAction simpleAction = SimpleAction.New(name, null);
            simpleAction.OnActivate  += async (sa, args) => await onActivateCallback(sa.Name);

            map.AddAction(simpleAction);
            return simpleAction;
        }
    }
}

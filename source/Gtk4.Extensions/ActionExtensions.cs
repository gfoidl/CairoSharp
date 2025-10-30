using Gio;
using Action = System.Action;
using Task   = System.Threading.Tasks.Task;

namespace Gtk4.Extensions;

public static class ActionExtensions
{
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

// (c) gfoidl, all rights reserved

namespace Gtk4.Extensions;

public static class Gtk4Constants
{
    // These should be exposed by GirCore ideally.
    public const uint GdkButtonAll       = 0;
    public const uint GdkButtonPrimary   = 1;
    public const uint GdkButtonMiddle    = 2;
    public const uint GdkButtonSecondary = 3;

    public const bool SourceContinue = true;
    public const bool SourceRemove   = false;

    public const uint InvalidListPosition = 4294967295;

    public const uint StyleProviderPriorityFallback    =   1;
    public const uint StyleProviderPriorityTheme       = 200;
    public const uint StyleProviderPrioritySettings    = 400;
    public const uint StyleProviderPriorityApplication = 600;
    public const uint StyleProviderPriorityUser        = 800;
}

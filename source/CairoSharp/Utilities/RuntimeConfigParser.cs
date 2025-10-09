// (c) gfoidl, all rights reserved

namespace Cairo.Utilities;

internal static class RuntimeConfigParser
{
    public static bool GetRuntimeSettingSwitch(string appContextSwitchName, string environmentVariable, bool defaultValue = false)
    {
        if (AppContext.TryGetSwitch(appContextSwitchName, out bool isEnabled))
        {
            return isEnabled;
        }

        string? envValue = Environment.GetEnvironmentVariable(environmentVariable);

        if (bool.TryParse(envValue, out isEnabled))
        {
            return isEnabled;
        }

        if (uint.TryParse(envValue, out uint value))
        {
            return value > 0;
        }

        return defaultValue;
    }
}

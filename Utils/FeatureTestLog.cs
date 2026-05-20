using System.Collections.Generic;

namespace BetterSleep;

internal static class FeatureTestLog
{
    private static readonly HashSet<string> LoggedOnceKeys = new HashSet<string>();

    internal static void Log(string feature, string detail)
    {
        BetterSleep.LogDebug(message: "[FeatureTest] " + feature + ": " + detail);
    }

    internal static void LogOnce(string feature, string key, string detail)
    {
        string combinedKey = feature + "|" + key;
        if (LoggedOnceKeys.Add(item: combinedKey) == false)
        {
            return;
        }

        Log(feature: feature, detail: detail);
    }
}

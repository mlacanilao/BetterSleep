namespace BetterSleep.Patches;

internal static class SleepPatch
{
    public static void SleepPrefix(ref int _hours)
    {
        int effectiveSleepHours = BetterSleepConfig.GetEffectiveSleepHours();
        _hours = effectiveSleepHours;
        FeatureTestLog.Log(
            feature: "Sleep Hours",
            detail: "set LayerSleep hours to " + effectiveSleepHours.ToString());
    }
}

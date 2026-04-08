using System;

namespace BetterSleep.Patches;

internal static class CharaPatch
{
    public static bool CanSleepPrefix(Chara __instance, ref bool __result)
    {
        bool enableCanSleep = BetterSleepConfig.EnableCanSleep?.Value ?? true;
        bool enableCanSleepDuringMeditate = BetterSleepConfig.EnableCanSleepDuringMeditate?.Value ?? false;

        if (enableCanSleep == true &&
            __instance.ai is AI_Meditate == false)
        {
            __result = true;
            return false;
        }

        if (enableCanSleepDuringMeditate == true &&
            __instance.ai is AI_Meditate == true)
        {
            __result = true;
            return false;
        }

        if (enableCanSleepDuringMeditate == false &&
            __instance.ai is AI_Meditate == true)
        {
            __result = false;
            return false;
        }

        return true;
    }

    public static void OnSleepPrefix(ref int power)
    {
        bool enableSleepPowerMultiplier = BetterSleepConfig.EnableSleepPowerMultiplier?.Value ?? false;

        if (enableSleepPowerMultiplier == true)
        {
            int multiplier = BetterSleepConfig.SleepPowerMultiplier?.Value ?? 1;
            power *= multiplier;
        }
    }

    public static void SleepPostfix(Chara __instance)
    {
        bool enableSleepDelay = BetterSleepConfig.EnableSleepDelay?.Value ?? false;
        int customSleepDelayTurns = BetterSleepConfig.SleepDelayTurns?.Value ?? 15;

        ConSleep? consleep = __instance.conSleep;
        if (enableSleepDelay == true &&
            consleep != null)
        {
            consleep.pcSleep = Math.Max(val1: 1, val2: customSleepDelayTurns);
        }
    }
}

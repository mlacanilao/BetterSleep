namespace BetterSleep.Patches
{
    public class CharaPatch
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
        
        public static void OnSleepPrefix(ref int power, int days)
        {
            bool enableSleepPowerMultiplier = BetterSleepConfig.EnableSleepPowerMultiplier?.Value ?? false;
            
            if (enableSleepPowerMultiplier == true)
            {
                int multiplier = BetterSleepConfig.SleepPowerMultiplier?.Value ?? 1;
                power *= multiplier;
            }
        }
    }
}
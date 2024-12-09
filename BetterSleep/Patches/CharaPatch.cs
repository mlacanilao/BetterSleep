namespace BetterSleep.Patches
{
    public class CharaPatch
    {
        public static bool CanSleep(Chara __instance, ref bool __result)
        {
            if (BetterSleepConfig.EnableBetterSleepMod?.Value == true &&
                BetterSleepConfig.EnableCanSleepDuringMeditate?.Value == true &&
                __instance.ai is AI_Meditate)
            {
                __result = BetterSleepConfig.CanSleepDuringMeditate?.Value ?? false;
                return false;
            }

            if (BetterSleepConfig.EnableBetterSleepMod?.Value == true &&
                BetterSleepConfig.EnableCanSleep?.Value == true)
            {
                __result = BetterSleepConfig.CanSleep?.Value ?? true;
                return false;
            }

            return true;
        }
        
        public static void OnSleep(ref int power, int days)
        {
            if (BetterSleepConfig.EnableBetterSleepMod?.Value == true &&
                BetterSleepConfig.EnableSleepPowerMultiplier?.Value == true)
            {
                int multiplier = BetterSleepConfig.SleepPowerMultiplier?.Value ?? 1;
                power *= multiplier;
            }
        }
    }
}
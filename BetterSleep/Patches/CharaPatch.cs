namespace BetterSleep.Patches
{
    public class CharaPatch
    {
        public static bool CanSleep(Chara __instance, ref bool __result)
        {
            if (BetterSleepConfig.EnableBetterSleepMod?.Value == true &&
                BetterSleepConfig.EnableSleepDuringMeditate?.Value == false &&
                __instance.ai is AI_Meditate)
            {
                return true;
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
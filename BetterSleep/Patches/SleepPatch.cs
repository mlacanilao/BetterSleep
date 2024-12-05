namespace BetterSleep.Patches
{
    public class SleepPatch
    {
        public static void Sleep(ref int _hours)
        {
            if (BetterSleepConfig.EnableBetterSleepMod?.Value == true &&
                BetterSleepConfig.EnableSleepHours?.Value == true)
            {
                _hours = BetterSleepConfig.SleepHours?.Value ?? 6;
            }
        }
    }
}
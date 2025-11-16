namespace BetterSleep.Patches
{
    public class SleepPatch
    {
        public static void SleepPrefix(ref int _hours)
        {
            int sleepHours = BetterSleepConfig.SleepHours?.Value ?? 6;
            
            _hours = sleepHours;
        }
    }
}
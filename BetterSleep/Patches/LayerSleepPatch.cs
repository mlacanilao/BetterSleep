namespace BetterSleep.Patches
{
    public class LayerSleepPatch
    {
        public static void Advance()
        {
            if (BetterSleepConfig.EnableBetterSleepMod?.Value == true &&
                BetterSleepConfig.EnableIgnoreAutoSave?.Value == true)
            {
                ELayer.debug.ignoreAutoSave = BetterSleepConfig.IgnoreAutoSave?.Value ?? false;
            }
        }
    }
}
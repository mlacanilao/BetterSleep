namespace BetterSleep.Patches
{
    public class LayerSleepPatch
    {
        public static void AdvancePrefix()
        {
            if (ELayer.core?.IsGameStarted == false)
            {
                return;
            }

            bool enableAutoSave = BetterSleepConfig.EnableAutoSave?.Value ?? true;
            
            ELayer.debug.ignoreAutoSave = enableAutoSave == false;
        }
    }
}
using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace BetterSleep
{
    internal static class ModInfo
    {
        internal const string Guid = "omegaplatinum.elin.bettersleep";
        internal const string Name = "Better Sleep";
        internal const string Version = "1.3.0.0";
    }

    [BepInPlugin(GUID: ModInfo.Guid, Name: ModInfo.Name, Version: ModInfo.Version)]
    public class BetterSleep : BaseUnityPlugin
    {
        private void Start()
        {
            BetterSleepConfig.LoadConfig(config: Config);
            Harmony.CreateAndPatchAll(type: typeof(Patcher), harmonyInstanceId: null);
            Logger.LogInfo(data: "Plugin [BetterSleep] is loaded!");
        }

        private void Update()
        {
            HandleInput();
        }

        private void HandleInput()
        {
            if (EInput.isInputFieldActive) return;

            if (Input.GetKeyDown(key: BetterSleepConfig.IncreaseSleepHoursKey.Value))
            {
                AdjustSleepHours(adjustment: 1);
            }

            if (Input.GetKeyDown(key: BetterSleepConfig.DecreaseSleepHoursKey.Value))
            {
                AdjustSleepHours(adjustment: -1);
            }
        }

        private void AdjustSleepHours(int adjustment)
        {
            if (BetterSleepConfig.EnableBetterSleepMod?.Value == true)
            {
                int newSleepHours = BetterSleepConfig.SleepHours.Value + adjustment;
                newSleepHours = Mathf.Clamp(value: newSleepHours, min: 0, max: 12);
                BetterSleepConfig.SleepHours.Value = newSleepHours;
                ELayer.pc.TalkRaw(text: $"Sleep Hours set to: {newSleepHours}", ref1: null, ref2: null, forceSync: false);
            }
        }
    }
}
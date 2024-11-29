using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace BetterSleep
{
    internal static class ModInfo
    {
        internal const string Guid = "omegaplatinum.elin.bettersleep";
        internal const string Name = "Better Sleep";
        internal const string Version = "1.0.0.0";
    }

    [BepInPlugin(GUID: ModInfo.Guid, Name: ModInfo.Name, Version: ModInfo.Version)]
    internal class BetterSleep : BaseUnityPlugin
    {
        internal static BetterSleep Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
            BetterSleepConfig.LoadConfig(config: Config);
            var harmony = new Harmony(id: ModInfo.Guid);
            harmony.PatchAll();
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
            if (BetterSleepConfig.EnableBetterSleep?.Value == true)
            {
                int newSleepHours = BetterSleepConfig.SleepHours.Value + adjustment;
                newSleepHours = Mathf.Clamp(value: newSleepHours, min: 1, max: 12);
                BetterSleepConfig.SleepHours.Value = newSleepHours;

                ELayer.pc.TalkRaw(text: $"Sleep hours updated to: {newSleepHours}", ref1: null, ref2: null, forceSync: false);
            }
        }
    }

    [HarmonyPatch(declaringType: typeof(Chara), methodName: nameof(Chara.CanSleep))]
    internal static class CanSleepPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(Chara __instance, ref bool __result)
        {
            if (BetterSleepConfig.EnableBetterSleep?.Value == true)
            {
                __result = BetterSleepConfig.CanSleepValue?.Value ?? true;
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(declaringType: typeof(LayerSleep), methodName: nameof(LayerSleep.Sleep))]
    internal static class LayerSleepSleepPatch
    {
        [HarmonyPrefix]
        public static void Prefix(ref int _hours)
        {
            if (BetterSleepConfig.EnableBetterSleep?.Value == true)
            {
                _hours = BetterSleepConfig.SleepHours?.Value ?? 6;
            }
        }
    }

    [HarmonyPatch(declaringType: typeof(LayerSleep), methodName: nameof(LayerSleep.Advance))]
    internal static class LayerSleepAdvancePatch
    {
        [HarmonyPrefix]
        public static void Prefix()
        {
            if (BetterSleepConfig.EnableBetterSleep?.Value == true)
            {
                ELayer.debug.ignoreAutoSave = BetterSleepConfig.IgnoreAutoSave?.Value ?? false;
            }
        }
    }
}
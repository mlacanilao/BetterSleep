using System;
using System.Linq;
using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace BetterSleep
{
    internal static class ModInfo
    {
        internal const string Guid = "omegaplatinum.elin.bettersleep";
        internal const string Name = "Better Sleep";
        internal const string Version = "2.0.1.0";
        internal const string ModOptionsGuid = "evilmask.elinplugins.modoptions";
        internal const string ModOptionsAssemblyName = "ModOptions";
    }

    [BepInPlugin(GUID: ModInfo.Guid, Name: ModInfo.Name, Version: ModInfo.Version)]
    public class BetterSleep : BaseUnityPlugin
    {
        internal static BetterSleep Instance { get; private set; }

        private void Start()
        {
            Instance = this;
            
            BetterSleepConfig.LoadConfig(config: Config);
            
            Harmony.CreateAndPatchAll(type: typeof(Patcher), harmonyInstanceId: ModInfo.Guid);
            
            if (IsModOptionsInstalled())
            {
                try
                {
                    UI.UIController.RegisterUI();
                }
                catch (Exception ex)
                {
                    Log(payload: $"An error occurred during UI registration: {ex.Message}");
                }
            }
            else
            {
                Log(payload: "Mod Options is not installed. Skipping UI registration.");
            }
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
            if (EClass.core.IsGameStarted == true)
            {
                int newSleepHours = BetterSleepConfig.SleepHours.Value + adjustment;
                newSleepHours = Mathf.Clamp(value: newSleepHours, min: 0, max: 12);
                BetterSleepConfig.SleepHours.Value = newSleepHours;
                ELayer.pc.TalkRaw(
                    text: BetterSleep.__(
                        jp: $"睡眠時間が設定されました: {newSleepHours}",
                        en: $"Sleep Hours set to: {newSleepHours}",
                        cn: $"睡眠时间已设置为: {newSleepHours}"
                    ),
                    ref1: null,
                    ref2: null,
                    forceSync: false
                );
            }
        }
        
        internal static void Log(object payload)
        {
            Instance.Logger.LogInfo(data: payload);
        }
        
        private bool IsModOptionsInstalled()
        {
            try
            {
                return AppDomain.CurrentDomain
                    .GetAssemblies()
                    .Any(predicate: assembly => assembly.GetName().Name == ModInfo.ModOptionsAssemblyName);
            }
            catch (Exception ex)
            {
                Log(payload: $"Error while checking for Mod Options: {ex.Message}");
                return false;
            }
        }
        
        public static string __(string jp = "", string en = "", string cn = "")
        {
            if (Lang.langCode == "JP")
            {
                return jp ?? en;
            }

            if (Lang.langCode == "CN")
            {
                return cn ?? en;
            }

            return en;
        }
    }
}
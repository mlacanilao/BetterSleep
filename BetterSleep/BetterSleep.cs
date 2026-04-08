using System;
using System.Runtime.CompilerServices;
using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace BetterSleep;

internal static class ModInfo
{
    internal const string Guid = "omegaplatinum.elin.bettersleep";
    internal const string Name = "Better Sleep";
    internal const string Version = "3.0.0";
    internal const string ModOptionsGuid = "evilmask.elinplugins.modoptions";
}

[BepInPlugin(GUID: ModInfo.Guid, Name: ModInfo.Name, Version: ModInfo.Version)]
internal class BetterSleep : BaseUnityPlugin
{
    internal static BetterSleep? Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        BetterSleepConfig.LoadConfig(config: Config);
        Harmony.CreateAndPatchAll(type: typeof(Patcher), harmonyInstanceId: ModInfo.Guid);

        if (HasModOptionsPlugin() == false)
        {
            return;
        }

        try
        {
            UIController.RegisterUI();
        }
        catch (Exception ex)
        {
            LogError(message: $"An error occurred during UI registration: {ex}");
        }
    }

    private void Update()
    {
        HandleInput();
    }

    private static void HandleInput()
    {
        bool increaseSleepHoursPressed = Input.GetKeyDown(key: BetterSleepConfig.IncreaseSleepHoursKey.Value);
        bool decreaseSleepHoursPressed = Input.GetKeyDown(key: BetterSleepConfig.DecreaseSleepHoursKey.Value);

        if (increaseSleepHoursPressed == false &&
            decreaseSleepHoursPressed == false)
        {
            return;
        }

        if (EClass.core.IsGameStarted != true)
        {
            return;
        }

        if (EInput.isInputFieldActive)
        {
            return;
        }

        if (increaseSleepHoursPressed)
        {
            AdjustSleepHours(adjustment: 1);
        }
        else if (decreaseSleepHoursPressed)
        {
            AdjustSleepHours(adjustment: -1);
        }
    }

    private static void AdjustSleepHours(int adjustment)
    {
        int newSleepHours = BetterSleepConfig.SleepHours.Value + adjustment;
        newSleepHours = Mathf.Clamp(value: newSleepHours, min: 0, max: 12);
        BetterSleepConfig.SleepHours.Value = newSleepHours;

        ELayer.pc.TalkRaw(
            text: GetLocalizedText(
                jp: $"睡眠時間が設定されました: {newSleepHours}",
                en: $"Sleep Hours set to: {newSleepHours}",
                cn: $"睡眠时间已设置为: {newSleepHours}"),
            ref1: null,
            ref2: null,
            forceSync: false);
    }

    internal static void LogDebug(object message, [CallerMemberName] string caller = "")
    {
        Instance?.Logger.LogDebug(data: $"[{caller}] {message}");
    }

    internal static void LogInfo(object message)
    {
        Instance?.Logger.LogInfo(data: message);
    }

    internal static void LogError(object message)
    {
        Instance?.Logger.LogError(data: message);
    }

    internal static string GetLocalizedText(string jp = "", string en = "", string cn = "")
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

    private static bool HasModOptionsPlugin()
    {
        try
        {
            foreach (var obj in ModManager.ListPluginObject)
            {
                if (obj is not BaseUnityPlugin plugin)
                {
                    continue;
                }

                if (plugin.Info.Metadata.GUID == ModInfo.ModOptionsGuid)
                {
                    return true;
                }
            }

            return false;
        }
        catch (Exception ex)
        {
            LogError(message: $"Error while checking for Mod Options: {ex}");
            return false;
        }
    }
}

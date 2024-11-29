using BepInEx.Configuration;
using UnityEngine;

namespace BetterSleep
{
    internal static class BetterSleepConfig
    {
        internal static ConfigEntry<bool> EnableBetterSleep;
        internal static ConfigEntry<bool> CanSleepValue;
        internal static ConfigEntry<int> SleepHours;
        internal static ConfigEntry<bool> IgnoreAutoSave;
        internal static ConfigEntry<KeyCode> IncreaseSleepHoursKey;
        internal static ConfigEntry<KeyCode> DecreaseSleepHoursKey;

        internal static void LoadConfig(ConfigFile config)
        {
            EnableBetterSleep = config.Bind(
                section: ModInfo.Name,
                key: "Enable Better Sleep",
                defaultValue: true,
                description: "Enable or disable the Better Sleep mod.\n" +
                             "Set to 'true' to activate the mod, or 'false' to keep the game unchanged.\n" +
                             "ベター・スリープMODを有効または無効にします。\n" +
                             "'true' に設定するとMODが有効になり、'false' に設定するとゲームのデフォルトのままになります。");

            CanSleepValue = config.Bind(
                section: ModInfo.Name,
                key: "Can Sleep Value",
                defaultValue: true,
                description: "Control whether the player character is always allowed to sleep.\n" +
                             "Set to 'true' to allow sleeping anytime, or 'false' to follow the game's normal sleep rules.\n" +
                             "プレイヤーキャラクターが常に眠れるようにするかどうかを設定します。\n" +
                             "'true' に設定すると、いつでも眠ることができ、'false' に設定するとゲームの通常の睡眠ルールに従います。");

            SleepHours = config.Bind(
                section: ModInfo.Name,
                key: "Sleep Hours",
                defaultValue: 6,
                description: "Set the number of hours slept during sleep events.\n" +
                             "You can also adjust this value in-game by pressing the configured keys.\n" +
                             "睡眠イベント中に眠る時間数を設定します。\n" +
                             "設定されたキーを押して、ゲーム内でこの値を調整することもできます。");

            IgnoreAutoSave = config.Bind(
                section: ModInfo.Name,
                key: "Ignore Auto Save",
                defaultValue: false,
                description: "Control whether autosaving is skipped during sleep events.\n" +
                             "Set to 'true' to skip autosaving during sleep, or 'false' to keep autosaving enabled.\n" +
                             "睡眠イベント中にオートセーブをスキップするかどうかを設定します。\n" +
                             "'true' に設定すると睡眠中のオートセーブがスキップされ、'false' に設定するとオートセーブが有効のままになります。");

            IncreaseSleepHoursKey = config.Bind(
                section: ModInfo.Name,
                key: "Increase Sleep Hours Key",
                defaultValue: KeyCode.Equals,
                description: "Key to increase the sleep hours in-game.\n" +
                             "ゲーム内で睡眠時間を増やすためのキーを設定します。");

            DecreaseSleepHoursKey = config.Bind(
                section: ModInfo.Name,
                key: "Decrease Sleep Hours Key",
                defaultValue: KeyCode.Minus,
                description: "Key to decrease the sleep hours in-game.\n" +
                             "ゲーム内で睡眠時間を減らすためのキーを設定します。");
        }
    }
}
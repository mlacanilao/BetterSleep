using BepInEx.Configuration;
using UnityEngine;

namespace BetterSleep
{
    internal static class BetterSleepConfig
    {
        internal static ConfigEntry<bool> EnableBetterSleep;
        internal static ConfigEntry<bool> EnableCanSleep;
        internal static ConfigEntry<bool> CanSleep;
        internal static ConfigEntry<int> SleepHours;
        internal static ConfigEntry<bool> IgnoreAutoSave;
        internal static ConfigEntry<KeyCode> IncreaseSleepHoursKey;
        internal static ConfigEntry<KeyCode> DecreaseSleepHoursKey;
        internal static ConfigEntry<bool> EnableSleepPowerMultiplier;
        internal static ConfigEntry<int> SleepPowerMultiplier;

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

            EnableCanSleep = config.Bind(
                section: ModInfo.Name,
                key: "Enable Can Sleep",
                defaultValue: true,
                description: "Enable or disable the custom 'Can Sleep' behavior.\n" +
                             "Set to 'true' to use the custom Can Sleep logic, or 'false' to disable it.\n" +
                             "カスタムの「眠れるかどうか」の動作を有効または無効にします。\n" +
                             "'true' に設定するとカスタムロジックが使用され、'false' に設定すると無効になります。");

            CanSleep = config.Bind(
                section: ModInfo.Name,
                key: "Can Sleep",
                defaultValue: true,
                description: "Control whether the player character is allowed to sleep.\n" +
                             "Set to 'true' to allow sleeping anytime, or 'false' to disable sleeping completely.\n" +
                             "プレイヤーキャラクターが眠れるようにするかどうかを設定します。\n" +
                             "'true' に設定するといつでも眠ることができ、'false' に設定すると眠ることが完全に無効になります。");

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

            EnableSleepPowerMultiplier = config.Bind(
                section: ModInfo.Name,
                key: "Enable Sleep Power Multiplier",
                defaultValue: false,
                description: "Enable or disable the sleep power multiplier.\n" +
                             "Set to 'true' to activate the multiplier, or 'false' to disable it.\n" +
                             "睡眠パワー乗数を有効または無効にします。\n" +
                             "'true' に設定すると乗数が有効になり、'false' に設定すると無効になります。");

            SleepPowerMultiplier = config.Bind(
                section: ModInfo.Name,
                key: "Sleep Power Multiplier",
                defaultValue: 1,
                description: "Multiplier for the power value during sleep. Must be a whole number.\n" +
                             "This multiplier affects the following:\n" +
                             "- HP healing\n" +
                             "- Stamina recovery\n" +
                             "- Mana restoration\n" +
                             "睡眠中のパワー値に乗じる倍率を設定します。整数のみ。\n" +
                             "これには以下が影響します:\n" +
                             "- HP回復\n" +
                             "- スタミナ回復\n" +
                             "- マナ回復");
        }
    }
}
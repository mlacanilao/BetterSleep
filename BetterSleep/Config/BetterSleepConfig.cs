using BepInEx.Configuration;
using UnityEngine;

namespace BetterSleep
{
    internal static class BetterSleepConfig
    {
        internal static ConfigEntry<bool> EnableBetterSleepMod;
        internal static ConfigEntry<bool> EnableCanSleep;
        internal static ConfigEntry<bool> CanSleep;
        internal static ConfigEntry<bool> EnableSleepHours;
        internal static ConfigEntry<int> SleepHours;
        internal static ConfigEntry<bool> EnableIgnoreAutoSave;
        internal static ConfigEntry<bool> IgnoreAutoSave;
        internal static ConfigEntry<KeyCode> IncreaseSleepHoursKey;
        internal static ConfigEntry<KeyCode> DecreaseSleepHoursKey;
        internal static ConfigEntry<bool> EnableSleepPowerMultiplier;
        internal static ConfigEntry<int> SleepPowerMultiplier;
        internal static ConfigEntry<bool> EnableSleepDuringMeditate;

        internal static void LoadConfig(ConfigFile config)
        {
            EnableBetterSleepMod = config.Bind(
                section: ModInfo.Name,
                key: "Enable Better Sleep Mod",
                defaultValue: true,
                description: "Enable or disable the Better Sleep mod.\n" +
                             "Set to 'true' to activate the mod, or 'false' to keep the game unchanged.\n" +
                             "ベター・スリープMODを有効または無効にします。\n" +
                             "'true' にするとMODが有効になり、'false' にするとゲームの通常の挙動になります。");

            EnableCanSleep = config.Bind(
                section: ModInfo.Name,
                key: "Enable Can Sleep",
                defaultValue: true,
                description: "Enable or disable the custom 'Can Sleep' behavior.\n" +
                             "Set to 'true' to use the custom Can Sleep logic, or 'false' to disable it.\n" +
                             "カスタムの「眠れるかどうか」の挙動を有効または無効にします。\n" +
                             "'true' にするとカスタムロジックが使用され、'false' にすると無効になります。");

            CanSleep = config.Bind(
                section: ModInfo.Name,
                key: "Can Sleep",
                defaultValue: true,
                description: "Control whether the player character is allowed to sleep.\n" +
                             "Set to 'true' to allow sleeping anytime, or 'false' to disable sleeping completely.\n" +
                             "プレイヤーキャラクターがいつでも眠れるかどうかを設定します。\n" +
                             "'true' にすると常に眠ることができ、'false' にすると完全に眠れなくなります。");

            EnableSleepHours = config.Bind(
                section: ModInfo.Name,
                key: "Enable Sleep Hours",
                defaultValue: true,
                description: "Enable or disable setting custom sleep hours.\n" +
                             "Set to 'true' to allow customizing sleep hours, or 'false' to disable this option.\n" +
                             "カスタム睡眠時間の設定を有効または無効にします。\n" +
                             "'true' にすると睡眠時間をカスタマイズでき、'false' にすると無効になります。");

            SleepHours = config.Bind(
                section: ModInfo.Name,
                key: "Sleep Hours",
                defaultValue: 6,
                description: "Set the number of hours slept during sleep events.\n" +
                             "You can also adjust this value in-game by pressing the configured keys.\n" +
                             "睡眠イベントでの睡眠時間を設定します。\n" +
                             "設定されたキーを使ってゲーム内でこの値を調整することもできます。");

            EnableIgnoreAutoSave = config.Bind(
                section: ModInfo.Name,
                key: "Enable Ignore Auto Save",
                defaultValue: true,
                description: "Enable or disable the 'Ignore Auto Save' feature.\n" +
                             "Set to 'true' to allow ignoring autosaves during sleep, or 'false' to always autosave.\n" +
                             "「オートセーブを無視」を有効または無効にします。\n" +
                             "'true' にすると睡眠中にオートセーブを無視し、'false' にすると常にオートセーブされます。");

            IgnoreAutoSave = config.Bind(
                section: ModInfo.Name,
                key: "Ignore Auto Save",
                defaultValue: false,
                description: "Control whether autosaving is skipped during sleep events.\n" +
                             "Set to 'true' to skip autosaving during sleep, or 'false' to keep autosaving enabled.\n" +
                             "睡眠イベント中にオートセーブをスキップするかどうかを設定します。\n" +
                             "'true' にすると睡眠中のオートセーブがスキップされ、'false' にするとオートセーブが有効のままになります。");

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
                defaultValue: true,
                description: "Enable or disable the sleep power multiplier.\n" +
                             "Set to 'true' to activate the multiplier, or 'false' to disable it.\n" +
                             "睡眠中のパワー回復倍率を有効または無効にします。\n" +
                             "'true' にすると倍率が有効になり、'false' にすると無効になります。");

            SleepPowerMultiplier = config.Bind(
                section: ModInfo.Name,
                key: "Sleep Power Multiplier",
                defaultValue: 1,
                description: "Multiplier for the power value during sleep. Must be a whole number.\n" +
                             "This multiplier affects the following:\n" +
                             "- HP healing\n" +
                             "- Stamina recovery\n" +
                             "- Mana restoration\n" +
                             "睡眠中のパワー値に乗じる倍率を設定します（整数のみ）。\n" +
                             "これには以下の効果があります:\n" +
                             "- HP回復\n" +
                             "- スタミナ回復\n" +
                             "- マナ回復");

            EnableSleepDuringMeditate = config.Bind(
                section: ModInfo.Name,
                key: "Enable Sleep During Meditate",
                defaultValue: false,
                description: "Enable or disable sleep during Meditate.\n" +
                             "Set to 'true' to allow Better Sleep mod behavior during Meditate, or 'false' to use default behavior.\n" +
                             "瞑想中に睡眠を有効または無効にします。\n" +
                             "'true' にするとベター・スリープの動作が瞑想中でも有効になり、'false' にすると通常の睡眠動作になります。");
        }
    }
}
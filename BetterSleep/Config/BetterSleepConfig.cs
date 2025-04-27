using BepInEx.Configuration;
using UnityEngine;
using System.IO;

namespace BetterSleep
{
    internal static class BetterSleepConfig
    {
        internal static ConfigEntry<bool> EnableCanSleep;
        internal static ConfigEntry<int> SleepHours;
        internal static ConfigEntry<bool> EnableAutoSave;
        internal static ConfigEntry<KeyCode> IncreaseSleepHoursKey;
        internal static ConfigEntry<KeyCode> DecreaseSleepHoursKey;
        internal static ConfigEntry<bool> EnableSleepPowerMultiplier;
        internal static ConfigEntry<int> SleepPowerMultiplier;
        internal static ConfigEntry<bool> EnableOnlyUnlearnedRecipes;
        internal static ConfigEntry<bool> EnableCanSleepDuringMeditate;
        internal static ConfigEntry<bool> EnableSleepDelay;
        internal static ConfigEntry<int> SleepDelayTurns;
        
        public static string XmlPath { get; private set; }
        public static string TranslationXlsxPath { get; private set; }
        
        internal static void LoadConfig(ConfigFile config)
        {
            EnableCanSleep = config.Bind(
                section: ModInfo.Name,
                key: "Enable Can Sleep",
                defaultValue: true,
                description: "Enable or disable the ability for the player character to sleep anytime.\n" +
                             "Set to 'true' to allow sleeping anytime, or 'false' to use the default sleep behavior.\n" +
                             "プレイヤーキャラクターがいつでも眠れるようにするかどうかを有効または無効にします。\n" +
                             "'true' に設定するといつでも眠ることができ、'false' に設定するとデフォルトの睡眠挙動が使用されます。\n" +
                             "启用或禁用玩家角色随时睡觉的能力。\n" +
                             "设置为 'true' 即允许随时睡觉，设置为 'false' 即恢复默认睡眠行为。"
            );

            SleepHours = config.Bind(
                section: ModInfo.Name,
                key: "Sleep Hours",
                defaultValue: 6,
                description: "Set the number of hours the player character sleeps during sleep events.\n" +
                             "You can adjust this value in-game using the configured keys.\n" +
                             "睡眠イベント中にプレイヤーキャラクターが眠る時間を設定します。\n" +
                             "ゲーム内で設定したキーを使ってこの値を調整できます。\n" +
                             "设置玩家角色在睡眠事件中睡觉的小时数。\n" +
                             "可以在游戏中使用配置的按键调整此值。"
            );
            
            IncreaseSleepHoursKey = config.Bind(
                section: ModInfo.Name,
                key: "Increase Sleep Hours Key",
                defaultValue: KeyCode.Equals,
                description: "Set the key to increase the sleep hours during gameplay.\n" +
                             "ゲームプレイ中に睡眠時間を増やすためのキーを設定します。\n" +
                             "设置在游戏中增加睡眠时间的按键。"
            );

            DecreaseSleepHoursKey = config.Bind(
                section: ModInfo.Name,
                key: "Decrease Sleep Hours Key",
                defaultValue: KeyCode.Minus,
                description: "Set the key to decrease the sleep hours during gameplay.\n" +
                             "ゲームプレイ中に睡眠時間を減らすためのキーを設定します。\n" +
                             "设置在游戏中减少睡眠时间的按键。"
            );

            EnableAutoSave = config.Bind(
                section: ModInfo.Name,
                key: "Enable Auto Save",
                defaultValue: true,
                description: "Set whether the game automatically saves during sleep events.\n" +
                             "Set to 'true' to enable autosaving during sleep, or 'false' to disable this behavior.\n" +
                             "睡眠イベント中にゲームが自動的に保存されるかどうかを設定します。\n" +
                             "'true' に設定すると睡眠中に自動保存が有効になり、'false' に設定するとこの機能が無効になります。\n" +
                             "设置游戏在睡眠事件期间是否自动保存。\n" +
                             "设置为 'true' 即在睡眠时启用自动保存，设置为 'false' 即禁用此行为。"
            );

            EnableSleepPowerMultiplier = config.Bind(
                section: ModInfo.Name,
                key: "Enable Sleep Power Multiplier",
                defaultValue: false,
                description: "Enable or disable the sleep power multiplier.\n" +
                             "Set to 'true' to apply the multiplier during sleep, or 'false' to use default recovery values.\n" +
                             "睡眠中のパワー回復倍率を有効または無効にします。\n" +
                             "'true' に設定すると睡眠中に倍率が適用され、'false' に設定するとデフォルトの回復値が使用されます。\n" +
                             "启用或禁用睡眠力量倍率。\n" +
                             "设置为 'true' 可在睡眠期间应用倍率，设置为 'false' 则使用默认恢复值。"
            );

            SleepPowerMultiplier = config.Bind(
                section: ModInfo.Name,
                key: "Sleep Power Multiplier",
                defaultValue: 1,
                description: "Adjust the power multiplier applied during sleep. Must be a whole number.\n" +
                             "This multiplier affects:\n" +
                             "- HP healing\n" +
                             "- Stamina recovery\n" +
                             "- Mana restoration\n" +
                             "睡眠中に適用されるパワー回復倍率を設定します（整数のみ）。\n" +
                             "この倍率は以下に影響します:\n" +
                             "- HP回復\n" +
                             "- スタミナ回復\n" +
                             "- マナ回復\n" +
                             "调整睡眠期间的力量恢复倍率（必须为整数）。\n" +
                             "此倍率会影响以下内容：\n" +
                             "- HP恢复\n" +
                             "- 耐力恢复\n" +
                             "- 法力恢复。"
            );
            
            EnableOnlyUnlearnedRecipes = config.Bind(
                section: ModInfo.Name,
                key: "Enable Only Unlearned Recipes",
                defaultValue: false,
                description: "Prioritize unlearned recipes when the player sleeps.\n" +
                             "Set to 'true' to prioritize unlearned recipes, or 'false' to allow learned recipes.\n" +
                             "プレイヤーが眠るときに未習得のレシピを優先します。\n" +
                             "'true' に設定すると未習得のレシピが優先され、'false' に設定すると習得済みのレシピも選択されます。\n" +
                             "玩家在睡觉时优先选择未学习的配方。\n" +
                             "设置为 'true' 时优先未学习的配方，设置为 'false' 时允许已学习的配方。"
            );
            
            EnableCanSleepDuringMeditate = config.Bind(
                section: ModInfo.Name,
                key: "Enable Can Sleep During Meditate",
                defaultValue: false,
                description: "Enable or disable the ability to sleep anytime during Meditate.\n" +
                             "Set to 'true' to allow sleeping anytime during Meditate, or 'false' to disable it.\n" +
                             "瞑想中にいつでも睡眠できるようにするかどうかを有効または無効にします。\n" +
                             "'true' に設定すると瞑想中でもいつでも睡眠できるようになり、'false' に設定すると無効になります。\n" +
                             "启用或禁用在冥想时随时睡觉的能力。\n" +
                             "设置为 'true' 即可在冥想时随时睡觉，设置为 'false' 则禁用此功能。"
            );
            
            EnableSleepDelay = config.Bind(
                section: ModInfo.Name,
                key: "Enable Sleep Delay",
                defaultValue: false,
                description: "Enable or disable custom sleep delay turns before the player fully sleeps.\n" +
                             "Set to 'true' to control how many turns it takes to fall asleep after starting sleep.\n" +
                             "プレイヤーが眠り始めた後、完全に眠るまでのターン数をカスタマイズできるかどうかを設定します。\n" +
                             "'true' に設定すると、眠るまでのターン数を制御できます。\n" +
                             "启用或禁用玩家完全入睡前的自定义延迟回合数。\n" +
                             "设置为 'true' 后可控制入睡所需的回合数。"
            );

            SleepDelayTurns = config.Bind(
                section: ModInfo.Name,
                key: "Sleep Delay Turns",
                defaultValue: 15,
                description: "Set the number of turns it takes before the player character fully falls asleep.\n" +
                             "Minimum 1. This affects how fast sleep triggers after starting.\n" +
                             "プレイヤーキャラクターが完全に眠るまでに必要なターン数を設定します（最小1）。\n" +
                             "睡眠開始後、眠りに入るまでの速度に影響します。\n" +
                             "设置玩家角色完全入睡所需的回合数（最小1）。\n" +
                             "影响开始睡觉后入睡的速度。"
            );
        }
        
        public static void InitializeXmlPath(string xmlPath)
        {
            if (File.Exists(path: xmlPath))
            {
                XmlPath = xmlPath;
            }
            else
            {
                XmlPath = string.Empty;
            }
        }
        
        public static void InitializeTranslationXlsxPath(string xlsxPath)
        {
            if (File.Exists(path: xlsxPath))
            {
                TranslationXlsxPath = xlsxPath;
            }
            else
            {
                TranslationXlsxPath = string.Empty;
            }
        }
    }
}
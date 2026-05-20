using System.IO;
using BepInEx.Configuration;
using UnityEngine;

namespace BetterSleep;

internal static class BetterSleepConfig
{
    internal const int MinimumSleepHours = 0;
    internal const int MaximumSleepHours = 12;
    internal const int MinimumSleepPowerMultiplier = 1;
    internal const int MaximumSleepPowerMultiplier = 10;
    internal const int MinimumSleepDelayTurns = 1;
    internal const int MaximumSleepDelayTurns = 15;

    internal static ConfigEntry<bool> EnableCanSleep = null!;
    internal static ConfigEntry<int> SleepHours = null!;
    internal static ConfigEntry<bool> EnableAutoSave = null!;
    internal static ConfigEntry<bool> EnableSleepSimulation = null!;
    internal static ConfigEntry<KeyCode> IncreaseSleepHoursKey = null!;
    internal static ConfigEntry<KeyCode> DecreaseSleepHoursKey = null!;
    internal static ConfigEntry<bool> EnableSleepPowerMultiplier = null!;
    internal static ConfigEntry<int> SleepPowerMultiplier = null!;
    internal static ConfigEntry<bool> EnableOnlyUnlearnedRecipes = null!;
    internal static ConfigEntry<bool> EnableCanSleepDuringMeditate = null!;
    internal static ConfigEntry<bool> EnableSleepDelay = null!;
    internal static ConfigEntry<int> SleepDelayTurns = null!;

    internal static string XmlPath { get; private set; } = string.Empty;
    internal static string TranslationXlsxPath { get; private set; } = string.Empty;

    internal static void LoadConfig(ConfigFile config)
    {
        EnableCanSleep = config.Bind(
            section: ModInfo.Name,
            key: "Enable Can Sleep",
            defaultValue: true,
            description: "Enable or disable the ability for the player character to sleep anytime outside Rest.\n" +
                         "Rest sleep is controlled separately by the Sleep During Rest setting.\n" +
                         "休憩中以外でプレイヤーキャラクターがいつでも眠れるようにするかどうかを有効または無効にします。\n" +
                         "休憩中の睡眠は「休憩中の睡眠」設定で別途設定します。\n" +
                         "启用或禁用玩家角色在休息以外随时睡觉的能力。\n" +
                         "休息时睡觉由“休息时睡觉”设置单独控制。"
        );

        SleepHours = config.Bind(
            section: ModInfo.Name,
            key: "Sleep Hours",
            defaultValue: 6,
            description: "Set the number of hours the player character sleeps during sleep events.\n" +
                         "Must be a whole number from 0 to 12.\n" +
                         "You can adjust this value in-game using the configured keys.\n" +
                         "睡眠イベント中にプレイヤーキャラクターが眠る時間を設定します。\n" +
                         "0から12までの整数を指定してください。\n" +
                         "ゲーム内で設定したキーを使ってこの値を調整できます。\n" +
                         "设置玩家角色在睡眠事件中睡觉的小时数。\n" +
                         "必须是 0 到 12 之间的整数。\n" +
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

        EnableSleepSimulation = config.Bind(
            section: ModInfo.Name,
            key: "Enable Sleep Simulation",
            defaultValue: true,
            description: "Set whether sleep advances time and runs normal sleep simulation.\n" +
                         "Set to 'true' to keep the normal black screen and time-passing sleep behavior, or 'false' to resolve sleep instantly without time simulation.\n" +
                         "睡眠時に時間を進め、通常の睡眠シミュレーションを実行するかどうかを設定します。\n" +
                         "'true' に設定すると通常の暗転と時間経過を伴う睡眠になります。'false' に設定すると時間シミュレーションなしで即座に睡眠が完了します。\n" +
                         "设置睡眠时是否推进时间并运行正常的睡眠模拟。\n" +
                         "设置为 'true' 时，保留正常的黑屏与时间流逝睡眠流程；设置为 'false' 时，不进行时间模拟而立即完成睡眠。"
        );

        EnableSleepPowerMultiplier = config.Bind(
            section: ModInfo.Name,
            key: "Enable Sleep Power Multiplier",
            defaultValue: false,
            description: "Enable or disable the sleep recovery multiplier.\n" +
                         "Set to 'true' to multiply recovery during sleep, or 'false' to use default recovery values.\n" +
                         "睡眠中の回復倍率を有効または無効にします。\n" +
                         "'true' に設定すると睡眠中の回復量に倍率が適用され、'false' に設定するとデフォルトの回復値が使用されます。\n" +
                         "启用或禁用睡眠恢复倍率。\n" +
                         "设置为 'true' 可在睡眠期间应用倍率，设置为 'false' 则使用默认恢复值。"
        );

        SleepPowerMultiplier = config.Bind(
            section: ModInfo.Name,
            key: "Sleep Power Multiplier",
            defaultValue: 1,
            description: "Adjust the recovery multiplier applied during sleep. Must be a whole number from 1 to 10.\n" +
                         "This multiplier affects:\n" +
                         "- HP healing\n" +
                         "- Stamina recovery\n" +
                         "- Mana restoration\n" +
                         "睡眠中に適用される回復倍率を設定します。1から10までの整数を指定してください。\n" +
                         "この倍率は以下に影響します:\n" +
                         "- HP回復\n" +
                         "- スタミナ回復\n" +
                         "- マナ回復\n" +
                         "调整睡眠期间的恢复倍率。必须是 1 到 10 之间的整数。\n" +
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
            description: "Enable or disable sleep from the Rest ability.\n" +
                         "Set to 'true' to allow Rest to trigger sleep, or 'false' to prevent sleeping while using Rest.\n" +
                         "休憩アビリティから睡眠できるようにするかどうかを有効または無効にします。\n" +
                         "'true' に設定すると休憩で睡眠でき、'false' に設定すると休憩中は睡眠しなくなります。\n" +
                         "启用或禁用通过休息能力睡觉。\n" +
                         "设置为 'true' 即允许休息触发睡觉，设置为 'false' 即在休息时不会睡觉。"
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
                         "Must be a whole number from 1 to 15. This affects how fast sleep triggers after starting.\n" +
                         "プレイヤーキャラクターが完全に眠るまでに必要なターン数を設定します。1から15までの整数を指定してください。\n" +
                         "睡眠開始後、眠りに入るまでの速度に影響します。\n" +
                         "设置玩家角色完全入睡所需的回合数。必须是 1 到 15 之间的整数。\n" +
                         "影响开始睡觉后入睡的速度。"
        );
    }

    internal static void InitializeXmlPath(string xmlPath)
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

    internal static void InitializeTranslationXlsxPath(string xlsxPath)
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

    internal static int GetEffectiveSleepHours()
    {
        int sleepHours = SleepHours?.Value ?? 6;
        return ClampSleepHours(sleepHours: sleepHours);
    }

    internal static int SetSleepHours(int sleepHours)
    {
        int clampedSleepHours = ClampSleepHours(sleepHours: sleepHours);
        SleepHours.Value = clampedSleepHours;
        return clampedSleepHours;
    }

    internal static int GetEffectiveSleepPowerMultiplier()
    {
        int sleepPowerMultiplier = SleepPowerMultiplier?.Value ?? MinimumSleepPowerMultiplier;
        return ClampSleepPowerMultiplier(sleepPowerMultiplier: sleepPowerMultiplier);
    }

    internal static int SetSleepPowerMultiplier(int sleepPowerMultiplier)
    {
        int clampedSleepPowerMultiplier = ClampSleepPowerMultiplier(sleepPowerMultiplier: sleepPowerMultiplier);
        SleepPowerMultiplier.Value = clampedSleepPowerMultiplier;
        return clampedSleepPowerMultiplier;
    }

    internal static int GetEffectiveSleepDelayTurns()
    {
        int sleepDelayTurns = SleepDelayTurns?.Value ?? MaximumSleepDelayTurns;
        return ClampSleepDelayTurns(sleepDelayTurns: sleepDelayTurns);
    }

    internal static int SetSleepDelayTurns(int sleepDelayTurns)
    {
        int clampedSleepDelayTurns = ClampSleepDelayTurns(sleepDelayTurns: sleepDelayTurns);
        SleepDelayTurns.Value = clampedSleepDelayTurns;
        return clampedSleepDelayTurns;
    }

    internal static int ClampSleepHours(int sleepHours)
    {
        if (sleepHours < MinimumSleepHours)
        {
            return MinimumSleepHours;
        }

        if (sleepHours > MaximumSleepHours)
        {
            return MaximumSleepHours;
        }

        return sleepHours;
    }

    internal static int ClampSleepPowerMultiplier(int sleepPowerMultiplier)
    {
        if (sleepPowerMultiplier < MinimumSleepPowerMultiplier)
        {
            return MinimumSleepPowerMultiplier;
        }

        if (sleepPowerMultiplier > MaximumSleepPowerMultiplier)
        {
            return MaximumSleepPowerMultiplier;
        }

        return sleepPowerMultiplier;
    }

    internal static int ClampSleepDelayTurns(int sleepDelayTurns)
    {
        if (sleepDelayTurns < MinimumSleepDelayTurns)
        {
            return MinimumSleepDelayTurns;
        }

        if (sleepDelayTurns > MaximumSleepDelayTurns)
        {
            return MaximumSleepDelayTurns;
        }

        return sleepDelayTurns;
    }
}

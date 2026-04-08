using System;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx.Configuration;
using EvilMask.Elin.ModOptions;
using EvilMask.Elin.ModOptions.UI;
using UnityEngine;

namespace BetterSleep;

public static class UIController
{
    public static void RegisterUI()
    {
        var controller = ModOptionController.Register(guid: ModInfo.Guid, tooptipId: "mod.tooltip");
        if (controller == null)
        {
            BetterSleep.LogError(message: "Failed to register Mod Options controller.");
            return;
        }

        var assemblyLocation = Path.GetDirectoryName(path: Assembly.GetExecutingAssembly().Location) ?? string.Empty;
        var xmlPath = Path.Combine(path1: assemblyLocation, path2: "BetterSleepConfig.xml");
        var xlsxPath = Path.Combine(path1: assemblyLocation, path2: "translations.xlsx");

        BetterSleepConfig.InitializeXmlPath(xmlPath: xmlPath);
        BetterSleepConfig.InitializeTranslationXlsxPath(xlsxPath: xlsxPath);

        if (File.Exists(path: BetterSleepConfig.XmlPath))
        {
            controller.SetPreBuildWithXml(xml: File.ReadAllText(path: BetterSleepConfig.XmlPath));
        }
        else
        {
            BetterSleep.LogError(message: $"Mod Options XML not found: {xmlPath}");
        }

        if (File.Exists(path: BetterSleepConfig.TranslationXlsxPath))
        {
            controller.SetTranslationsFromXslx(path: BetterSleepConfig.TranslationXlsxPath);
        }
        else
        {
            BetterSleep.LogError(message: $"Mod Options translations not found: {xlsxPath}");
        }

        RegisterEvents(controller: controller);
    }

    private static void RegisterEvents(ModOptionController controller)
    {
        controller.OnBuildUI += builder =>
        {
            var enableCanSleepToggle = GetRequiredPreBuild<OptToggle>(builder: builder, id: "enableCanSleepToggle");
            if (enableCanSleepToggle != null)
            {
                enableCanSleepToggle.Checked = BetterSleepConfig.EnableCanSleep.Value;
                enableCanSleepToggle.OnValueChanged += isChecked =>
                {
                    BetterSleepConfig.EnableCanSleep.Value = isChecked;
                };
            }

            var sleepHoursSlider = GetRequiredPreBuild<OptSlider>(builder: builder, id: "sleepHoursSlider");
            if (sleepHoursSlider != null)
            {
                var originalTitle = sleepHoursSlider.Title;
                sleepHoursSlider.Title = $"{BetterSleepConfig.SleepHours.Value} {originalTitle}";
                sleepHoursSlider.Value = BetterSleepConfig.SleepHours.Value;
                sleepHoursSlider.Step = 1;
                sleepHoursSlider.OnValueChanged += value =>
                {
                    BetterSleepConfig.SleepHours.Value = (int)value;
                    sleepHoursSlider.Title = $"{BetterSleepConfig.SleepHours.Value} {originalTitle}";
                };
            }

            var enableAutoSaveToggle = GetRequiredPreBuild<OptToggle>(builder: builder, id: "enableAutoSaveToggle");
            if (enableAutoSaveToggle != null)
            {
                enableAutoSaveToggle.Checked = BetterSleepConfig.EnableAutoSave.Value;
                enableAutoSaveToggle.OnValueChanged += isChecked =>
                {
                    BetterSleepConfig.EnableAutoSave.Value = isChecked;
                };
            }

            var enableSleepSimulationToggle = GetRequiredPreBuild<OptToggle>(builder: builder, id: "enableSleepSimulationToggle");
            if (enableSleepSimulationToggle != null)
            {
                enableSleepSimulationToggle.Checked = BetterSleepConfig.EnableSleepSimulation.Value;
                enableSleepSimulationToggle.OnValueChanged += isChecked =>
                {
                    BetterSleepConfig.EnableSleepSimulation.Value = isChecked;
                };
            }

            var enableOnlyUnlearnedRecipesToggle = GetRequiredPreBuild<OptToggle>(builder: builder, id: "enableOnlyUnlearnedRecipesToggle");
            if (enableOnlyUnlearnedRecipesToggle != null)
            {
                enableOnlyUnlearnedRecipesToggle.Checked = BetterSleepConfig.EnableOnlyUnlearnedRecipes.Value;
                enableOnlyUnlearnedRecipesToggle.OnValueChanged += isChecked =>
                {
                    BetterSleepConfig.EnableOnlyUnlearnedRecipes.Value = isChecked;
                };
            }

            var enableCanSleepDuringMeditateToggle = GetRequiredPreBuild<OptToggle>(builder: builder, id: "enableCanSleepDuringMeditateToggle");
            if (enableCanSleepDuringMeditateToggle != null)
            {
                enableCanSleepDuringMeditateToggle.Checked = BetterSleepConfig.EnableCanSleepDuringMeditate.Value;
                enableCanSleepDuringMeditateToggle.OnValueChanged += isChecked =>
                {
                    BetterSleepConfig.EnableCanSleepDuringMeditate.Value = isChecked;
                };
            }

            for (int i = 1; i <= 2; i++)
            {
                string textId = $"text{i:D2}";

                var text = builder.GetPreBuild<OptLabel>(id: textId);
                if (text != null)
                {
                    text.Align = TextAnchor.LowerLeft;
                }
            }

            var increaseSleepHoursDropdown = GetRequiredPreBuild<OptDropdown>(builder: builder, id: "dropdown01");
            if (increaseSleepHoursDropdown != null)
            {
                SetupKeyCodeDropdown(dropdown: increaseSleepHoursDropdown, configEntry: BetterSleepConfig.IncreaseSleepHoursKey);
            }

            var decreaseSleepHoursDropdown = GetRequiredPreBuild<OptDropdown>(builder: builder, id: "dropdown02");
            if (decreaseSleepHoursDropdown != null)
            {
                SetupKeyCodeDropdown(dropdown: decreaseSleepHoursDropdown, configEntry: BetterSleepConfig.DecreaseSleepHoursKey);
            }

            var enableSleepPowerMultiplierToggle = GetRequiredPreBuild<OptToggle>(builder: builder, id: "enableSleepPowerMultiplierToggle");
            if (enableSleepPowerMultiplierToggle != null)
            {
                enableSleepPowerMultiplierToggle.Checked = BetterSleepConfig.EnableSleepPowerMultiplier.Value;
                enableSleepPowerMultiplierToggle.OnValueChanged += isChecked =>
                {
                    BetterSleepConfig.EnableSleepPowerMultiplier.Value = isChecked;
                };
            }

            var sleepPowerMultiplierSlider = GetRequiredPreBuild<OptSlider>(builder: builder, id: "sleepPowerMultiplierSlider");
            if (sleepPowerMultiplierSlider != null)
            {
                sleepPowerMultiplierSlider.Title = BetterSleepConfig.SleepPowerMultiplier.Value.ToString();
                sleepPowerMultiplierSlider.Value = BetterSleepConfig.SleepPowerMultiplier.Value;
                sleepPowerMultiplierSlider.Step = 1;
                sleepPowerMultiplierSlider.OnValueChanged += value =>
                {
                    sleepPowerMultiplierSlider.Title = value.ToString();
                    BetterSleepConfig.SleepPowerMultiplier.Value = (int)value;
                };
            }

            var enableSleepDelayToggle = GetRequiredPreBuild<OptToggle>(builder: builder, id: "enableSleepDelayToggle");
            if (enableSleepDelayToggle != null)
            {
                enableSleepDelayToggle.Checked = BetterSleepConfig.EnableSleepDelay.Value;
                enableSleepDelayToggle.OnValueChanged += isChecked =>
                {
                    BetterSleepConfig.EnableSleepDelay.Value = isChecked;
                };
            }

            var sleepDelayTurnsSlider = GetRequiredPreBuild<OptSlider>(builder: builder, id: "sleepDelayTurnsSlider");
            if (sleepDelayTurnsSlider != null)
            {
                var sleepDelayTurnsSliderTitle = sleepDelayTurnsSlider.Title;
                sleepDelayTurnsSlider.Title = $"{BetterSleepConfig.SleepDelayTurns.Value} {sleepDelayTurnsSliderTitle}";
                sleepDelayTurnsSlider.Value = BetterSleepConfig.SleepDelayTurns.Value;
                sleepDelayTurnsSlider.Step = 1;
                sleepDelayTurnsSlider.OnValueChanged += value =>
                {
                    BetterSleepConfig.SleepDelayTurns.Value = (int)value;
                    sleepDelayTurnsSlider.Title = $"{BetterSleepConfig.SleepDelayTurns.Value} {sleepDelayTurnsSliderTitle}";
                };
            }
        };
    }

    private static void SetupKeyCodeDropdown(OptDropdown dropdown, ConfigEntry<KeyCode> configEntry)
    {
        dropdown.Base.options.Clear();

        var keyCodes = Enum.GetValues(enumType: typeof(KeyCode)).Cast<KeyCode>().ToList();

        foreach (KeyCode key in keyCodes)
        {
            dropdown.Base.options.Add(item: new UnityEngine.UI.Dropdown.OptionData(text: key.ToString()));
        }

        int currentIndex = keyCodes.IndexOf(item: configEntry.Value);
        dropdown.Value = currentIndex >= 0 ? currentIndex : 0;

        dropdown.OnValueChanged += index =>
        {
            if (index < 0 || index >= keyCodes.Count)
            {
                return;
            }

            configEntry.Value = keyCodes[index: index];
        };

        dropdown.Base.RefreshShownValue();
    }

    private static T? GetRequiredPreBuild<T>(OptionUIBuilder builder, string id) where T : OptUIElement
    {
        T? element = builder.GetPreBuild<T>(id: id);
        if (element == null)
        {
            BetterSleep.LogError(message: $"Missing Mod Options prebuilt element: {id}");
        }

        return element;
    }
}

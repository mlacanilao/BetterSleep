using System;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using EvilMask.Elin.ModOptions;
using EvilMask.Elin.ModOptions.UI;
using UnityEngine;

namespace BetterSleep.UI
{
    public class UIController
    {
        public static void RegisterUI()
        {
            foreach (var obj in ModManager.ListPluginObject)
            {
                if (obj is BaseUnityPlugin plugin && plugin.Info.Metadata.GUID == ModInfo.ModOptionsGuid)
                {
                    var controller = ModOptionController.Register(guid: ModInfo.Guid, tooptipId: "mod.tooltip");
                    
                    var assemblyLocation = Path.GetDirectoryName(path: Assembly.GetExecutingAssembly().Location);
                    var xmlPath = Path.Combine(path1: assemblyLocation, path2: "BetterSleepConfig.xml");
                    BetterSleepConfig.InitializeXmlPath(xmlPath: xmlPath);
            
                    var xlsxPath = Path.Combine(path1: assemblyLocation, path2: "translations.xlsx");
                    BetterSleepConfig.InitializeTranslationXlsxPath(xlsxPath: xlsxPath);
                    
                    if (File.Exists(path: BetterSleepConfig.XmlPath))
                    {
                        using (StreamReader sr = new StreamReader(path: BetterSleepConfig.XmlPath))
                            controller.SetPreBuildWithXml(xml: sr.ReadToEnd());
                    }
                    
                    if (File.Exists(path: BetterSleepConfig.TranslationXlsxPath))
                    {
                        controller.SetTranslationsFromXslx(path: BetterSleepConfig.TranslationXlsxPath);
                    }
                    
                    SetTranslations(controller: controller);
                    RegisterEvents(controller: controller);
                }
            }
        }

        private static void SetTranslations(ModOptionController controller)
        {
            
        }

        private static void RegisterEvents(ModOptionController controller)
        {
            controller.OnBuildUI += builder =>
            {
                var enableCanSleepToggle = builder.GetPreBuild<OptToggle>(id: "enableCanSleepToggle");
                enableCanSleepToggle.Checked = BetterSleepConfig.EnableCanSleep.Value;
                enableCanSleepToggle.OnValueChanged += isChecked =>
                {
                    BetterSleepConfig.EnableCanSleep.Value = isChecked;
                };
                
                var sleepHoursSlider = builder.GetPreBuild<OptSlider>(id: "sleepHoursSlider");
                var originalTitle = sleepHoursSlider.Title;
                sleepHoursSlider.Title = $"{BetterSleepConfig.SleepHours.Value} {originalTitle}";
                sleepHoursSlider.Value = BetterSleepConfig.SleepHours.Value;
                sleepHoursSlider.Step = 1;
                sleepHoursSlider.OnValueChanged += value =>
                {
                    BetterSleepConfig.SleepHours.Value = (int)value;
                    sleepHoursSlider.Title = $"{BetterSleepConfig.SleepHours.Value} {originalTitle}";
                };
                
                var enableAutoSaveToggle = builder.GetPreBuild<OptToggle>(id: "enableAutoSaveToggle");
                enableAutoSaveToggle.Checked = BetterSleepConfig.EnableAutoSave.Value;
                enableAutoSaveToggle.OnValueChanged += isChecked =>
                {
                    BetterSleepConfig.EnableAutoSave.Value = isChecked;
                };
                
                var enableOnlyUnlearnedRecipesToggle = builder.GetPreBuild<OptToggle>(id: "enableOnlyUnlearnedRecipesToggle");
                enableOnlyUnlearnedRecipesToggle.Checked = BetterSleepConfig.EnableOnlyUnlearnedRecipes.Value;
                enableOnlyUnlearnedRecipesToggle.OnValueChanged += isChecked =>
                {
                    BetterSleepConfig.EnableOnlyUnlearnedRecipes.Value = isChecked;
                };
                
                var enableCanSleepDuringMeditateToggle = builder.GetPreBuild<OptToggle>(id: "enableCanSleepDuringMeditateToggle");
                enableCanSleepDuringMeditateToggle.Checked = BetterSleepConfig.EnableCanSleepDuringMeditate.Value;
                enableCanSleepDuringMeditateToggle.OnValueChanged += isChecked =>
                {
                    BetterSleepConfig.EnableCanSleepDuringMeditate.Value = isChecked;
                };
                
                for (int i = 1; i <= 2; i++)
                {
                    string textId = $"text{i:D2}";
            
                    var text = builder.GetPreBuild<OptLabel>(id: textId);
                    if (text != null)
                    {
                        text.Align = TextAnchor.LowerLeft;
                    }
                }
                
                var dropdown01 = builder.GetPreBuild<OptDropdown>(id: "dropdown01");
                SetupKeyCodeDropdown(dropdown: dropdown01, configEntry: BetterSleepConfig.IncreaseSleepHoursKey);

                var dropdown02 = builder.GetPreBuild<OptDropdown>(id: "dropdown02");
                SetupKeyCodeDropdown(dropdown: dropdown02, configEntry: BetterSleepConfig.DecreaseSleepHoursKey);
                
                var enableSleepPowerMultiplierToggle = builder.GetPreBuild<OptToggle>(id: "enableSleepPowerMultiplierToggle");
                enableSleepPowerMultiplierToggle.Checked = BetterSleepConfig.EnableSleepPowerMultiplier.Value;
                enableSleepPowerMultiplierToggle.OnValueChanged += isChecked =>
                {
                    BetterSleepConfig.EnableSleepPowerMultiplier.Value = isChecked;
                };
                
                var sleepPowerMultiplierSlider = builder.GetPreBuild<OptSlider>(id: "sleepPowerMultiplierSlider");
                sleepPowerMultiplierSlider.Title = BetterSleepConfig.SleepPowerMultiplier.Value.ToString();
                sleepPowerMultiplierSlider.Value = BetterSleepConfig.SleepPowerMultiplier.Value;
                sleepPowerMultiplierSlider.Step = 1;
                sleepPowerMultiplierSlider.OnValueChanged += value =>
                {
                    sleepPowerMultiplierSlider.Title = value.ToString();
                    BetterSleepConfig.SleepPowerMultiplier.Value = (int)value;
                };
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

            int currentIndex = keyCodes.IndexOf(item: configEntry?.Value ?? KeyCode.None);
            dropdown.Value = currentIndex >= 0 ? currentIndex : 0;

            dropdown.OnValueChanged += index =>
            {
                configEntry.Value = keyCodes[index: index];
            };

            dropdown.Base.RefreshShownValue();
        }
    }
}
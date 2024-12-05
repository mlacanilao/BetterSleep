using System;
using System.Collections.Generic;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.UI;

namespace BetterSleep
{
    public class BetterSleepWidgetMain : OmegaWidget
    {
        public override OmegaWidget Setup(object dummy)
        {
            Window window = this.AddWindow(setting: new Window.Setting
            {
                textCaption = OmegaUI.__(ja: "Better Sleep Config", en: "Better Sleep Config"),
                bound = new Rect(x: 0f, y: 0f, width: 680f, height: 500f),
                transparent = false,
                allowMove = true
            });

            try
            {
                window.AddTab(
                    idLang: OmegaUI.__(ja: "Better Sleep Config", en: "Better Sleep Config"),
                    content: OmegaUI.CreatePage<BetterSleepWidgetMain.ConfigUI>(
                        id: "bettersleep.config",
                        window: window).root,
                    action: null,
                    sprite: null,
                    langTooltip: null);
            }
            catch (Exception ex)
            {
                Debug.Log(message: $"[Better Sleep] {ex.Message}");
            }

            return this;
        }

        public class ConfigUI : OmegaLayout<object>
        {
            private ScrollLayout scrollLayout;
            public override void OnCreate(object dummy)
            {
                this.layout.childControlHeight = true;
                this.layout.childForceExpandHeight = true;

                base.AddText(
                    text: OmegaUI.__(ja: "ベター・スリープMODを有効または無効にします。", 
                                     en: "Enable or disable Better Sleep mod."),
                    parent: this.layout.transform
                );
                
                base.AddToggle(
                    text: OmegaUI.__(ja: "Enable Better Sleep Mod", en: "Enable Better Sleep Mod"),
                    isOn: BetterSleepConfig.EnableBetterSleepMod.Value,
                    action: isOn =>
                    {
                        BetterSleepConfig.EnableBetterSleepMod.Value = isOn;
                        string status = isOn ? "enabled" : "disabled";
                        ELayer.pc.TalkRaw(text: $"Better Sleep mod is now {status}.", ref1: null, ref2: null, forceSync: false);
                        ToggleFeatures(isEnabled: isOn);
                    },
                    parent: this.layout.transform
                );
                
                scrollLayout = AddScrollLayout(parent: this.layout.transform);
                scrollLayout.headerRect.SetActive(enable: true);
                scrollLayout.uiHeader.SetText(s: "Features");
                scrollLayout.layout.spacing = 10f;
                
                // Add the toggle and dropdown for Can Sleep
                AddToggleWithDropdown(
                    parent: scrollLayout.root,
                    toggleName: "Enable Can Sleep",
                    jaDescription: "カスタムの「眠れるかどうか」の挙動を有効または無効にします。\n" +
                                   "プレイヤーキャラクターがいつでも眠れるかどうかを設定します。\n" +
                                   "'true' にすると常に眠ることができ、'false' にすると完全に眠れなくなります。",
                    enDescription: "Enable or disable the custom 'Can Sleep' behavior feature.\n" +
                                   "Control whether the player character is allowed to sleep.\n" +
                                   "Set to 'true' to allow sleeping anytime, or 'false' to disable sleeping completely.",
                    toggleConfig: BetterSleepConfig.EnableCanSleep,
                    dropdownConfig: BetterSleepConfig.CanSleep,
                    jaDropdownLabel: "Can Sleep",
                    enDropdownLabel: "Can Sleep"
                );

                AddToggleWithInputTextField(
                    parent: scrollLayout.root,
                    toggleName: "Enable Sleep Hours",
                    inputLabel: "Sleep Hours",
                    jaDescription: "カスタム睡眠時間の設定を有効または無効にします。\n" +
                                   "睡眠イベントでの睡眠時間を設定します。\n" +
                                   "値は 0 ～ 12 の間で設定してください。",
                    enDescription: "Enable or disable setting custom sleep hours.\n" +
                                   "Input field to set custom sleep hours.\n" +
                                   "Value must be between 0 and 12.",
                    toggleConfig: BetterSleepConfig.EnableSleepHours,
                    inputConfig: BetterSleepConfig.SleepHours,
                    minValue: 0,
                    maxValue: 12
                );

                AddToggleWithDropdown(
                    parent: scrollLayout.root,
                    toggleName: "Enable Ignore Auto Save",
                    jaDescription: "「オートセーブを無視」を有効または無効にします。\n" +
                                   "'true' にすると睡眠中にオートセーブを無視し、'false' にすると常にオートセーブされます。",
                    enDescription: "Enable or disable the 'Ignore Auto Save' feature.\n" +
                                   "Set to 'true' to allow ignoring autosaves during sleep, or 'false' to always autosave.",
                    toggleConfig: BetterSleepConfig.EnableIgnoreAutoSave,
                    dropdownConfig: BetterSleepConfig.IgnoreAutoSave,
                    jaDropdownLabel: "Ignore Auto Save",
                    enDropdownLabel: "Ignore Auto Save"
                );

                AddToggleWithInputTextField(
                    parent: scrollLayout.root,
                    toggleName: "Enable Sleep Power Multiplier",
                    inputLabel: "Sleep Power Multiplier",
                    jaDescription: "睡眠中のパワー回復倍率を有効または無効にします。\n" +
                                   "睡眠中のパワー回復倍率を設定するための入力フィールドです。\n" +
                                   "これには以下の効果があります:\n" +
                                   "- HP回復\n" +
                                   "- スタミナ回復\n" +
                                   "- マナ回復",
                    enDescription: "Enable or disable the sleep power multiplier.\n" +
                                   "Input field to set the sleep power multiplier.\n" +
                                   "This multiplier affects the following:\n" +
                                   "- HP healing\n" +
                                   "- Stamina recovery\n" +
                                   "- Mana restoration\n",
                    toggleConfig: BetterSleepConfig.EnableSleepPowerMultiplier,
                    inputConfig: BetterSleepConfig.SleepPowerMultiplier,
                    minValue: 0,
                    maxValue: 100
                );

                AddEnableToggleWithDescription(parent: scrollLayout.root, 
                    toggleName: "Enable Sleep During Meditate", 
                    jaDescription: "瞑想中に睡眠を有効または無効にします。\n" +
                                   "'true' にするとベター・スリープの動作が瞑想中でも有効になり、'false' にすると通常の睡眠動作になります。",
                    enDescription: "Enable or disable sleep during Meditate feature.\n" +
                                   "Set to 'true' to allow Better Sleep mod behavior during Meditate, or 'false' to use default behavior.",
                    configEntry: BetterSleepConfig.EnableSleepDuringMeditate);

                ToggleFeatures(isEnabled: BetterSleepConfig.EnableBetterSleepMod.Value);
            }

            private void AddEnableToggleWithDescription(Transform parent, string toggleName, string jaDescription, string enDescription, ConfigEntry<bool> configEntry)
            {
                base.AddText(
                    text: OmegaUI.__(ja: jaDescription, en: enDescription),
                    parent: parent
                );

                base.AddToggle(
                    text: OmegaUI.__(ja: toggleName, en: toggleName),
                    isOn: configEntry.Value,
                    action: isOn =>
                    {
                        configEntry.Value = isOn;
                        string status = isOn ? "enabled" : "disabled";
                        ELayer.pc.TalkRaw(text: $"{toggleName} is now {status}.", ref1: null, ref2: null, forceSync: false);
                    },
                    parent: parent
                );
            }
            
            private void AddToggleWithDropdown(Transform parent, string toggleName, string jaDescription, string enDescription, ConfigEntry<bool> toggleConfig, ConfigEntry<bool> dropdownConfig, string jaDropdownLabel, string enDropdownLabel)
            {
                base.AddText(
                    text: OmegaUI.__(ja: jaDescription, en: enDescription),
                    parent: parent
                );

                OmegaLayout<object>.LayoutGroup layoutGroup = null;
                CanvasGroup canvasGroup = null;
                
                base.AddToggle(
                    text: OmegaUI.__(ja: toggleName, en: toggleName),
                    isOn: toggleConfig.Value,
                    action: isOn =>
                    {
                        toggleConfig.Value = isOn;
                        string status = isOn ? "enabled" : "disabled";
                        ELayer.pc.TalkRaw(text: $"{toggleName} is now {status}.", ref1: null, ref2: null, forceSync: false);
                        ToggleLayout(isEnabled: isOn);
                    },
                    parent: parent
                );
                
                // Create a layout group for the dropdown (initially hidden)
                layoutGroup = base.AddLayoutGroup(parent: parent);
                layoutGroup.group.childControlWidth = false;
                layoutGroup.group.childForceExpandWidth = false;
                
                canvasGroup = layoutGroup.ui.gameObject.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = layoutGroup.ui.gameObject.AddComponent<CanvasGroup>();
                }
                canvasGroup.interactable = toggleConfig.Value;
                canvasGroup.blocksRaycasts = toggleConfig.Value;
                canvasGroup.alpha = toggleConfig.Value ? 1f : 0.5f;

                // Add the dropdown description
                base.AddText(
                    text: OmegaUI.__(ja: jaDropdownLabel, en: enDropdownLabel),
                    parent: layoutGroup.transform
                );

                // Add the dropdown
                UIDropdown uidropdown = base.AddDropdown(parent: layoutGroup.transform);
                uidropdown.gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(axis: RectTransform.Axis.Horizontal, size: 150f);

                // Configure dropdown options
                uidropdown.ClearOptions();
                uidropdown.options.Add(item: new Dropdown.OptionData(text: "True"));
                uidropdown.options.Add(item: new Dropdown.OptionData(text: "False"));

                // Set initial value based on the dropdown config
                uidropdown.value = dropdownConfig.Value ? 0 : 1;

                // Add listener to update the dropdown config
                uidropdown.onValueChanged.AddListener(call: delegate (int i)
                {
                    dropdownConfig.Value = i == 0; // True if index is 0, False if index is 1
                    ELayer.pc.TalkRaw(
                        text: $"{enDropdownLabel} is now {(dropdownConfig.Value ? "enabled" : "disabled")}.",
                        ref1: null, ref2: null, forceSync: false
                    );
                });
                
                void ToggleLayout(bool isEnabled)
                {
                    if (layoutGroup != null)
                    {
                        if (canvasGroup != null)
                        {
                            canvasGroup.interactable = isEnabled;
                            canvasGroup.blocksRaycasts = isEnabled;
                            canvasGroup.alpha = isEnabled ? 1f : 0.5f;
                        }
                    }
                }
            }
            
            private void AddToggleWithInputTextField(Transform parent, string toggleName, string inputLabel, string jaDescription, string enDescription, ConfigEntry<bool> toggleConfig, ConfigEntry<int> inputConfig, int minValue, int maxValue)
            {
                // Add the description for the toggle and input field
                base.AddText(
                    text: OmegaUI.__(ja: jaDescription, en: enDescription),
                    parent: parent
                );

                OmegaLayout<object>.LayoutGroup layoutGroup = null;
                CanvasGroup canvasGroup = null;

                // Add the toggle
                base.AddToggle(
                    text: OmegaUI.__(ja: toggleName, en: toggleName),
                    isOn: toggleConfig.Value,
                    action: isOn =>
                    {
                        toggleConfig.Value = isOn;
                        string status = isOn ? "enabled" : "disabled";
                        ELayer.pc.TalkRaw(text: $"{toggleName} is now {status}.", ref1: null, ref2: null, forceSync: false);
                        ToggleLayout(isEnabled: isOn);
                    },
                    parent: parent
                );

                // Create a layout group for the input field (initially hidden)
                layoutGroup = base.AddLayoutGroup(parent: parent);
                layoutGroup.group.childControlWidth = false;
                layoutGroup.group.childForceExpandWidth = false;
                
                canvasGroup = layoutGroup.ui.gameObject.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = layoutGroup.ui.gameObject.AddComponent<CanvasGroup>();
                }
                canvasGroup.interactable = toggleConfig.Value;
                canvasGroup.blocksRaycasts = toggleConfig.Value;
                canvasGroup.alpha = toggleConfig.Value ? 1f : 0.5f;

                // Add the input label
                base.AddText(
                    text: OmegaUI.__(ja: inputLabel, en: inputLabel),
                    parent: layoutGroup.transform
                );

                // Add the input text field
                var inputTextField = base.AddInputText(parent: layoutGroup.group.transform);
                inputTextField.input.Num = inputConfig.Value; // Set initial value
                inputTextField.transform.SetSizeWithCurrentAnchors(axis: RectTransform.Axis.Horizontal, size: 100f);
                inputTextField.inputTransform.SetSizeWithCurrentAnchors(axis: RectTransform.Axis.Horizontal, size: 100f);
                inputTextField.placeholder.SetActive(enable: true);
                inputTextField.placeholderText.text = OmegaUI.__(ja: "上書き", en: "modify");

                // Add value change listener with clamping
                inputTextField.input.onValueChanged = value =>
                {
                    int clampedValue = Mathf.Clamp(value: value, min: minValue, max: maxValue); // Clamp the value between min and max
                    inputConfig.Value = clampedValue;
                    inputTextField.input.Num = clampedValue; // Update the input field with the clamped value
                    ELayer.pc.TalkRaw(
                        text: $"{inputLabel} set to {clampedValue}.",
                        ref1: null, ref2: null, forceSync: false
                    );
                };

                void ToggleLayout(bool isEnabled)
                {
                    if (layoutGroup != null)
                    {
                        if (canvasGroup != null)
                        {
                            canvasGroup.interactable = isEnabled;
                            canvasGroup.blocksRaycasts = isEnabled;
                            canvasGroup.alpha = isEnabled ? 1f : 0.5f;
                        }
                    }
                }
            }
            
            private void ToggleFeatures(bool isEnabled)
            {
                if (scrollLayout != null)
                {
                    var canvasGroup = scrollLayout.root.gameObject.GetComponent<CanvasGroup>();
                    if (canvasGroup == null)
                    {
                        canvasGroup = scrollLayout.root.gameObject.AddComponent<CanvasGroup>();
                    }
                    canvasGroup.interactable = isEnabled;
                    canvasGroup.blocksRaycasts = isEnabled;
                    canvasGroup.alpha = isEnabled ? 1f : 0.5f;
                }
            }
        }
    }
}
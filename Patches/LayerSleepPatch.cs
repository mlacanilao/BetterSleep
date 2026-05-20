using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace BetterSleep.Patches;

internal static class LayerSleepPatch
{
    private static readonly AccessTools.FieldRef<LayerSleep, int> MinField =
        AccessTools.FieldRefAccess<LayerSleep, int>(fieldName: "min");

    private static readonly AccessTools.FieldRef<LayerSleep, int> MaxMinField =
        AccessTools.FieldRefAccess<LayerSleep, int>(fieldName: "maxMin");

    private static readonly HashSet<LayerSleep> PendingInstantAdvanceLayers = new HashSet<LayerSleep>();

    public static IEnumerable<CodeInstruction> OnAfterInitTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        List<CodeInstruction> instructionList = new List<CodeInstruction>(collection: instructions);

        var codeMatcher = new CodeMatcher(instructions: instructionList);

        MethodInfo? showCover = AccessTools.Method(
            type: typeof(UI),
            name: nameof(UI.ShowCover),
            parameters: new[] { typeof(float), typeof(float), typeof(Action), typeof(Color) });
        MethodInfo? showSleepCover = AccessTools.Method(
            type: typeof(LayerSleepPatch),
            name: nameof(ShowSleepCover),
            parameters: new[] { typeof(UI), typeof(float), typeof(float), typeof(Action), typeof(Color) });

        if (showCover == null || showSleepCover == null)
        {
            BetterSleep.LogError(message: "LayerSleep.OnAfterInit transpiler: cover methods not found");
            return codeMatcher.Instructions();
        }

        int showCoverMatchCount = CountMethodCalls(instructions: instructionList, method: showCover);
        if (showCoverMatchCount != 1)
        {
            BetterSleep.LogError(
                message: "LayerSleep.OnAfterInit transpiler expected exactly 1 ShowCover call but found " +
                    showCoverMatchCount.ToString());
            return codeMatcher.Instructions();
        }

        codeMatcher.MatchStartForward(matches: new[]
        {
            new CodeMatch(opcode: OpCodes.Callvirt, operand: showCover)
        });

        if (codeMatcher.IsValid == false)
        {
            BetterSleep.LogError(message: "LayerSleep.OnAfterInit transpiler: ShowCover call not matched");
            return codeMatcher.Instructions();
        }

        BetterSleep.LogDebug(message: "LayerSleep.OnAfterInit transpiler: matched ShowCover call");
        SetInstructionPreservingMetadata(
            codeMatcher: codeMatcher,
            replacement: new CodeInstruction(opcode: OpCodes.Call, operand: showSleepCover));

        return codeMatcher.Instructions();
    }

    public static IEnumerable<CodeInstruction> SleepTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        List<CodeInstruction> instructionList = new List<CodeInstruction>(collection: instructions);

        var codeMatcher = new CodeMatcher(instructions: instructionList);

        MethodInfo? invokeRepeating = AccessTools.Method(
            type: typeof(MonoBehaviour),
            name: nameof(MonoBehaviour.InvokeRepeating),
            parameters: new[] { typeof(string), typeof(float), typeof(float) });
        MethodInfo? beginSleepAdvance = AccessTools.Method(
            type: typeof(LayerSleepPatch),
            name: nameof(BeginSleepAdvance),
            parameters: new[] { typeof(LayerSleep), typeof(string), typeof(float), typeof(float) });
        FieldInfo? repeatRateField = AccessTools.Field(
            type: typeof(LayerSleep),
            name: nameof(LayerSleep.repeatRate));

        if (invokeRepeating == null ||
            beginSleepAdvance == null ||
            repeatRateField == null)
        {
            BetterSleep.LogError(message: "LayerSleep.Sleep transpiler: InvokeRepeating methods not found");
            return codeMatcher.Instructions();
        }

        int matchCount = CountSleepAdvanceSchedulers(
            instructions: instructionList,
            repeatRateField: repeatRateField,
            invokeRepeating: invokeRepeating);

        if (matchCount != 1)
        {
            BetterSleep.LogError(
                message: "LayerSleep.Sleep transpiler expected 1 sleep advance scheduler but found " +
                    matchCount.ToString() +
                    ". Sleep simulation timing will remain vanilla.");
            return codeMatcher.Instructions();
        }

        codeMatcher.MatchStartForward(matches: new[]
        {
            new CodeMatch(opcode: OpCodes.Ldarg_0),
            new CodeMatch(opcode: OpCodes.Ldstr, operand: "Advance"),
            new CodeMatch(opcode: OpCodes.Ldarg_0),
            new CodeMatch(predicate: instruction => LoadsField(instruction: instruction, field: repeatRateField)),
            new CodeMatch(opcode: OpCodes.Ldarg_0),
            new CodeMatch(predicate: instruction => LoadsField(instruction: instruction, field: repeatRateField)),
            new CodeMatch(predicate: instruction => CallsMethod(instruction: instruction, method: invokeRepeating))
        });

        if (codeMatcher.IsValid == false)
        {
            BetterSleep.LogError(message: "LayerSleep.Sleep transpiler: matched sleep advance scheduler was not replaced");
            return codeMatcher.Instructions();
        }

        codeMatcher.Advance(offset: 6);
        CodeInstruction originalInstruction = codeMatcher.Instruction;

        BetterSleep.LogDebug(message: "LayerSleep.Sleep transpiler: matched InvokeRepeating call");
        codeMatcher.SetInstruction(
            instruction: new CodeInstruction(opcode: OpCodes.Call, operand: beginSleepAdvance)
                .WithLabels(labels: originalInstruction.labels)
                .WithBlocks(blocks: originalInstruction.blocks));

        return codeMatcher.Instructions();
    }

    public static IEnumerable<CodeInstruction> AdvanceTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        List<CodeInstruction> instructionList = new List<CodeInstruction>(collection: instructions);

        var codeMatcher = new CodeMatcher(instructions: instructionList);

        MethodInfo? simulateFaction = AccessTools.Method(
            type: typeof(Player),
            name: nameof(Player.SimulateFaction));
        MethodInfo? simulateFactionMaybe = AccessTools.Method(
            type: typeof(LayerSleepPatch),
            name: nameof(SimulateFactionMaybe),
            parameters: new[] { typeof(Player) });
        MethodInfo? showCover = AccessTools.Method(
            type: typeof(UI),
            name: nameof(UI.ShowCover),
            parameters: new[] { typeof(float), typeof(float), typeof(Action), typeof(Color) });
        MethodInfo? showSleepCover = AccessTools.Method(
            type: typeof(LayerSleepPatch),
            name: nameof(ShowSleepCover),
            parameters: new[] { typeof(UI), typeof(float), typeof(float), typeof(Action), typeof(Color) });
        MethodInfo? tweenDelay = AccessTools.Method(
            type: typeof(TweenUtil),
            name: nameof(TweenUtil.Delay),
            parameters: new[] { typeof(float), typeof(Action) });
        MethodInfo? delaySleepClose = AccessTools.Method(
            type: typeof(LayerSleepPatch),
            name: nameof(DelaySleepClose),
            parameters: new[] { typeof(float), typeof(Action) });

        if (simulateFaction == null ||
            simulateFactionMaybe == null ||
            showCover == null ||
            showSleepCover == null ||
            tweenDelay == null ||
            delaySleepClose == null)
        {
            BetterSleep.LogError(message: "LayerSleep.Advance transpiler: required methods not found");
            return codeMatcher.Instructions();
        }

        bool replacedSimulateFaction = false;
        bool replacedShowCover = false;
        bool replacedDelay = false;
        int simulateFactionMatchCount = CountMethodCalls(instructions: instructionList, method: simulateFaction);
        int showCoverMatchCount = CountMethodCalls(instructions: instructionList, method: showCover);
        int delayMatchCount = CountMethodCalls(instructions: instructionList, method: tweenDelay);

        if (simulateFactionMatchCount != 1 ||
            showCoverMatchCount != 1 ||
            delayMatchCount != 1)
        {
            BetterSleep.LogError(
                message: "LayerSleep.Advance transpiler expected exactly 1 match for each target but found " +
                    $"SimulateFaction={simulateFactionMatchCount}, ShowCover={showCoverMatchCount}, Delay={delayMatchCount}");
            return codeMatcher.Instructions();
        }

        codeMatcher.MatchStartForward(matches: new[]
        {
            new CodeMatch(opcode: OpCodes.Callvirt, operand: simulateFaction)
        });

        if (codeMatcher.IsValid)
        {
            replacedSimulateFaction = true;
            SetInstructionPreservingMetadata(
                codeMatcher: codeMatcher,
                replacement: new CodeInstruction(opcode: OpCodes.Call, operand: simulateFactionMaybe));
        }

        codeMatcher.MatchStartForward(matches: new[]
        {
            new CodeMatch(opcode: OpCodes.Callvirt, operand: showCover)
        });

        if (codeMatcher.IsValid)
        {
            replacedShowCover = true;
            SetInstructionPreservingMetadata(
                codeMatcher: codeMatcher,
                replacement: new CodeInstruction(opcode: OpCodes.Call, operand: showSleepCover));
        }

        codeMatcher.MatchStartForward(matches: new[]
        {
            new CodeMatch(opcode: OpCodes.Call, operand: tweenDelay)
        });

        if (codeMatcher.IsValid)
        {
            replacedDelay = true;
            SetInstructionPreservingMetadata(
                codeMatcher: codeMatcher,
                replacement: new CodeInstruction(opcode: OpCodes.Call, operand: delaySleepClose));
        }

        BetterSleep.LogDebug(
            message: $"LayerSleep.Advance transpiler matches: SimulateFaction={replacedSimulateFaction}, ShowCover={replacedShowCover}, Delay={replacedDelay}");

        if (replacedSimulateFaction == false ||
            replacedShowCover == false ||
            replacedDelay == false)
        {
            BetterSleep.LogError(message: "LayerSleep.Advance transpiler: one or more target calls were not matched");
        }

        return codeMatcher.Instructions();
    }

    public static void AdvancePrefix(out bool __state)
    {
        __state = ELayer.debug.ignoreAutoSave;

        if (ELayer.core?.IsGameStarted != true)
        {
            return;
        }

        bool enableAutoSave = BetterSleepConfig.EnableAutoSave?.Value ?? true;

        if (enableAutoSave == false)
        {
            ELayer.debug.ignoreAutoSave = true;
            FeatureTestLog.Log(
                feature: "Auto Save During Sleep",
                detail: "set ELayer.debug.ignoreAutoSave=True; previous=" + __state.ToString());
        }
    }

    public static void AdvanceFinalizer(bool __state)
    {
        ELayer.debug.ignoreAutoSave = __state;

        bool enableAutoSave = BetterSleepConfig.EnableAutoSave?.Value ?? true;
        if (enableAutoSave == false)
        {
            FeatureTestLog.Log(
                feature: "Auto Save During Sleep",
                detail: "restored ELayer.debug.ignoreAutoSave=" + __state.ToString());
        }
    }

    public static void ShowSleepCover(UI ui, float duration, float dest, Action onComplete, Color color)
    {
        if (ShouldRunSleepSimulation() == false)
        {
            FeatureTestLog.LogOnce(
                feature: "Sleep Simulation",
                key: "cover-suppressed",
                detail: "suppressed sleep cover transition");
            return;
        }

        ui.ShowCover(duration, dest, onComplete, color);
    }

    public static void BeginSleepAdvance(LayerSleep layer, string methodName, float time, float repeatRate)
    {
        if (ShouldRunSleepSimulation())
        {
            if (MaxMinField(instance: layer) == 0)
            {
                MinField(instance: layer) = MaxMinField(instance: layer) + 1;
            }

            layer.InvokeRepeating(methodName, time, repeatRate);
            return;
        }

        MinField(instance: layer) = MaxMinField(instance: layer) + 1;
        PendingInstantAdvanceLayers.Add(item: layer);
    }

    public static void SleepPostfix(LayerSleep __instance)
    {
        if (PendingInstantAdvanceLayers.Remove(item: __instance) == false)
        {
            return;
        }

        FeatureTestLog.LogOnce(
            feature: "Sleep Simulation",
            key: "advance-immediate",
            detail: "advanced sleep immediately without repeated time simulation");
        __instance.Advance();
    }

    public static void SimulateFactionMaybe(Player player)
    {
        if (ShouldRunSleepSimulation() == false)
        {
            FeatureTestLog.LogOnce(
                feature: "Sleep Simulation",
                key: "faction-suppressed",
                detail: "suppressed Player.SimulateFaction during sleep");
            return;
        }

        player.SimulateFaction();
    }

    public static object? DelaySleepClose(float duration, Action action)
    {
        if (ShouldRunSleepSimulation())
        {
            return TweenUtil.Delay(duration, action);
        }

        FeatureTestLog.LogOnce(
            feature: "Sleep Simulation",
            key: "close-delay-suppressed",
            detail: "suppressed sleep close delay");

        if (TryCloseCapturedLayerSleep(action: action))
        {
            return null;
        }

        action?.Invoke();
        return null;
    }

    private static bool ShouldRunSleepSimulation()
    {
        return BetterSleepConfig.EnableSleepSimulation?.Value ?? true;
    }

    private static bool TryCloseCapturedLayerSleep(Action action)
    {
        object? target = action?.Target;
        if (target == null)
        {
            return false;
        }

        if (target is LayerSleep targetLayer)
        {
            targetLayer.Close();
            return true;
        }

        FieldInfo[] fields = target.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (FieldInfo field in fields)
        {
            if (typeof(LayerSleep).IsAssignableFrom(field.FieldType) == false)
            {
                continue;
            }

            if (field.GetValue(target) is LayerSleep layer)
            {
                layer.Close();
                return true;
            }
        }

        return false;
    }

    private static bool CallsMethod(CodeInstruction instruction, MethodInfo method)
    {
        return (instruction.opcode == OpCodes.Call || instruction.opcode == OpCodes.Callvirt) &&
               Equals(objA: instruction.operand, objB: method);
    }

    private static int CountMethodCalls(List<CodeInstruction> instructions, MethodInfo method)
    {
        int count = 0;

        foreach (CodeInstruction instruction in instructions)
        {
            if (CallsMethod(instruction: instruction, method: method))
            {
                count++;
            }
        }

        return count;
    }

    private static void SetInstructionPreservingMetadata(CodeMatcher codeMatcher, CodeInstruction replacement)
    {
        CodeInstruction originalInstruction = codeMatcher.Instruction;
        codeMatcher.SetInstruction(
            instruction: replacement
                .WithLabels(labels: originalInstruction.labels)
                .WithBlocks(blocks: originalInstruction.blocks));
    }

    private static int CountSleepAdvanceSchedulers(
        List<CodeInstruction> instructions,
        FieldInfo repeatRateField,
        MethodInfo invokeRepeating)
    {
        int count = 0;

        for (int i = 0; i < instructions.Count; i++)
        {
            if (IsSleepAdvanceSchedulerStart(
                instructions: instructions,
                startIndex: i,
                repeatRateField: repeatRateField,
                invokeRepeating: invokeRepeating))
            {
                count++;
            }
        }

        return count;
    }

    private static bool IsSleepAdvanceSchedulerStart(
        List<CodeInstruction> instructions,
        int startIndex,
        FieldInfo repeatRateField,
        MethodInfo invokeRepeating)
    {
        if (startIndex + 6 >= instructions.Count)
        {
            return false;
        }

        return instructions[index: startIndex].opcode == OpCodes.Ldarg_0 &&
            instructions[index: startIndex + 1].opcode == OpCodes.Ldstr &&
            instructions[index: startIndex + 1].operand is string methodName &&
            methodName == "Advance" &&
            instructions[index: startIndex + 2].opcode == OpCodes.Ldarg_0 &&
            LoadsField(instruction: instructions[index: startIndex + 3], field: repeatRateField) &&
            instructions[index: startIndex + 4].opcode == OpCodes.Ldarg_0 &&
            LoadsField(instruction: instructions[index: startIndex + 5], field: repeatRateField) &&
            CallsMethod(instruction: instructions[index: startIndex + 6], method: invokeRepeating);
    }

    private static bool LoadsField(CodeInstruction instruction, FieldInfo field)
    {
        return instruction.opcode == OpCodes.Ldfld &&
            Equals(objA: instruction.operand, objB: field);
    }

}

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

    public static IEnumerable<CodeInstruction> OnAfterInitTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        List<CodeInstruction> instructionList = new List<CodeInstruction>(collection: instructions);
        LogInstructions(context: "LayerSleep.OnAfterInit", instructions: instructionList);

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
        codeMatcher.SetInstruction(instruction: new CodeInstruction(opcode: OpCodes.Call, operand: showSleepCover));

        return codeMatcher.Instructions();
    }

    public static IEnumerable<CodeInstruction> SleepTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        List<CodeInstruction> instructionList = new List<CodeInstruction>(collection: instructions);
        LogInstructions(context: "LayerSleep.Sleep", instructions: instructionList);

        var codeMatcher = new CodeMatcher(instructions: instructionList);

        MethodInfo? invokeRepeating = AccessTools.Method(
            type: typeof(MonoBehaviour),
            name: nameof(MonoBehaviour.InvokeRepeating),
            parameters: new[] { typeof(string), typeof(float), typeof(float) });
        MethodInfo? beginSleepAdvance = AccessTools.Method(
            type: typeof(LayerSleepPatch),
            name: nameof(BeginSleepAdvance),
            parameters: new[] { typeof(LayerSleep), typeof(string), typeof(float), typeof(float) });

        if (invokeRepeating == null || beginSleepAdvance == null)
        {
            BetterSleep.LogError(message: "LayerSleep.Sleep transpiler: InvokeRepeating methods not found");
            return codeMatcher.Instructions();
        }

        codeMatcher.MatchStartForward(matches: new[]
        {
            new CodeMatch(predicate: instruction => CallsMethod(instruction: instruction, method: invokeRepeating))
        });

        if (codeMatcher.IsValid == false)
        {
            BetterSleep.LogError(message: "LayerSleep.Sleep transpiler: InvokeRepeating call not matched");
            return codeMatcher.Instructions();
        }

        BetterSleep.LogDebug(message: "LayerSleep.Sleep transpiler: matched InvokeRepeating call");
        codeMatcher.SetInstruction(instruction: new CodeInstruction(opcode: OpCodes.Call, operand: beginSleepAdvance));

        return codeMatcher.Instructions();
    }

    public static IEnumerable<CodeInstruction> AdvanceTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        List<CodeInstruction> instructionList = new List<CodeInstruction>(collection: instructions);
        LogInstructions(context: "LayerSleep.Advance", instructions: instructionList);

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

        codeMatcher.MatchStartForward(matches: new[]
        {
            new CodeMatch(opcode: OpCodes.Callvirt, operand: simulateFaction)
        });

        if (codeMatcher.IsValid)
        {
            replacedSimulateFaction = true;
            codeMatcher.SetInstruction(instruction: new CodeInstruction(opcode: OpCodes.Call, operand: simulateFactionMaybe));
        }

        codeMatcher.MatchStartForward(matches: new[]
        {
            new CodeMatch(opcode: OpCodes.Callvirt, operand: showCover)
        });

        if (codeMatcher.IsValid)
        {
            replacedShowCover = true;
            codeMatcher.SetInstruction(instruction: new CodeInstruction(opcode: OpCodes.Call, operand: showSleepCover));
        }

        codeMatcher.MatchStartForward(matches: new[]
        {
            new CodeMatch(opcode: OpCodes.Call, operand: tweenDelay)
        });

        if (codeMatcher.IsValid)
        {
            replacedDelay = true;
            codeMatcher.SetInstruction(instruction: new CodeInstruction(opcode: OpCodes.Call, operand: delaySleepClose));
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

        ELayer.debug.ignoreAutoSave = enableAutoSave == false;
    }

    public static void AdvanceFinalizer(bool __state)
    {
        ELayer.debug.ignoreAutoSave = __state;
    }

    public static void ShowSleepCover(UI ui, float duration, float dest, Action onComplete, Color color)
    {
        if (ShouldRunSleepSimulation() == false)
        {
            return;
        }

        ui.ShowCover(duration, dest, onComplete, color);
    }

    public static void BeginSleepAdvance(LayerSleep layer, string methodName, float time, float repeatRate)
    {
        if (ShouldRunSleepSimulation())
        {
            layer.InvokeRepeating(methodName, time, repeatRate);
            return;
        }

        MinField(instance: layer) = MaxMinField(instance: layer) + 1;
        layer.Advance();
    }

    public static void SimulateFactionMaybe(Player player)
    {
        if (ShouldRunSleepSimulation() == false)
        {
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

    private static void LogInstructions(string context, List<CodeInstruction> instructions)
    {
        BetterSleep.LogDebug(message: $"{context} transpiler instruction dump start");

        for (int i = 0; i < instructions.Count; i++)
        {
            CodeInstruction instruction = instructions[i];
            string operand = FormatOperand(operand: instruction.operand);
            BetterSleep.LogDebug(message: $"{context} [{i}] {instruction.opcode} {operand}");
        }

        BetterSleep.LogDebug(message: $"{context} transpiler instruction dump end");
    }

    private static bool CallsMethod(CodeInstruction instruction, MethodInfo method)
    {
        return (instruction.opcode == OpCodes.Call || instruction.opcode == OpCodes.Callvirt) &&
               Equals(objA: instruction.operand, objB: method);
    }

    private static string FormatOperand(object? operand)
    {
        if (operand == null)
        {
            return "<null>";
        }

        if (operand is MethodInfo methodInfo)
        {
            return $"{methodInfo.DeclaringType?.FullName}.{methodInfo.Name}";
        }

        if (operand is FieldInfo fieldInfo)
        {
            return $"{fieldInfo.DeclaringType?.FullName}.{fieldInfo.Name}";
        }

        return operand.ToString() ?? "<null>";
    }
}

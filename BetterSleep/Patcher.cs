using System;
using System.Collections.Generic;
using BetterSleep.Patches;
using HarmonyLib;

namespace BetterSleep;

internal static class Patcher
{
    [HarmonyPrefix]
    [HarmonyPatch(declaringType: typeof(Chara), methodName: nameof(Chara.CanSleep))]
    internal static bool CharaCanSleep(Chara __instance, ref bool __result)
    {
        return CharaPatch.CanSleepPrefix(__instance: __instance, __result: ref __result);
    }

    [HarmonyTranspiler]
    [HarmonyPatch(declaringType: typeof(Chara), methodName: nameof(Chara.OnSleep), argumentTypes: new[] { typeof(int), typeof(int), typeof(bool) })]
    internal static IEnumerable<CodeInstruction> CharaOnSleep(IEnumerable<CodeInstruction> instructions)
    {
        return CharaPatch.OnSleepTranspiler(instructions: instructions);
    }

    [HarmonyPostfix]
    [HarmonyPatch(declaringType: typeof(Chara), methodName: nameof(Chara.Sleep))]
    internal static void CharaSleep(Chara __instance)
    {
        CharaPatch.SleepPostfix(__instance: __instance);
    }

    [HarmonyTranspiler]
    [HarmonyPatch(declaringType: typeof(LayerSleep), methodName: nameof(LayerSleep.OnAfterInit))]
    internal static IEnumerable<CodeInstruction> LayerSleepOnAfterInit(IEnumerable<CodeInstruction> instructions)
    {
        return LayerSleepPatch.OnAfterInitTranspiler(instructions: instructions);
    }

    [HarmonyPrefix]
    [HarmonyPatch(declaringType: typeof(LayerSleep), methodName: nameof(LayerSleep.Sleep))]
    internal static void LayerSleepSleepPrefix(ref int _hours)
    {
        SleepPatch.SleepPrefix(_hours: ref _hours);
    }

    [HarmonyTranspiler]
    [HarmonyPatch(declaringType: typeof(LayerSleep), methodName: nameof(LayerSleep.Sleep))]
    internal static IEnumerable<CodeInstruction> LayerSleepSleep(IEnumerable<CodeInstruction> instructions)
    {
        return LayerSleepPatch.SleepTranspiler(instructions: instructions);
    }

    [HarmonyPostfix]
    [HarmonyPatch(declaringType: typeof(LayerSleep), methodName: nameof(LayerSleep.Sleep))]
    internal static void LayerSleepSleepPostfix(LayerSleep __instance)
    {
        LayerSleepPatch.SleepPostfix(__instance: __instance);
    }

    [HarmonyPrefix]
    [HarmonyPatch(declaringType: typeof(LayerSleep), methodName: nameof(LayerSleep.Advance))]
    internal static void LayerSleepAdvancePrefix(out bool __state)
    {
        LayerSleepPatch.AdvancePrefix(__state: out __state);
    }

    [HarmonyFinalizer]
    [HarmonyPatch(declaringType: typeof(LayerSleep), methodName: nameof(LayerSleep.Advance))]
    internal static Exception LayerSleepAdvanceFinalizer(Exception __exception, bool __state)
    {
        LayerSleepPatch.AdvanceFinalizer(__state: __state);
        return __exception;
    }

    [HarmonyTranspiler]
    [HarmonyPatch(declaringType: typeof(LayerSleep), methodName: nameof(LayerSleep.Advance))]
    internal static IEnumerable<CodeInstruction> LayerSleepAdvance(IEnumerable<CodeInstruction> instructions)
    {
        return LayerSleepPatch.AdvanceTranspiler(instructions: instructions);
    }

    [HarmonyTranspiler]
    [HarmonyPatch(declaringType: typeof(RecipeManager), methodName: nameof(RecipeManager.OnSleep))]
    internal static IEnumerable<CodeInstruction> RecipeManagerOnSleep(IEnumerable<CodeInstruction> instructions)
    {
        return RecipeManagerPatch.OnSleepTranspiler(instructions: instructions);
    }
}


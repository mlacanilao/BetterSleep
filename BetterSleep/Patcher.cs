using System;
using System.Collections.Generic;
using BetterSleep.Patches;
using HarmonyLib;

namespace BetterSleep;

internal static class Patcher
{
    [HarmonyPrefix]
    [HarmonyPatch(declaringType: typeof(Chara), methodName: nameof(Chara.CanSleep))]
    public static bool CharaCanSleep(Chara __instance, ref bool __result)
    {
        return CharaPatch.CanSleepPrefix(__instance: __instance, __result: ref __result);
    }

    [HarmonyPrefix]
    [HarmonyPatch(declaringType: typeof(Chara), methodName: nameof(Chara.OnSleep), argumentTypes: new[] { typeof(int), typeof(int), typeof(bool) })]
    public static void CharaOnSleep(ref int power)
    {
        CharaPatch.OnSleepPrefix(power: ref power);
    }

    [HarmonyPostfix]
    [HarmonyPatch(declaringType: typeof(Chara), methodName: nameof(Chara.Sleep))]
    public static void CharaSleep(Chara __instance)
    {
        CharaPatch.SleepPostfix(__instance: __instance);
    }

    [HarmonyTranspiler]
    [HarmonyPatch(declaringType: typeof(LayerSleep), methodName: nameof(LayerSleep.OnAfterInit))]
    public static IEnumerable<CodeInstruction> LayerSleepOnAfterInit(IEnumerable<CodeInstruction> instructions)
    {
        return LayerSleepPatch.OnAfterInitTranspiler(instructions: instructions);
    }

    [HarmonyPrefix]
    [HarmonyPatch(declaringType: typeof(LayerSleep), methodName: nameof(LayerSleep.Sleep))]
    public static void LayerSleepSleepPrefix(ref int _hours)
    {
        SleepPatch.SleepPrefix(_hours: ref _hours);
    }

    [HarmonyTranspiler]
    [HarmonyPatch(declaringType: typeof(LayerSleep), methodName: nameof(LayerSleep.Sleep))]
    public static IEnumerable<CodeInstruction> LayerSleepSleep(IEnumerable<CodeInstruction> instructions)
    {
        return LayerSleepPatch.SleepTranspiler(instructions: instructions);
    }

    [HarmonyPrefix]
    [HarmonyPatch(declaringType: typeof(LayerSleep), methodName: nameof(LayerSleep.Advance))]
    public static void LayerSleepAdvancePrefix(out bool __state)
    {
        LayerSleepPatch.AdvancePrefix(__state: out __state);
    }

    [HarmonyFinalizer]
    [HarmonyPatch(declaringType: typeof(LayerSleep), methodName: nameof(LayerSleep.Advance))]
    public static Exception LayerSleepAdvanceFinalizer(Exception __exception, bool __state)
    {
        LayerSleepPatch.AdvanceFinalizer(__state: __state);
        return __exception;
    }

    [HarmonyTranspiler]
    [HarmonyPatch(declaringType: typeof(LayerSleep), methodName: nameof(LayerSleep.Advance))]
    public static IEnumerable<CodeInstruction> LayerSleepAdvance(IEnumerable<CodeInstruction> instructions)
    {
        return LayerSleepPatch.AdvanceTranspiler(instructions: instructions);
    }

    [HarmonyPrefix]
    [HarmonyPatch(declaringType: typeof(RecipeManager), methodName: nameof(RecipeManager.GetRandomRecipe))]
    public static void RecipeManagerGetRandomRecipe(ref bool onlyUnlearned)
    {
        RecipeManagerPatch.GetRandomRecipePrefix(onlyUnlearned: ref onlyUnlearned);
    }
}

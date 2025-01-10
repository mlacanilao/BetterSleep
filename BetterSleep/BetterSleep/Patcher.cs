using BetterSleep.Patches;
using HarmonyLib;

namespace BetterSleep
{
    [HarmonyPatch]
    public class Patcher
    {
        [HarmonyPrefix]
        [HarmonyPatch(declaringType: typeof(Chara), methodName: nameof(Chara.CanSleep))]
        public static bool CharaCanSleep(Chara __instance, ref bool __result)
        {
            return CharaPatch.CanSleepPrefix(__instance: __instance, __result: ref __result);
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(declaringType: typeof(LayerSleep), methodName: nameof(LayerSleep.Sleep))]
        public static void LayerSleepSleep(ref int _hours)
        {
            SleepPatch.SleepPrefix(_hours: ref _hours);
        }

        [HarmonyPrefix]
        [HarmonyPatch(declaringType: typeof(Chara), methodName: nameof(Chara.OnSleep), argumentTypes: new[] { typeof(int), typeof(int) })]
        public static void CharaOnSleep(ref int power, int days)
        {
            CharaPatch.OnSleepPrefix(power: ref power, days: days);
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(declaringType: typeof(LayerSleep), methodName: nameof(LayerSleep.Advance))]
        public static void LayerSleepAdvance()
        {
            LayerSleepPatch.AdvancePrefix();
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(declaringType: typeof(RecipeManager), methodName: nameof(RecipeManager.GetRandomRecipe))]
        public static void RecipeManagerGetRandomRecipe(ref bool onlyUnlearned)
        {
            RecipeManagerPatch.GetRandomRecipePrefix(onlyUnlearned: ref onlyUnlearned);
        }
    }
}
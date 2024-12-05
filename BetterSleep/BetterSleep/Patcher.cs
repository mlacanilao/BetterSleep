using BetterSleep.Patches;
using HarmonyLib;

namespace BetterSleep
{
    [HarmonyPatch]
    public class Patcher
    {
        [HarmonyPrefix]
        [HarmonyPatch(declaringType: typeof(Chara), methodName: nameof(Chara.CanSleep))]
        public static bool CanSleep(Chara __instance, ref bool __result)
        {
            return CharaPatch.CanSleep(__instance: __instance, __result: ref __result);
        }

        [HarmonyPrefix]
        [HarmonyPatch(declaringType: typeof(Chara), methodName: nameof(Chara.OnSleep), argumentTypes: new[] { typeof(int), typeof(int) })]
        public static void OnSleep(ref int power, int days)
        {
            CharaPatch.OnSleep(power: ref power, days: days);
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(declaringType: typeof(LayerSleep), methodName: nameof(LayerSleep.Advance))]
        public static void Advance()
        {
            LayerSleepPatch.Advance();
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(declaringType: typeof(LayerSleep), methodName: nameof(LayerSleep.Sleep))]
        public static void Sleep(ref int _hours)
        {
            SleepPatch.Sleep(_hours: ref _hours);
        }
        
        [HarmonyPostfix]
        [HarmonyPatch(declaringType: typeof(UIContextMenuManager), methodName: nameof(UIContextMenuManager.Create))]
        public static void UIContextMenuManager_Create(UIContextMenuManager __instance, string menuName = "ContextMenu", bool destroyOnHide = true)
        {
            UIContextMenuManagerPatch.Create(__instance: __instance, menuName: menuName, destroyOnHide: destroyOnHide);
        }
    }
}
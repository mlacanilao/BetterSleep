using UnityEngine;

namespace BetterSleep.Patches
{
    public class UIContextMenuManagerPatch
    {
        public static void Create(UIContextMenuManager __instance, string menuName, bool destroyOnHide)
        {
            if (menuName == "ContextSystem")
            {
                __instance.currentMenu.AddButton(idLang: OmegaUI.__(ja: "Better Sleep 設定", en: "Better Sleep Config"), action: delegate()
                {
                    OmegaUI.OpenWidget<BetterSleepWidgetMain>();
                }, hideAfter: true);
                return;
            }
        }
    }
}
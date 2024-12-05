using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace BetterSleep
{
    public static class OmegaUI
    {
        static OmegaUI()
        {
            OmegaUI.UIElementsList = new Dictionary<Type, string>
            {
                {
                    typeof(Layer),
                    "Layers(Float)"
                },
                {
                    typeof(UIText),
                    "text caption"
                },
                {
                    typeof(UIButton),
                    "ButtonBottom Parchment"
                },
                {
                    typeof(UIScrollView),
                    "Scrollview default"
                },
                {
                    typeof(InputField),
                    "InputField"
                },
                {
                    typeof(ScrollRect),
                    "Scrollview parchment with Header"
                }
            };
            OmegaUI.UIObjects = OmegaUI.UIElementsList.ToDictionary(keySelector: (KeyValuePair<Type, string> k) => k.Key, elementSelector: (KeyValuePair<Type, string> v) => Resources.FindObjectsOfTypeAll(type: v.Key).FirstOrDefault(predicate: (UnityEngine.Object x) => x.name == v.Value));
            OmegaUI.BaseWindow = Resources.FindObjectsOfTypeAll<Window>().FirstOrDefault(predicate: (Window x) => x.name == "Window Parchment");
        }
        
        public static string __(string ja, string en = "")
        {
            if (en == null)
            {
                return ja;
            }
            if (!Lang.isJP)
            {
                return en;
            }
            return ja;
        }
        
        public static T Create<T>(Transform parent = null) where T : MonoBehaviour
        {
            GameObject gameObject = new GameObject(name: typeof(T).Name, components: new Type[]
            {
                typeof(RectTransform)
            });
            if (parent != null)
            {
                gameObject.transform.SetParent(p: parent);
            }
            return gameObject.AddComponent<T>();
        }
        
        public static void DestroyAllChildren(this Transform parent)
        {
            foreach (object obj in parent)
            {
                Transform transform = (Transform)obj;
                transform.gameObject.SetActive(value: false);
                UnityEngine.Object.Destroy(obj: transform.gameObject);
                transform.SetActive(enable: false);
                UnityEngine.Object.Destroy(obj: transform);
            }
        }
        
        public static TDerived ReplaceComponent<TBase, TDerived>(this TBase original) where TBase : MonoBehaviour where TDerived : MonoBehaviour
        {
            GameObject gameObject = original.gameObject;
            TDerived tderived = gameObject.AddComponent<TDerived>();
            foreach (FieldInfo fieldInfo in typeof(TBase).GetFields(bindingAttr: BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                fieldInfo.SetValue(obj: tderived, value: fieldInfo.GetValue(obj: original));
            }
            UnityEngine.Object.Destroy(obj: original);
            gameObject.name = typeof(TDerived).Name;
            gameObject.transform.DestroyChildren(destroyInactive: false, ignoreDestroy: true);
            return tderived;
        }
        
        public static T GetResource<T>() where T : UnityEngine.Object
        {
            return (T)((object)UnityEngine.Object.Instantiate(original: OmegaUI.UIObjects[key: typeof(T)]));
        }
        
        public static bool TryGetHeader(this Window window, out UIHeader header)
        {
            header = null;
            if (!window.rectHeader)
            {
                return false;
            }
            header = window.rectHeader.GetComponentsInChildren<UIHeader>(includeInactive: true).FirstOrDefault<UIHeader>();
            return true;
        }
        
        public static Transform FindMainContent(this Window window)
        {
            return window.Find(name: "Content View").Find(n: "Inner Simple Scroll").Find(n: "Scrollview default").Find(n: "Viewport").Find(n: "Content");
        }
        
        public static Window AddWindow(this Layer layer, Window.Setting setting)
        {
            if (setting.tabs == null)
            {
                setting.tabs = new List<Window.Setting.Tab>();
            }
            LayerList layerList = (LayerList)Layer.Create(path: typeof(LayerList).Name);
            Window window = layerList.windows.First<Window>();
            window.transform.SetParent(p: layer.transform);
            layer.windows.Add(item: window);
            UnityEngine.Object.Destroy(obj: layerList.gameObject);
            window.setting = setting;
            window.Init(_layer: layer);
            window.RectTransform.position = setting.bound.position;
            window.RectTransform.sizeDelta = setting.bound.size;
            window.GetType().GetMethod(name: "RecalculatePositionCaches", bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic).Invoke(obj: window, parameters: null);
            window.RebuildLayout(recursive: true);
            UIHeader c;
            if (window.TryGetHeader(header: out c))
            {
                c.SetActive(enable: false);
            }
            window.GetComponent<VerticalLayoutGroup>().enabled = false;
            Transform transform = window.Find(name: "Content View");
            transform.DestroyChildren(destroyInactive: false, ignoreDestroy: true);
            transform.DestroyAllChildren();
            transform.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(x: 0f, y: 0f);
            UnityEngine.Object.Destroy(obj: transform.GetComponent<HorizontalLayoutGroup>());
            return window;
        }
        
        public static T OpenWidget<T>() where T : OmegaWidget
        {
            foreach (LayerInventory layerInventory in LayerInventory.listInv.Copy<LayerInventory>())
            {
                if (layerInventory.IsPlayerContainer(true)) // Check if it's a player container
                {
                    // Close the layer
                    ELayer.ui.layerFloat.RemoveLayer(layerInventory);
                }
            }
            T t = EMono.ui.layers.Find(match: (Layer o) => o.GetType() == typeof(T)) as T;
            if (t != null)
            {
                t.SetActive(enable: true);
                return t;
            }
            T t2 = OmegaUI.GetResource<Layer>().ReplaceComponent<Layer, T>();
            t2.Setup(arg: 0);
            EMono.ui.AddLayer(l: t2);
            return t2;
        }
        
        public static T OpenWidget<T>(object arg) where T : OmegaWidget
        {
            T t = EMono.ui.layers.Find(match: (Layer o) => o.GetType() == typeof(T)) as T;
            if (t != null)
            {
                t.SetActive(enable: true);
                return t;
            }
            T t2 = OmegaUI.GetResource<Layer>().ReplaceComponent<Layer, T>();
            t2.Setup(arg: arg);
            EMono.ui.AddLayer(l: t2);
            return t2;
        }
        
        public static UIContent CreateBaseContent(Transform parent)
        {
            RectTransform component = parent.gameObject.GetComponent<RectTransform>();
            UIContent uicontent = OmegaUI.Create<UIContent>(parent: parent);
            RectTransform component2 = uicontent.GetComponent<RectTransform>();
            component2.SetAnchor(anchor: RectPosition.TopLEFT);
            component2.SetSizeWithCurrentAnchors(axis: RectTransform.Axis.Horizontal, size: component.rect.width);
            component2.SetSizeWithCurrentAnchors(axis: RectTransform.Axis.Vertical, size: component.rect.height);
            VerticalLayoutGroup verticalLayoutGroup = uicontent.gameObject.AddComponent<VerticalLayoutGroup>();
            verticalLayoutGroup.childControlHeight = false;
            verticalLayoutGroup.childForceExpandHeight = false;
            verticalLayoutGroup.padding = new RectOffset(left: 0, right: 0, top: 0, bottom: 0);
            verticalLayoutGroup.spacing = 5f;
            return uicontent;
        }
        
        public static T CreatePage<T, TArg>(string id, Window window, TArg arg) where T : OmegaLayout<TArg>, new()
        {
            Transform parent = ClassExtension.Find(c: window, name: "Content View");
            UIContent uicontent = OmegaUI.CreateBaseContent(parent: parent);
            uicontent.gameObject.name = id;
            uicontent.gameObject.GetComponent<RectTransform>();
            VerticalLayoutGroup component = uicontent.gameObject.GetComponent<VerticalLayoutGroup>();
            component.padding = new RectOffset(left: 10, right: 10, top: 40, bottom: 0);
            RectTransform component2 = component.gameObject.GetComponent<RectTransform>();
            T t = Activator.CreateInstance<T>();
            t.window = window;
            t.parent = parent;
            t.root = uicontent;
            t.layout = component;
            t.rect = component2;
            t.OnCreate(data: arg);
            ClassExtension.RebuildLayout(c: uicontent, recursive: true);
            return t;
        }
        
        public static T CreatePage<T>(string id, Window window) where T : OmegaLayout<object>, new()
        {
            return OmegaUI.CreatePage<T, object>(id: id, window: window, arg: 0);
        }
        
        private static Window BaseWindow;
        
        private static Dictionary<Type, UnityEngine.Object> UIObjects;
        
        private static Dictionary<Type, string> UIElementsList;
    }
}
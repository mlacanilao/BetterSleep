using System;
using UnityEngine;
using UnityEngine.UI;

namespace BetterSleep
{
    public class OmegaLayout<T>
    {
		public virtual void OnCreate(T data)
		{
		}

		public UIItem AddTopic(string text, string value = "", Transform parent = null)
		{
			UIItem uiitem = this.root.AddTopic(text: text, value: value);
			uiitem.transform.SetParent(p: parent ?? this.layout.transform);
			return uiitem;
		}

		public UIItem AddText(string text, Transform parent = null)
		{
			UIItem uiitem = this.root.AddText(text: text, color: FontColor.DontChange);
			uiitem.transform.SetParent(p: parent ?? this.layout.transform);
			LayoutElement component = uiitem.gameObject.GetComponent<LayoutElement>();
			UIText component2 = uiitem.gameObject.GetComponent<UIText>();
			component.minWidth = ((component2.preferredWidth < 80f) ? 80f : component2.preferredWidth);
			uiitem.gameObject.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
			return uiitem;
		}

		public UIButton AddToggle(string text, bool isOn, Action<bool> action, Transform parent = null)
		{
			UIButton uibutton = this.root.AddToggle(idLang: text, isOn: isOn, action: action);
			uibutton.transform.SetParent(p: parent ?? this.layout.transform);
			uibutton.gameObject.AddComponent<LayoutElement>();
			return uibutton;
		}

		public UIDropdown AddDropdown(Transform parent = null)
		{
			Transform transform = Util.Instantiate(path: "UI/Element/Input/DropdownDefault", parent: parent ?? this.layout.transform);
			Text component = transform.Find(n: "Label").GetComponent<Text>();
			component.horizontalOverflow = HorizontalWrapMode.Wrap;
			component.verticalOverflow = VerticalWrapMode.Truncate;
			return transform.GetComponent<UIDropdown>();
		}

		public UIButton AddButton(string text, Action action, Transform parent = null)
		{
			UIButton uibutton = this.root.AddButton(text: text, onClick: action);
			uibutton.transform.SetParent(p: parent ?? this.layout.transform);
			return uibutton;
		}

		public OmegaLayout<T>.InputTextField AddInputText(Transform parent = null)
		{
			return new OmegaLayout<T>.InputTextField(parent: parent ?? this.layout.transform);
		}

		public TLayout AddItem<TLayout>(Transform parent = null) where TLayout : MonoBehaviour
		{
			GameObject gameObject = new GameObject(name: typeof(TLayout).Name, components: new Type[]
			{
				typeof(RectTransform)
			});
			gameObject.transform.SetParent(p: this.root.gameObject.transform);
			TLayout tlayout = gameObject.AddComponent<TLayout>();
			tlayout.transform.SetParent(p: parent ?? this.layout.transform);
			return tlayout;
		}

		public RectTransform AddSpace(int sizeY = 0, int sizeX = 1, Transform parent = null)
		{
			RectTransform rectTransform = Util.Instantiate<Transform>(path: "UI/Element/Deco/Space", parent: parent ?? this.layout.transform).Rect();
			rectTransform.sizeDelta = new Vector2(x: (float)sizeX, y: (float)sizeY);
			if (sizeY != 1)
			{
				rectTransform.GetComponent<LayoutElement>().preferredHeight = (float)sizeY;
			}
			if (sizeX != 1)
			{
				rectTransform.GetComponent<LayoutElement>().preferredWidth = (float)sizeX;
			}
			return rectTransform;
		}
		
		public OmegaLayout<T>.GridLayout AddGridLayout(Transform parent)
		{
			return new OmegaLayout<T>.GridLayout(parent: parent ?? this.layout.transform);
		}
		
		public OmegaLayout<T>.LayoutGroup AddLayoutGroup(Transform parent)
		{
			return new OmegaLayout<T>.LayoutGroup(parent: parent ?? this.layout.transform);
		}
		
		public OmegaLayout<T>.ScrollLayout AddScrollLayout(Transform parent)
		{
			return new OmegaLayout<T>.ScrollLayout(parent: parent);
		}

		public Window window;

		public Transform parent;

		public UIContent root;

		public VerticalLayoutGroup layout;

		public RectTransform rect;

		public class GridLayout
		{
			public GridLayout(Transform parent)
			{
				this.ui = OmegaUI.Create<UIContent>(parent: parent);
				this.transform = this.ui.GetComponent<RectTransform>();
				this.transform.SetAnchor(anchor: RectPosition.TopLEFT);
				this.transform.SetSizeWithCurrentAnchors(axis: RectTransform.Axis.Horizontal, size: 0f);
				this.group = this.ui.gameObject.AddComponent<GridLayoutGroup>();
				this.group.padding = new RectOffset(left: 0, right: 0, top: 0, bottom: 15);
				this.items = this.ui.gameObject.AddComponent<UIItemList>();
				this.items.gridLayout.cellSize = new Vector2(x: 180f, y: 50f);
				this.items.gridLayout.spacing = new Vector2(x: 2f, y: 2f);
				this.items.gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
				this.items.gridLayout.constraintCount = 3;
			}

			public UIContent ui;

			public RectTransform transform;

			public GridLayoutGroup group;

			public UIItemList items;
		}

		public class LayoutGroup
		{
			public LayoutGroup(Transform parent)
			{
				this.ui = OmegaUI.Create<UIContent>(parent: parent);
				this.element = this.ui.gameObject.AddComponent<LayoutElement>();
				this.element.preferredHeight = 36f;
				this.transform = this.ui.GetComponent<RectTransform>();
				this.transform.SetAnchor(anchor: RectPosition.TopLEFT);
				this.transform.SetSizeWithCurrentAnchors(axis: RectTransform.Axis.Horizontal, size: 0f);
				this.transform.SetSizeWithCurrentAnchors(axis: RectTransform.Axis.Vertical, size: 52f);
				this.group = this.ui.gameObject.AddComponent<HorizontalLayoutGroup>();
				this.group.childControlHeight = false;
				this.group.childForceExpandHeight = false;
				this.group.childAlignment = TextAnchor.MiddleLeft;
				this.items = this.ui.gameObject.AddComponent<UIItemList>();
				this.items.layoutItems.padding = new RectOffset(left: 2, right: 2, top: 2, bottom: 2);
			}

			public UIContent ui;

			public RectTransform transform;

			public HorizontalLayoutGroup group;

			public UIItemList items;

			public LayoutElement element;
		}

		public class InputTextField
		{
			public InputTextField(Transform parent)
			{
				Transform transform = Util.Instantiate(path: "UI/Element/Input/InputText", parent: parent);
				this.transform = transform.gameObject.GetComponent<RectTransform>();
				this.element = transform.gameObject.GetComponent<LayoutElement>();
				this.element.preferredWidth = 80f;
				this.input = transform.Find(n: "InputField").GetComponent<UIInputText>();
				this.input.gameObject.GetComponent<CanvasRenderer>().SetColor(color: new Color(r: 0.99f, g: 0.99f, b: 0.99f, a: 0.3f));
				this.inputTransform = this.input.gameObject.GetComponent<RectTransform>();
				this.inputTransform.anchoredPosition = new Vector2(x: 0f, y: 27f);
				this.inputTransform.SetSizeWithCurrentAnchors(axis: RectTransform.Axis.Vertical, size: 37f);
				this.input.Find(name: "Text").gameObject.GetComponent<Text>().color = new Color(r: 0.2896f, g: 0.2255f, b: 0.1293f, a: 1f);
				transform.Find(n: "text invalid (1)").SetActive(enable: false);
				this.placeholder = this.input.Find(name: "Placeholder");
				this.placeholderText = this.placeholder.GetComponent<Text>();
				this.placeholderText.color = new Color(r: 0.2896f, g: 0.2255f, b: 0.1293f, a: 1f);
				this.placeholder.gameObject.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(axis: RectTransform.Axis.Vertical, size: 16f);
				this.input.Find(name: "InputField Input Caret");
				this.input.Find(name: "Image").SetActive(enable: false);
				this.transform.SetSizeWithCurrentAnchors(axis: RectTransform.Axis.Vertical, size: 55f);
			}

			public RectTransform transform;

			public UIInputText input;

			public RectTransform inputTransform;

			public Transform placeholder;

			public Text placeholderText;

			public LayoutElement element;
		}

		public class ScrollLayout
		{
			public ScrollLayout(Transform parent)
			{
				this.scroll = OmegaUI.GetResource<ScrollRect>();
				this.rect = this.scroll.Rect();
				this.rect.SetParent(p: parent);
				this.element = this.scroll.gameObject.GetComponent<LayoutElement>();
				this.element.flexibleHeight = 10f;
				this.headerRect = (RectTransform)this.rect.Find(n: "Header Top Parchment");
				this.uiHeader = this.headerRect.GetComponent<UIHeader>();
				this.header = this.headerRect.Find(n: "UIText").gameObject.GetComponent<UIText>();
				RectTransform rectTransform = (RectTransform)this.rect.Find(n: "Viewport");
				rectTransform.anchoredPosition = new Vector2(x: 0f, y: -25f);
				this.root = (RectTransform)rectTransform.Find(n: "Content");
				this.root.DestroyChildren(destroyInactive: false, ignoreDestroy: true);
				this.root.DestroyAllChildren();
				this.layout = this.root.gameObject.GetComponent<VerticalLayoutGroup>();
				this.layout.childControlHeight = true;
			}
			
			public ScrollRect scroll;

			public RectTransform rect;

			public RectTransform headerRect;
			
			public UIHeader uiHeader;

			public UIText header;

			public VerticalLayoutGroup layout;

			public RectTransform root;

			public LayoutElement element;
		}
    }
}
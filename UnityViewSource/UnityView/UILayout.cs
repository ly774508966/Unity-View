﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityView.Component;

namespace UnityView
{
    public enum ScrollOrentation
    {
        Vertical,
        Horizontal,
        Free
    }
    public enum Alignment
    {
        Left,
        Mid,
        Right,
        Top,
        Bottom
    }
    public class UILayout
    {
        public GameObject UIObject { get; protected set; }

        public string Name
        {
            get { return UIObject.name; }
            set { UIObject.name = value; }
        }
        public int Layer { get; set; }

        public UILayout SuperView = null;
        public readonly List<UILayout> Subviews = new List<UILayout>();

        public RectTransform RectTransform;

        public virtual float Width { get; set; }
        public virtual float Height { get; set; }

        public Vector2 Size
        {
            get
            {
                return new Vector2(Width, Height);
            }
        }
        // 矩形布局
        public Rect Rect
        {
            set
            {
                RectTransform.pivot = UIConstant.TopLeftVector2;
                RectTransform.anchorMin = new Vector2(value.position.x, 1 - value.position.y - value.size.y);
                RectTransform.anchorMax = new Vector2(value.position.x + value.size.x, 1 - value.position.y);
                RectTransform.offsetMin = Vector2.zero;
                RectTransform.offsetMax = Vector2.zero;
                RectTransform.localScale = Vector3.one;
                Vector3[] vectors = new Vector3[4];
                RectTransform.GetWorldCorners(vectors);
                Width = vectors[2].x - vectors[1].x;
                Height = vectors[1].y - vectors[0].y;
            }
            get { return GetRect(RectTransform); }
        }
        // 边框布局
        public Frame Frame
        {
            set
            {
                Width = value.Size.x;
                Height = value.Size.y;
                RectTransform.pivot = UIConstant.TopLeftVector2;
                RectTransform.anchorMin = RectTransform.anchorMax = new Vector2(value.Origin.x, 1 - value.Origin.y);
                RectTransform.sizeDelta = value.Size;
                RectTransform.anchoredPosition = Vector2.zero;
                RectTransform.localScale = Vector3.one;
            }
        }

        // 标准化矩形布局，以1280*720为基准
        public UIRect UIRect
        {
            set
            {
                Rect rect = GetRect(RectTransform.parent.GetComponent<RectTransform>());
                RectTransform.anchorMin = new Vector2(value.Left / rect.width, (rect.height - value.Bottom) / rect.height);
                RectTransform.anchorMax = new Vector2(value.Right / rect.width, (rect.height - value.Top) / rect.height);
                RectTransform.offsetMin = RectTransform.offsetMax = Vector2.zero;
                Width = value.Width;
                Height = value.Height;
            }
        }
        // 标准化边框布局， 以1280*720为基准
        public UIFrame UIFrame
        {
            set
            {
                Rect rect = GetRect(RectTransform.parent.GetComponent<RectTransform>());

                RectTransform.pivot = new Vector2(0, 1);
                RectTransform.anchorMin =
                    RectTransform.anchorMax = new Vector2(value.Origin.x / rect.width, (rect.height - value.Origin.y) / rect.height);
                RectTransform.sizeDelta = new Vector2(value.Size.x, value.Size.y);
                RectTransform.anchoredPosition = Vector2.zero;
                Width = value.Width;
                Height = value.Height;
            }
        }

        public Vector2 Origin
        {
            get
            {
                Vector3[] corners = new Vector3[4];
                RectTransform.GetWorldCorners(corners);
                return new Vector2(corners[1].x, Screen.height - corners[1].y);
            }
        }
        public UILayout() : this(UIViewManager.GetInstance().RootView) { }

        public UILayout(RectTransform transform)
        {
            UIObject = BaseLayout();
            RectTransform = UIObject.GetComponent<RectTransform>();
            RectTransform.SetParent(transform);
        }
        public UILayout(UILayout layout)
        {
            UIObject = BaseLayout();
            RectTransform = UIObject.GetComponent<RectTransform>();
            layout.AddSubview(this);
        }

        public UILayout(GameObject gameObject)
        {
            UIObject = gameObject;
            UIObject.layer = UIConstant.UILayer;
            RectTransform = gameObject.GetComponent<RectTransform>() ?? gameObject.AddComponent<RectTransform>();
        }

        public static GameObject BaseLayout()
        {
            return BaseLayout("New Layout");
        }

        public static GameObject BaseLayout(string name)
        {
            var gameObject = new GameObject
            {
                layer = UIConstant.UILayer,
                name = name
            };
            var transform = gameObject.AddComponent<RectTransform>();
            return gameObject;
        }

        // 是否可见
        private bool _visible = true;

        public bool Visible
        {
            set
            {
                _visible = value;
                UIObject.SetActive(value);
            }
            get
            {
                return _visible;
            }
        }

        /// <summary>
        /// 添加一个子View
        /// </summary>
        /// <param name="view">被添加的view</param>
        public void AddSubview(UILayout view)
        {
            if (view == null) return;
            view.RectTransform.SetParent(RectTransform);
            if (view.SuperView != null)
            {
                view.SuperView.RemoveView(view);
            }
            view.SuperView = this;
            Subviews.Add(view);
        }

        public virtual void MoveToView(UILayout view)
        {
            view.RectTransform.SetParent(view.RectTransform);
            view.SuperView = view;
        }

        public virtual void RemoveView(UILayout view)
        {
            Subviews.Remove(view);
        }

        public void Destory()
        {
            foreach (var subview in Subviews)
            {
                subview.Destory();
            }
            Subviews.Clear();
            Object.Destroy(UIObject);
        }

        public static Rect GetRect(RectTransform transform)
        {
            Vector3[] corners = new Vector3[4];
            transform.GetWorldCorners(corners);
            return new Rect(corners[0].x, corners[0].y, corners[2].x - corners[0].x, corners[2].y - corners[0].y);
        }
        public static void RectFill(UILayout layout)
        {
            RectTransform parent = layout.RectTransform.parent.GetComponent<RectTransform>();
            RectFill(layout.RectTransform);
            Rect rect = GetRect(parent);
            layout.Width = rect.width;
            layout.Height = rect.height;
        }
        public static void RectFill(RectTransform rectTransform)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }

        public static void FrameFill(UILayout layout)
        {
            
        }
        public static void FrameFill(RectTransform rectTransform)
        {

        }

        public static void RectToCenterZoom(RectTransform rectTransform, float zoom)
        {
            var minOffset = (1f - zoom) / 2f;
            var maxOffset = zoom + minOffset;
            rectTransform.anchorMin = new Vector2(minOffset, minOffset);
            rectTransform.anchorMax = new Vector2(maxOffset, maxOffset);
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }

        public static void RectToCenterMargin(RectTransform rectTransform, float margin)
        {

        }

        public static void FrameToCenterZoom(RectTransform rectTransform, float zoom)
        {

        }

        public static void FrameToCenterMargin(RectTransform rectTransform, float margin)
        {

        }

        // 给Layout添加一个下划线
        public static void AddUnderLine(RectTransform parent, float height, Color color)
        {
            GameObject underLine = new GameObject("Under Line");
            underLine.AddComponent<CanvasRenderer>();
            RectTransform rectTransform = underLine.AddComponent<RectTransform>();
            Image image = underLine.AddComponent<Image>();
            image.color = color;
            rectTransform.SetParent(parent);
            rectTransform.anchorMax = new Vector2(1f, height / parent.rect.height);
            rectTransform.anchorMin = rectTransform.offsetMin = rectTransform.offsetMax = Vector2.zero;
        }
    }
}

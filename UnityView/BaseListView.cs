using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityView.Adapter;
using UnityView.Event;

namespace UnityView
{
    public abstract class BaseListView : UIView
    {
        // 滚动方向
        public ScrollOrentation ScrollOrentation
        {
            get
            {
                if (ScrollRect.horizontal)
                {
                    return ScrollRect.vertical ? ScrollOrentation.Free : ScrollOrentation.Horizontal;
                }
                return ScrollOrentation.Vertical;
            }
            set
            {
                if (ScrollOrentation == value) return;
                switch (value)
                {
                    case ScrollOrentation.Horizontal:
                        ScrollRect.horizontal = true;
                        ScrollRect.vertical = false;
                        break;
                    case ScrollOrentation.Vertical:
                        ScrollRect.horizontal = false;
                        ScrollRect.vertical = true;
                        break;
                    case ScrollOrentation.Free:
                        ScrollRect.horizontal = true;
                        ScrollRect.vertical = true;
                        break;
                }
                RepositionItem();
            }
        }
        // 是否开启边界弹性
        public bool BounceEnable
        {
            set
            {
                ScrollRect.movementType = value ? ScrollRect.MovementType.Elastic : ScrollRect.MovementType.Clamped;
            }
            get
            {
                return ScrollRect.movementType == ScrollRect.MovementType.Clamped;
            }
        }

        public Vector2 Spacing = Vector2.zero;

//        public bool MaskEnable = true;
        public Mask Mask;

        protected ScrollRect ScrollRect;
        protected RectTransform ContentTransform;

        public Vector2 ContentSize
        {
            get
            {
                return ContentTransform.sizeDelta;
            }
        }
        protected List<IConvertView> Items = new List<IConvertView>();

        protected int LastStartIndex = 0;
        // 当前显示的元素个数
        protected int CurrentShowItemCount
        {
            get
            {
                int maxShowNum = GetMaxShowItemNum();
                int maxItemNum = Adapter.GetCount() - GetStartIndex();
                return maxShowNum < Adapter.GetCount() - GetStartIndex() ? maxShowNum : maxItemNum;
            }
        }

        protected BaseEventHandler EventHandler;
        public OnItemSelectedListener OnItemSelectedListener;

        protected IListAdapter MAdapter;
        public virtual IListAdapter Adapter
        {
            get
            {
                return MAdapter;
            }
            set
            {
                MAdapter = value;
                CalculateContentSize();
                RepositionItem();
            }
        }

        protected BaseListView()
        {
            ScrollRect = UIObject.AddComponent<ScrollRect>();
            ScrollRect.onValueChanged.AddListener(OnValueChanged);
            EventHandler = UIObject.AddComponent<BaseEventHandler>();
            EventHandler.BaseListView = this;
            var contentView = new UILayout { UIObject = { name = "Content View" } };
            AddSubview(contentView);
            ContentTransform = contentView.RectTransform;
            ContentTransform.pivot = ContentTransform.anchorMin = ContentTransform.anchorMax = new Vector2(0, 1);
            ContentTransform.anchoredPosition = Vector2.zero;
            ScrollRect.content = ContentTransform;

            ScrollOrentation = ScrollOrentation.Vertical;

            BackgroundColor = Color.black;
            Mask = UIObject.AddComponent<Mask>();
            Mask.showMaskGraphic = false;
        }
        private void OnValueChanged(Vector2 pos)
        {
            int startIndex = GetStartIndex();
            if (startIndex != LastStartIndex && startIndex >= 0)
            {
                RepositionItem();
                LastStartIndex = startIndex;
            }
        }

        public abstract int GetMaxShowItemNum();
        protected abstract Vector2 GetItemAnchorPostion(int index);
        protected abstract int GetStartIndex();
        public abstract void HideNonuseableItems();
        public abstract IConvertView GetItem(int index);

        // 重载表视图的数据
        public void RepositionItem()
        {
            if (Adapter == null) return;

            int startIndex = GetStartIndex();
            if (startIndex < 0) startIndex = 0;

            for (int i = 0; i < CurrentShowItemCount; ++i)
            {
                var item = GetItem(i);
                var transform = item.GetRectTransform();
                transform.SetParent(ContentTransform);
                transform.pivot = transform.anchorMin = transform.anchorMax = new Vector2(0.5f, 0.5f);
                transform.anchoredPosition = GetItemAnchorPostion(startIndex + i);
                Adapter.GetConvertView(startIndex + i, item);
            }

            HideNonuseableItems();
        }

        public virtual void Reload()
        {
            if (Adapter == null) return;
            CalculateContentSize();
            int count = Mathf.Min(GetMaxShowItemNum(), Adapter.GetCount());
            if (Items.Count > count)
            {
                for (int i = count; i < Items.Count; i++)
                {
                    if (Items[i].GetRectTransform().gameObject.activeSelf)
                    {
                        Items[i].GetRectTransform().gameObject.SetActive(false);
                    }
                }
            }
            for (int i = 0; i < count; ++i)
            {
                var item = GetItem(i);
                var transform = item.GetRectTransform();
                transform.SetParent(ContentTransform);
                transform.pivot = transform.anchorMin = transform.anchorMax = new Vector2(0.5f, 0.5f);
                transform.anchoredPosition = GetItemAnchorPostion(i);
                Adapter.GetConvertView(i, item);
            }

            HideNonuseableItems();
        }

        protected virtual Vector2 CalculateContentSize()
        {
            if (Adapter == null) return Vector2.zero;
            float size = 0;

            for (int i = 0; i < Adapter.GetCount(); ++i)
            {
                switch (ScrollOrentation)
                {
                    case ScrollOrentation.Horizontal:
                        size += Adapter.GetItemSize(i).x + Spacing.x;
                        break;
                    case ScrollOrentation.Vertical:
                        size += Adapter.GetItemSize(i).y + Spacing.y;
                        break;
                }
            }
            switch (ScrollOrentation)
            {
                case ScrollOrentation.Horizontal:
                    ContentTransform.sizeDelta = new Vector2(size, Height);
                    break;
                case ScrollOrentation.Vertical:
                    ContentTransform.sizeDelta = new Vector2(Width, size);
                    break;
            }
            return ContentTransform.sizeDelta;
        }

        public class BaseEventHandler : MonoBehaviour, IPointerClickHandler
        {
            public BaseListView BaseListView;
            public void OnPointerClick(PointerEventData eventData)
            {
                var clickPosition = eventData.position - BaseListView.Origin;
                var anchorPosition = -BaseListView.ContentTransform.anchoredPosition;

                anchorPosition += clickPosition;
                anchorPosition.x -= BaseListView.ContentTransform.rect.size.x / 2;
                anchorPosition.y += BaseListView.ContentTransform.rect.size.y / 2;

                var startIndex = BaseListView.GetStartIndex();
                if (startIndex < 0) startIndex = 0;
                for (int i = 0; i < BaseListView.CurrentShowItemCount; ++i)
                {
                    Vector2 itemAnchorPos = BaseListView.GetItemAnchorPostion(startIndex + i);
                    var itemSize = BaseListView.Adapter.GetItemSize(startIndex + i);
                    if (Mathf.Abs(anchorPosition.x - itemAnchorPos.x) <= itemSize.x / 2 &&
                       Mathf.Abs(anchorPosition.y - itemAnchorPos.y) <= itemSize.y / 2)
                    {
                        BaseListView.OnItemSelectedListener(startIndex + i);
                        break;
                    }
                }
            }
        }
    }

    public interface IListAdapter : IAdapter
    {
        Vector2 GetItemSize(int index);
    }
}

using UnityEngine;
using UnityView.Adapter;

namespace UnityView
{
    public class GridView : BaseListView
    {
        protected int Row;
        protected int Column;

        protected Vector2 Padding = Vector2.zero;
        protected Vector2 ItemSize;

        public override IListAdapter Adapter
        {
            get
            {
                return MAdapter;
            }
            set
            {
                MAdapter = value;
                ItemSize = Adapter.GetItemSize(0);
                CalculateContentSize();
                RepositionItem();
            }
        }

        public override int GetMaxShowItemNum()
        {
            int max = 0;

            switch (ScrollOrentation)
            {
                case ScrollOrentation.Horizontal:
                    max = ((int)(ContentSize.x / ItemSize.x) + 2) * Column;
                    break;
                case ScrollOrentation.Vertical:
                    max = ((int)(ContentSize.y / ItemSize.y) + 2) * Row;
                    break;
            }
            return max;
        }

        protected override Vector2 GetItemAnchorPostion(int index)
        {
            Vector2 basePos = Vector2.zero;
            Vector2 offset = Vector2.zero;

            int offsetIndex = 0;
            switch (ScrollOrentation)
            {
                case ScrollOrentation.Horizontal:
                    offsetIndex = index % Column;
                    basePos.x = -ContentSize.x / 2 + ItemSize.x / 2;
                    offset.x = (index / Column) * (ItemSize.x + Spacing.x);
                    offset.y = ContentSize.y / 2 - ItemSize.y / 2 - offsetIndex * (ItemSize.y + Spacing.y);
                    break;
                case ScrollOrentation.Vertical:
                    offsetIndex = index % Row;
                    basePos.y = ContentSize.y / 2 - ItemSize.y / 2;
                    offset.y = -(index / Row) * (ItemSize.y + Spacing.y);
                    offset.x = -(ContentSize.x - ItemSize.x) / 2 + offsetIndex * (ItemSize.x + Spacing.x);
                    break;
            }
            return basePos + offset + Padding;
        }

        protected override int GetStartIndex()
        {
            Vector2 anchorPosition = ContentTransform.anchoredPosition;
            anchorPosition.x *= -1;
            int index = 0;

            switch (ScrollOrentation)
            {
                case ScrollOrentation.Horizontal:
                    index = (int)(anchorPosition.x / (ItemSize.x + Spacing.x)) * Column;
                    break;
                case ScrollOrentation.Vertical:
                    index = (int)(anchorPosition.y / (ItemSize.y + Spacing.y)) * Row;
                    break;
            }
            if (index < 0) index = 0;
            return index;
        }

        public override void HideNonuseableItems()
        {
            for (int i = Items.Count; Items != null && i < Items.Count; ++i)
            {
                if (Items[i].GetRectTransform().gameObject.activeSelf)
                {
                    Items[i].GetRectTransform().gameObject.SetActive(false);
                }
            }
        }

        public override IConvertView GetItem(int index)
        {
            if (index < Items.Count)
            {
                IConvertView convertView = Items[index];
                if (false == convertView.GetRectTransform().gameObject.activeSelf)
                {
                    convertView.GetRectTransform().gameObject.SetActive(true);
                }
                return Items[index];
            }
            else
            {
                IConvertView convertView = Adapter.GetConvertView(index, null);
                convertView.GetRectTransform().sizeDelta = ItemSize;
                Items.Add(convertView);
                return convertView;
            }
        }

        protected override Vector2 CalculateContentSize()
        {
            Vector2 size = RectTransform.sizeDelta;
            Row = (int)(size.x / (ItemSize.x + Spacing.x));
            Column = (int)(size.y / (ItemSize.y + Spacing.y));
            int count = Adapter.GetCount();

            switch (ScrollOrentation)
            {
                case ScrollOrentation.Horizontal:
                    count = (count + Column - 1) / Column;
                    size.x = ItemSize.x * count + Spacing.x * (count > 0 ? count - 1 : count);
                    break;
                case ScrollOrentation.Vertical:
                    count = (count + Row - 1) / Row;
                    size.y = ItemSize.y * count + Spacing.y * (count > 0 ? count - 1 : count);
                    break;
            }
            ContentTransform.sizeDelta = size;
            return size;
        }
    }
    //public class GridView : UIView
    //{
    //    public GridLayoutGroup GridLayoutGroup;

    //    public ScrollRect ScrollRect;
    //    public Mask Mask;

    //    public GameObject ContentView;
    //    public RectTransform ContentViewTransform;

    //    // 存储所有 Cell 的集合
    //    private List<IConvertView> _items = new List<IConvertView>(); 

    //    // 滑动方向
    //    public ScrollOrentation ScrollOrentation
    //    {
    //        get
    //        {
    //            if (ScrollRect.horizontal)
    //            {
    //                return ScrollRect.vertical ? ScrollOrentation.Free : ScrollOrentation.Horizontal;
    //            }
    //            return ScrollOrentation.Vertical;
    //        }
    //        set
    //        {
    //            if (ScrollOrentation == value) return;
    //            switch (value)
    //            {
    //                case ScrollOrentation.Horizontal:
    //                    ScrollRect.horizontal = true;
    //                    ScrollRect.vertical = false;
    //                    break;
    //                case ScrollOrentation.Vertical:
    //                    ScrollRect.horizontal = false;
    //                    ScrollRect.vertical = true;
    //                    break;
    //                case ScrollOrentation.Free:
    //                    ScrollRect.horizontal = true;
    //                    ScrollRect.vertical = true;
    //                    break;
    //            }
    //            RepositionItem();
    //        }
    //    }

    //    // 单元大小
    //    public Vector2 CellSize
    //    {
    //        set
    //        {
    //            GridLayoutGroup.cellSize = value;
    //        }
    //        get
    //        {
    //            return GridLayoutGroup.cellSize;
    //        }
    //    }

    //    private IAdapter _adapter;
    //    public IAdapter Adapter
    //    {
    //        set
    //        {
    //            _adapter = value;
    //            RepositionItem();
    //        }
    //        get
    //        {
    //            return _adapter;
    //        }
    //    }

    //    public GridView() : this(UIViewManager.GetInstance().RootView) { }
    //    public GridView(UILayout layout) : base(layout)
    //    {
    //        UIObject.name = "Grid View";
    //        ImageComponent = UIObject.AddComponent<Image>();
    //        ScrollRect = UIObject.AddComponent<ScrollRect>();
    //        Mask = UIObject.AddComponent<Mask>();
    //        Mask.showMaskGraphic = false;
    //        ContentView = BaseLayout("Content View");
    //        ContentViewTransform = ContentView.GetComponent<RectTransform>();
    //        GridLayoutGroup = ContentView.AddComponent<GridLayoutGroup>();

    //        ContentViewTransform.SetParent(RectTransform);

    //        ContentViewTransform.anchorMin = ContentViewTransform.anchorMax = UIConstant.TopLeftVector2;
    //        ContentViewTransform.anchoredPosition = Vector2.zero;

    //        ScrollRect.content = ContentView.GetComponent<RectTransform>();
    //        ScrollOrentation = ScrollOrentation.Vertical;
    //    }

    //    public void RepositionItem()
    //    {
    //        if (_adapter == null) return;
    //        // 重新计算 Content View 大小
    //        switch (ScrollOrentation)
    //        {
    //            case ScrollOrentation.Vertical:
    //                ContentViewTransform.sizeDelta = new Vector2(Width, Mathf.Ceil(Adapter.GetCount() / Mathf.Round(Width / CellSize.x)) * CellSize.y);
    //                break;
    //            case ScrollOrentation.Horizontal:
    //                ContentViewTransform.sizeDelta = new Vector2(Mathf.Ceil(Adapter.GetCount() / Mathf.Round(Height / CellSize.y)) * CellSize.x, Height);
    //                break;
    //        }

    //        var list = new List<IConvertView>();
    //        for (var i = 0; i < Adapter.GetCount(); i++)
    //        {
    //            IConvertView convertView = null;
    //            var hasConvertView = _items.Any();
    //            if (hasConvertView)
    //            {
    //                convertView = _items[0];
    //                _items.RemoveAt(0);
    //            }
    //            convertView = Adapter.GetConvertView(i, convertView);
    //            if (!hasConvertView)
    //            {
    //                convertView.GetRectTransform().SetParent(ContentViewTransform);
    //            }
    //            list.Add(convertView);
    //        }
    //        if (_items.Any())
    //        {
    //            foreach (var cell in _items)
    //            {
    //                cell.GetRectTransform().gameObject.SetActive(false);
    //            }
    //        }
    //        _items = list;
    //    }

    //    public IConvertView AddItem(IConvertView item)
    //    {
    //        _items.Add(item);
    //        return item;
    //    }

    //    public IConvertView RemoveItem()
    //    {
    //        var item = _items.Last();
    //        _items.Remove(item);
    //        return item;
    //    }
    //}
}

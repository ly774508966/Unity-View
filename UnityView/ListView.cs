using UnityEngine;
using UnityView.Adapter;

namespace UnityView
{
    public class ListView : BaseListView
    {
        public Alignment Alignment;
        public ListView()
        {
            BounceEnable = true;
        }

        public override int GetMaxShowItemNum()
        {
            int max = 0;
            int startIndex = GetStartIndex();
            float sum = 0;
            switch (ScrollOrentation)
            {
                case ScrollOrentation.Horizontal:
                    while (startIndex < Adapter.GetCount() && sum < Width)
                    {
                        sum += Adapter.GetItemSize(startIndex).x + Spacing.x;
                        startIndex++;
                        max++;
                    }
                    break;
                case ScrollOrentation.Vertical:
                    while (startIndex < Adapter.GetCount() && sum < Height)
                    {
                        sum += Adapter.GetItemSize(startIndex).y + Spacing.y;
                        startIndex++;
                        max++;
                    }
                    break;
            }
            return max + 1;
        }

        protected override Vector2 GetItemAnchorPostion(int index)
        {
            Vector2 basePosition = Vector2.zero;
            Vector2 offset = Vector2.zero;
            RectTransform contentRectTransform = ContentTransform;
            Vector2 contentRectSize = contentRectTransform.rect.size;

            switch (ScrollOrentation)
            {
                case ScrollOrentation.Horizontal:
                    basePosition.x = -contentRectSize.x / 2 - Adapter.GetItemSize(index).x / 2;
                    for (int i = 0; i <= index; ++i)
                    {
                        offset.x += (Adapter.GetItemSize(i).x + Spacing.x);
                    }
                    break;
                case ScrollOrentation.Vertical:

                    basePosition.y = contentRectSize.y / 2 + Adapter.GetItemSize(index).y / 2;
                    for (int i = 0; i <= index; ++i)
                    {
                        offset.y -= (Adapter.GetItemSize(i).y + Spacing.y);
                    }
                    break;
            }
            return basePosition + offset;
        }

        protected override int GetStartIndex()
        {
            Vector2 anchor = ContentTransform.anchoredPosition;
            anchor.x *= -1;
            int startIndex = -1;
            float sum = 0;
            switch (ScrollOrentation)
            {
                case ScrollOrentation.Horizontal:
                    sum = -Spacing.x;
                    for (int i = 0; i < Adapter.GetCount(); ++i)
                    {
                        var itemSize = new Vector2(Adapter.GetItemSize(startIndex + i).x, Height);

                        sum += (itemSize.x + Spacing.x);
                        if (sum <= anchor.x)
                        {
                            startIndex = i;
                        }
                        else
                        {
                            break;
                        }
                    }
                    break;
                case ScrollOrentation.Vertical:
                    sum = Spacing.y;
                    for (int i = 0; i < Adapter.GetCount(); ++i)
                    {
                        Vector2 itemSize = new Vector2(Width, Adapter.GetItemSize(startIndex + i).y);
                        sum += (itemSize.y + Spacing.y);
                        if (sum <= anchor.y)
                        {
                            startIndex = i;
                        }
                        else
                        {
                            break;
                        }
                    }
                    break;
            }
            return ++startIndex;
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
                var transform = convertView.GetRectTransform();
                switch (ScrollOrentation)
                {
                    case ScrollOrentation.Horizontal:
                        transform.sizeDelta = new Vector2(Adapter.GetItemSize(index).x, Height);
                        break;
                    case ScrollOrentation.Vertical:
                        transform.sizeDelta = new Vector2(Width, Adapter.GetItemSize(index).y);
                        break;
                }
                Items.Add(convertView);
                return convertView;
            }
        }
    }
}

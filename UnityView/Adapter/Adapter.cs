using UnityEngine;

namespace UnityView.Adapter
{
    public interface IAdapter
    {
        int GetCount();
        IConvertView GetConvertView(int position, IConvertView convertView);
    }

    public interface IConvertView
    {
        RectTransform GetRectTransform();
    }
}

using UnityEngine;
using UnityEngine.UI;
using UnityView.Component;

namespace UnityView
{
    public class ImageView : UIView
    {
        public readonly Image Image;

        public Image.Type ImageType
        {
            get
            {
                return Image.type;
            }
            set
            {
                Image.type = value;
            }
        }

        public ImageView()
        {
            Image = UIObject.AddComponent<Image>();
        }

        public ImageView(UILayout layout) : base(layout) { }
    }
}

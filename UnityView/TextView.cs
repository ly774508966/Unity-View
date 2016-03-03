using System.Security;
using UnityEngine;
using UnityEngine.UI;
using UnityView.Component;

namespace UnityView
{
    public class TextView : UIView
    {
        public readonly Text TextHolder;

        public string Text
        {
            set
            {
                TextHolder.text = value;
            }
            get
            {
                return TextHolder.text;
            }
        }

        public Font Font
        {
            set
            {
                TextHolder.font = value;
            }
            get
            {
                return TextHolder.font;
            }
        }

        public int FontSize
        {
            set
            {
                TextHolder.fontSize = (int)(value * UIConstant.FontCoefficient);
            }
            get
            {
                return TextHolder.fontSize;
            }
        }

        public TextAnchor Alignment
        {
            set
            {
                TextHolder.alignment = value;
            }
            get
            {
                return TextHolder.alignment;
            }
        }

        public TextView() : this(UIViewManager.GetInstance().RootView) { }

        public TextView(UILayout layout)
            : base(layout)
        {
            TextHolder = UIObject.AddComponent<Text>();
            Font = UIViewManager.GetInstance().Font;
        }

        public TextView(GameObject gameObject)
            : base(gameObject)
        {
            TextHolder = gameObject.GetComponent<Text>() ?? gameObject.AddComponent<Text>();
            Font = UIViewManager.GetInstance().Font;
        }

        public static GameObject BaseTextView()
        {
            var gameObject = BaseView();
            var text = gameObject.AddComponent<Text>();
            text.font = UIViewManager.GetInstance().Font;
            text.alignment = TextAnchor.MiddleCenter;
            return gameObject;
        }

        public static int NormalizedFontSize(float fontSize)
        {
            return Mathf.RoundToInt(fontSize * UIConstant.FontCoefficient);
        }
    }
}

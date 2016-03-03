using UnityEngine;
using UnityEngine.UI;
using UnityView.Component;

namespace UnityView
{
    public class UIView : UILayout
    {

        public CanvasRenderer CanvasRenderer { get; set; }

        protected Image BackgroundImage;
        public Color BackgroundColor
        {
            set
            {
                if (BackgroundImage == null)
                {
                    BackgroundImage = UIObject.AddComponent<Image>();
                }
                BackgroundImage.color = value;
            }
            get
            {
                return BackgroundImage == null ? Color.clear : BackgroundImage.color;
            }
        }

        public Sprite Sprite
        {
            set
            {
                if (BackgroundImage == null)
                {
                    BackgroundImage = UIObject.AddComponent<Image>();
                }
                BackgroundImage.sprite = value;
            }
            get
            {
                return BackgroundImage.sprite;
            }
        }

        /// <summary>
        /// 构造方法
        /// 不传参时，默认以 Canvas 为父物体
        /// 传参时，以参数为父物体
        /// 也可根据传入的 gameObject 构造对象
        /// </summary>
        public UIView() : this(UIViewManager.GetInstance().RootView) { }

        public UIView(UILayout layout) : base(layout)
        {
            CanvasRenderer = UIObject.AddComponent<CanvasRenderer>();
        }

        public UIView(GameObject gameObject) : base(gameObject)
        {
            CanvasRenderer = gameObject.GetComponent<CanvasRenderer>() ?? gameObject.AddComponent<CanvasRenderer>();
        }

        public static GameObject BaseView()
        {
            return BaseView("New View");
        }
        public static GameObject BaseView(string name)
        {
            var gameObject = BaseLayout(name);
            gameObject.AddComponent<CanvasRenderer>();
            return gameObject;
        }
    }
}

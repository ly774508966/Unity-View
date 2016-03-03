using UnityEngine;

namespace UnityView.Component
{
    /// <summary>
    /// View 管理器，存放管理视图的基本信息
    /// </summary>
    public class UIViewManager : MonoBehaviour
    {
        public GameObject Canvas
        {
            get
            {
                return UICanvas.GetInstance().UIObject;
            }
        }

        public UILayout RootView
        {
            get { return UICanvas.GetInstance(); }
        }

        public Font Font;

        private static UIViewManager _instance;

        public static readonly float UnitWidth = Screen.width / 1280f;
        public static readonly float UnitHeight = Screen.height / 720f;

        void Awake()
        {
            _instance = this;
        }

        public static UIViewManager GetInstance()
        {
            return _instance ?? FindObjectOfType<UIViewManager>();
        }
    }
}

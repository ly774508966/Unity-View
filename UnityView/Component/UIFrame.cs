using UnityEngine;

namespace UnityView.Component
{
    public struct UIFrame
    {
        public static readonly float UnitWidth = Screen.orientation == ScreenOrientation.Landscape ? Screen.width / 1280f : Screen.width / 720f;
        public static readonly float UnitHeight = Screen.orientation == ScreenOrientation.Landscape ? Screen.height / 720f : Screen.height / 1280f;

        public Vector2 Origin;
        public Vector2 Size;

        public UIFrame(Vector2 origin, Vector2 size)
        {
            Origin = origin;
            Size = size;
        }
        public UIFrame(float originX, float originY, float width, float height)
        {
            Origin = new Vector2(originX * UnitWidth, originY * UnitHeight);
            Size = new Vector2(width * UnitWidth, height * UnitHeight);
        }
    }
}

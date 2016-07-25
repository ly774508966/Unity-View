using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UnityView.Component
{
    public struct UIRect
    {
        public static readonly float UnitWidth = Screen.orientation == ScreenOrientation.Landscape ? Screen.width / 1280f : Screen.width / 720f;
        public static readonly float UnitHeight = Screen.orientation == ScreenOrientation.Landscape ? Screen.height / 720f : Screen.height / 1280f;

        public Vector2 Origin;
        public Vector2 Size;

        public float Top
        {
            get { return Origin.y; }
        }

        public float Left
        {
            get { return Origin.x; }
        }

        public float Width
        {
            get { return Size.x; }
        }

        public float Height
        {
            get { return Size.y; }
        }
        
        public UIRect(float originX, float originY, float width, float height)
        {
            Origin = new Vector2(originX * UnitWidth, originY * UnitHeight);
            Size = new Vector2(width * UnitWidth, height * UnitHeight);
        }
    }
}

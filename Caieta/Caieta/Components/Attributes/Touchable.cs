using System;

namespace Caieta.Components.Attributes
{
    public abstract class Touchable : Component
    {
        public Action OnTap;
        public Action OnDoubleTap;
        public Action OnHold;

        public Action OnTouchedObject;

        public Action OnMouseEnter;
        public Action OnMouseExit;

        public bool IsMouseOver;
    }
}

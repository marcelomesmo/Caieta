using System;

namespace Caieta.Components.Attributes
{
    public class OneWayPlatform : BoxCollider
    {
        public bool FallDown;

        public OneWayPlatform(bool fallDown, float width, float height, float x = 0, float y = 0) : base(width, height, x, y)
        {
            IsTrigger = true;
            FallDown = fallDown;
        }
    }
}
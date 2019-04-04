using System;

namespace Caieta
{
    public class Solid : BoxCollider, IUnique
    {
        public Solid(float width, float height, float x = 0, float y = 0) : base(width, height, x, y)
        {
            // Notes: Set IsTrigger = true in BoxCollider to make Solid mandatory.
            IsTrigger = false;
        }
    }
}

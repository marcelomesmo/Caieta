using System;

namespace Caieta
{
    public class Solid : Component, IUnique
    {
        /*
         *      Collision
         */
        public BoxCollider box;

        public Solid(float width, float height, float x = 0, float y = 0)
        {
            box = new BoxCollider(width, height, x, y);

            box.SetSolid(true);
        }

    }
}

using System;

namespace Caieta
{
    public class Solid : Component
    {
        /*
         *      Position
         */
        public Transform Transform;

        /*
         *      Collision
         */
        public BoxCollider box;

        public Solid(float width, float height, float x = 0, float y = 0)
        {
            Transform = new Transform();

            box = new BoxCollider(width, height, x, y);

            box.IsVisible = true;

            box.SetSolid(true);
        }

        public override void Render()
        {
            base.Render();

            if (box.IsVisible)
                box.Render(Transform);
        }

    }
}

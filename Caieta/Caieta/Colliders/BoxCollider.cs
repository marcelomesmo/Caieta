using System;
using Caieta.Entities;
using Microsoft.Xna.Framework;

namespace Caieta
{
    public class BoxCollider : Collider
    {
        /*
         *      VISIBILITY
         */
        public bool IsVisible;
        public Color Color = Color.White;

        /* 
         *      SIZE & POSITION
         */
        #region Attributes

        public override float Width
        {
            get { return _width; }
            set { _width = value; }
        }

        public override float Height
        {
            get { return _height; }
            set { _height = value; }
        }
        private float _width;
        private float _height;

        public override float Left
        {
            get { return LocalPosition.X; }
            set { LocalPosition.X = value; }
        }

        public override float Top
        {
            get { return LocalPosition.Y; }
            set { LocalPosition.Y = value; }
        }

        public override float Right
        {
            get { return LocalPosition.X + Width; }
            set { LocalPosition.X = value - Width; }
        }

        public override float Bottom
        {
            get { return LocalPosition.Y + Height; }
            set { LocalPosition.Y = value - Height; }
        }

        #endregion

        public BoxCollider(float width, float height, float x = 0, float y = 0)
        {
            _width = width;
            _height = height;

            LocalPosition.X = x;
            LocalPosition.Y = y;
        }
        public BoxCollider(Rectangle rect) : this(rect.Width, rect.Height, rect.X, rect.Y) { }

        public void Update(Transform parent)
        {
            AbsolutePosition = parent.Position + LocalPosition;
        }

        public override void Render(Transform parent)
        {
            Graphics.DrawRect(AbsolutePosition.X, AbsolutePosition.Y, Width, Height, Color, 100, FillType.HOLLOW);
        }

        #region Collisions

        /*
         *  Checking against other colliders
         */
        /*public override bool Collide(Vector2 point)
        {
            return Collide.RectToPoint(AbsoluteLeft, AbsoluteTop, Width, Height, point);
        }

        public override bool Collide(Rectangle rect)
        {
            return AbsoluteRight > rect.Left && AbsoluteBottom > rect.Top && AbsoluteLeft < rect.Right && AbsoluteTop < rect.Bottom;
        }

        public override bool Collide(Vector2 from, Vector2 to)
        {
            return Collide.RectToLine(AbsoluteLeft, AbsoluteTop, Width, Height, from, to);
        }

        public override bool Collide(BoxCollider hitbox)
        {
            return Intersects(hitbox);
        }

        public bool Intersects(BoxCollider other_collider)
        {
            return AbsoluteLeft < other_collider.AbsoluteRight && AbsoluteRight > other_collider.AbsoluteLeft && AbsoluteBottom > other_collider.AbsoluteTop && AbsoluteTop < hitbox.AbsoluteBottom;
        }

        public bool Intersects(float x, float y, float width, float height)
        {
            return AbsoluteRight > x && AbsoluteBottom > y && AbsoluteLeft < x + width && AbsoluteTop < y + height;
        }*/

        public override bool Collide(Vector2 point)
        {
            throw new NotImplementedException();
        }

        public override bool Collide(Rectangle rect)
        {
            throw new NotImplementedException();
        }

        public override bool Collide(Vector2 from, Vector2 to)
        {
            throw new NotImplementedException();
        }

        public override bool IsOverlapping(BoxCollider other_collider)
        {
            return Intersects(other_collider);
        }

        public bool Intersects(BoxCollider other_collider)
        {
            return AbsolutePosition.X < other_collider.AbsolutePosition.X + other_collider.Width && AbsolutePosition.X + Width > other_collider.AbsolutePosition.X && AbsolutePosition.Y + Height > other_collider.AbsolutePosition.Y && AbsolutePosition.Y < other_collider.AbsolutePosition.Y + other_collider.Height;
        }

        #endregion

        #region Utils

        public override Collider Clone()
        {
            return new BoxCollider(_width, _height, LocalPosition.X, LocalPosition.Y);
        }

        /*public override string ToString()
         {
             return string.Format("[BoxCollider]: {0} Origin: {1} Center: {2} Transform:\n {3} ", Name, Origin, Center, Transform);
         }*/

        #endregion
    }
}

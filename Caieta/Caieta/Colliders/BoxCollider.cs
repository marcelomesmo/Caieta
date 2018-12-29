﻿using System;
using Caieta.Entities;
using Microsoft.Xna.Framework;

namespace Caieta
{
    public class BoxCollider : Collider
    {
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
            get { return Origin.X; }
            set { Origin.X = value; }
        }

        public override float Top
        {
            get { return Origin.Y; }
            set { Origin.Y = value; }
        }

        public override float Right
        {
            get { return Origin.X + Width; }
            set { Origin.X = value - Width; }
        }

        public override float Bottom
        {
            get { return Origin.Y + Height; }
            set { Origin.Y = value - Height; }
        }

        #endregion

        public BoxCollider(float width, float height, float x = 0, float y = 0)
        {
            _width = width;
            _height = height;

            // Notes: may need to invert this
            //      Origin is based on Entity position.
            //      A 0, 0 Origin would be Entity Origin.
            //      A 5, 5 Origin would be centered in the Entity's 5, 5 position
            Origin.X = -x;
            Origin.Y = -y;
        }
        public BoxCollider(Rectangle rect) : this(rect.Width, rect.Height, rect.X, rect.Y) { }
        public BoxCollider(Sprite sprite) : this(sprite.Width, sprite.Height)
        {
            Origin = sprite.Origin;
        }

        #region Collisions

        /*
         *  Checking against other colliders
         */
        public override bool IsOverlapping(Rectangle rect)
        {
            return AbsolutePosition.X + Width > rect.Left && AbsolutePosition.Y + Height > rect.Top && AbsolutePosition.X < rect.Right && AbsolutePosition.Y < rect.Bottom;
        }

        public override bool IsOverlapping(BoxCollider other_collider)
        {
            return Intersects(other_collider);
        }

        public bool Intersects(BoxCollider other_collider)
        {
            return AbsolutePosition.X < other_collider.AbsolutePosition.X + other_collider.Width && AbsolutePosition.X + Width > other_collider.AbsolutePosition.X && AbsolutePosition.Y + Height > other_collider.AbsolutePosition.Y && AbsolutePosition.Y < other_collider.AbsolutePosition.Y + other_collider.Height;
        }

        public bool Intersects(float x, float y, float width, float height)
        {
            return AbsolutePosition.X + Width > x && AbsolutePosition.Y + Height > y && AbsolutePosition.X < x + width && AbsolutePosition.Y < y + height;
        }

        #endregion

        #region Utils

        public override Collider Clone()
        {
            return new BoxCollider(_width, _height, Origin.X, Origin.Y);
        }

        public override string ToString()
        {
             return string.Format("[BoxCollider]: Absolute Position: {0} Origin: {1} Center: {2} Is trigger: {3} ", AbsolutePosition, Origin, Center, IsTrigger);
        }

        #endregion
    }
}

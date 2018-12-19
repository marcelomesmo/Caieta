using System;
using Caieta.Entities;
using Microsoft.Xna.Framework;

namespace Caieta
{
    public abstract class Collider
    {
        public abstract float Width { get; set; }
        public abstract float Height { get; set; }
        public abstract float Top { get; set; }
        public abstract float Bottom { get; set; }
        public abstract float Left { get; set; }
        public abstract float Right { get; set; }

        public bool IsTrigger = true;

        public Vector2 LocalPosition;

        public Vector2 AbsolutePosition;

        public bool IsOverlapping(Collider collider)
        {
            if (collider is BoxCollider)
            {
                return IsOverlapping(collider as BoxCollider);
            }/*
            else if (collider is Grid)
            {
                return Collide(collider as Grid);
            }
            else if (collider is ColliderList)
            {
                return Collide(collider as ColliderList);
            }
            else if (collider is Circle)
            {
                return Collide(collider as Circle);
            }*/
            else
                throw new Exception("[Collider]: Collisions against the collider type are not implemented.");
        }

        public abstract bool Collide(Vector2 point);
        public abstract bool Collide(Rectangle rect);
        public abstract bool Collide(Vector2 from, Vector2 to);
        public abstract bool IsOverlapping(BoxCollider other_collider);
        /*public abstract bool Collide(Grid grid);
        public abstract bool Collide(Circle circle);
        public abstract bool Collide(ColliderList list);*/
        public abstract Collider Clone();
        //public abstract void Render(Camera camera, Color color);
        public abstract void Render(Transform parent);

        public float CenterX
        {
            get
            {
                return Left + Width / 2f;
            }

            set
            {
                Left = value - Width / 2f;
            }
        }

        public float CenterY
        {
            get
            {
                return Top + Height / 2f;
            }

            set
            {
                Top = value - Height / 2f;
            }
        }

        public Vector2 TopLeft
        {
            get
            {
                return new Vector2(Left, Top);
            }

            set
            {
                Left = value.X;
                Top = value.Y;
            }
        }

        public Vector2 TopCenter
        {
            get
            {
                return new Vector2(CenterX, Top);
            }

            set
            {
                CenterX = value.X;
                Top = value.Y;
            }
        }

        public Vector2 TopRight
        {
            get { return new Vector2(Right, Top); }

            set {
                Right = value.X;
                Top = value.Y;
            }
        }

        public Vector2 CenterLeft
        {
            get { return new Vector2(Left, CenterY); }

            set {
                Left = value.X;
                CenterY = value.Y;
            }
        }

        public Vector2 Center
        {
            get { return new Vector2(CenterX, CenterY); }

            set {
                CenterX = value.X;
                CenterY = value.Y;
            }
        }

        public Vector2 Size
        {
            get
            {
                return new Vector2(Width, Height);
            }
        }

        public Vector2 HalfSize
        {
            get
            {
                return Size * .5f;
            }
        }

        public Vector2 CenterRight
        {
            get
            {
                return new Vector2(Right, CenterY);
            }

            set
            {
                Right = value.X;
                CenterY = value.Y;
            }
        }

        public Vector2 BottomLeft
        {
            get
            {
                return new Vector2(Left, Bottom);
            }

            set
            {
                Left = value.X;
                Bottom = value.Y;
            }
        }

        public Vector2 BottomCenter
        {
            get
            {
                return new Vector2(CenterX, Bottom);
            }

            set
            {
                CenterX = value.X;
                Bottom = value.Y;
            }
        }

        public Vector2 BottomRight
        {
            get
            {
                return new Vector2(Right, Bottom);
            }

            set
            {
                Right = value.X;
                Bottom = value.Y;
            }
        }

        public void SetSolid(bool solid)
        {
            IsTrigger = !solid;
        }
    }
}

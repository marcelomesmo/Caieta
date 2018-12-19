using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Caieta
{
    public abstract class Renderable : Component
    {
        /*
         *      Size & Position   
         */
        public enum Anchor { BOTTOM_LEFT, BOTTOM, BOTTOM_RIGHT, LEFT, CENTER, RIGHT, TOP_LEFT, TOP, TOP_RIGHT  }

        public readonly Rectangle ClipRect;
        public readonly Vector2 Center;
        public Vector2 Origin;

        public int Width { get; private set; }
        public int Height { get; private set; }

        public Transform Transform;

        /*
         *      Graphics
         */
        public bool IsVisible;

        public Color Color = Color.White;
        public SpriteEffects Effects = SpriteEffects.None;
        public bool IsMirrored
        {
            get { return (Effects & SpriteEffects.FlipHorizontally) == SpriteEffects.FlipHorizontally; }

            set { Effects = value ? (Effects | SpriteEffects.FlipHorizontally) : (Effects & ~SpriteEffects.FlipHorizontally); }
        }

        public bool IsFlipped
        {
            get { return (Effects & SpriteEffects.FlipVertically) == SpriteEffects.FlipVertically; }

            set { Effects = value ? (Effects | SpriteEffects.FlipVertically) : (Effects & ~SpriteEffects.FlipVertically); }
        }
        // Notes: the Get will check if Effects contain the Flip tag
        //          while the Set will add or remove the tag.


        protected Renderable(Rectangle sourceRect)
        {
            IsVisible = true;

            ClipRect = sourceRect;
            Origin = Center = new Vector2(sourceRect.Width * 0.5f, sourceRect.Height * 0.5f);
            Width = ClipRect.Width;
            Height = ClipRect.Height;

            Transform = new Transform();
        }

        public override void Update()
        {
            // Notes: This is overall innefective. Idea solution would be to update only when necessary.
            //          Although, that would increase code size and create too many conditions.
            // Force Update on All Children
            if (Transform.Children.Count > 0)
                Transform.UpdateChildren();
        }

        public override void Render()
        {

        }

        #region Image Points

        public void SetOrigin(float x, float y)
        {
            Origin.X = x;
            Origin.Y = y;
        }

        public void SetOrigin(Anchor anchor)
        {
            switch(anchor)
            {
                case Anchor.BOTTOM_LEFT:
                    Origin.X = ClipRect.Left;
                    Origin.Y = ClipRect.Bottom;
                    break;
                case Anchor.BOTTOM:
                    Origin.X = ClipRect.Center.X;
                    Origin.Y = ClipRect.Bottom;
                    break;
                case Anchor.BOTTOM_RIGHT:
                    Origin.X = ClipRect.Right;
                    Origin.Y = ClipRect.Bottom;
                    break;
                case Anchor.TOP_LEFT:
                    Origin.X = ClipRect.Left;
                    Origin.Y = ClipRect.Top;
                    break;
                case Anchor.TOP:
                    Origin.X = ClipRect.Center.X;
                    Origin.Y = ClipRect.Top;
                    break;
                case Anchor.TOP_RIGHT:
                    Origin.X = ClipRect.Right;
                    Origin.Y = ClipRect.Top;
                    break;
                case Anchor.LEFT:
                    Origin.X = ClipRect.Left;
                    Origin.Y = ClipRect.Top;
                    break;
                case Anchor.CENTER:
                    Origin.X = ClipRect.Center.X;
                    Origin.Y = ClipRect.Center.Y;
                    break;
                case Anchor.RIGHT:
                    Origin.X = ClipRect.Right;
                    Origin.Y = ClipRect.Top;
                    break;
            }
        }

        public void CenterOrigin()
        {
            Origin.X = Width / 2f;
            Origin.Y = Height / 2f;
        }

        public void JustifyOrigin(Vector2 at)
        {
            Origin.X = Width * at.X;
            Origin.Y = Height * at.Y;
        }

        public void JustifyOrigin(float x, float y)
        {
            Origin.X = Width * x;
            Origin.Y = Height * y;
        }

        #endregion

        #region Utils

        public override string ToString()
        {
            return string.Format("[Renderable]: {0} Origin: {1} Center: {2} Transform:\n {3} ", ClipRect, Origin, Center, Transform);
        }

        #endregion
    }
}

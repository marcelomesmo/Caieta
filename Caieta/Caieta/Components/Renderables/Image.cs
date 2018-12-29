using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Caieta
{
    public class Image
    {
        public enum Anchor { BOTTOM_LEFT, BOTTOM, BOTTOM_RIGHT, LEFT, CENTER, RIGHT, TOP_LEFT, TOP, TOP_RIGHT }

        public enum AnchorPolicy { ThisSprite, CurrentAnimation, AllAnimations }

        public Texture2D Texture;

        /*
         *      Size & Position   
         */
        public readonly Rectangle ClipRect;
        public readonly Vector2 Center;
        public Vector2 Origin;
        //public Dictionary<string, Vector2> ImagePoints;

        public int Width { get; private set; }
        public int Height { get; private set; }

        public Image(Texture2D texture, Rectangle sourceRect)
        {
            Texture = texture;

            ClipRect = sourceRect;
            Origin = Center = new Vector2(sourceRect.Width * 0.5f, sourceRect.Height * 0.5f);
            Width = ClipRect.Width;
            Height = ClipRect.Height;
        }
        public Image(Texture2D texture) : this(texture, new Rectangle(0, 0, texture.Width, texture.Height)) { }
        public Image(Texture2D texture, int x, int y, int width, int height) : this(texture, new Rectangle(x, y, width, height)) { }

        #region Origin

        public void SetOrigin(Anchor anchor)
        {
            // ClipRect = new Rectangle(0, 0, clip.Width, clip.Height)
            switch (anchor)
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
                    Origin.Y = ClipRect.Center.Y;
                    break;
                case Anchor.CENTER:
                    Origin.X = ClipRect.Center.X;
                    Origin.Y = ClipRect.Center.Y;
                    break;
                case Anchor.RIGHT:
                    Origin.X = ClipRect.Right;
                    Origin.Y = ClipRect.Center.Y;
                    break;
            }
        }

        public void SetOrigin(float x, float y)
        {
            Origin.X = x;
            Origin.Y = y;
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

        #region Image Points

        /*
         * 
        #region Image Points

        public Entity AddImagePoint(string name, Vector2 image_point)
        {
            ImagePoint.Add(name, image_point);

            return this;
        }

        #endregion
        */

        #endregion

        #region Utils

        public Image GetSubtexture(Rectangle ClipRect)
        {
            return new Image(Texture, ClipRect);
        }

        /*
        public Spritee GetSubtexture(int x, int y, int width, int height, Image applyTo = null)
        {
            if (applyTo == null)
                return new Image(Texture, x, y, width, height);
            else
            {
                applyTo.Texture = Texture;

                //applyTo.ClipRect = GetRelativeRect(x, y, width, height);
                applyTo.Width = width;
                applyTo.Height = height;
                //applyTo.Center = applyTo.Origin = new Vector2(Width, Height) * 0.5f;

                return applyTo;
            }
        }*/

        public override string ToString()
        {
            return string.Format("[Renderable]: {0} Origin (Local Offset): {1} Center: {2}", ClipClipRect, Origin, Center);
        }

        #endregion
    }
}

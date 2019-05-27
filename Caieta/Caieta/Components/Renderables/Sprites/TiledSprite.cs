using System;
using Caieta.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Caieta.Components.Renderables.Sprites
{
    public class TiledSprite : Renderable
    {
        public enum Anchor { BOTTOM_LEFT, BOTTOM, BOTTOM_RIGHT, LEFT, CENTER, RIGHT, TOP_LEFT, TOP, TOP_RIGHT }

        public int Width;
        public int Height;
        //public int SpriteWidth { get; private set; }
        //public int SpriteHeight { get; private set; }
        public int TilesWide, TilesHigh;

        public Texture2D Texture { get; private set; }

        private Rectangle Frame;
        public Vector2 Center { get; private set; }
        public Vector2 Origin;

        public TiledSprite(Texture2D texture, Rectangle sourceRect, int regionWidth, int regionHeight)
        {
            Width = regionWidth;
            Height = regionHeight;

            TilesWide = regionWidth / sourceRect.Width;
            TilesHigh = regionHeight / sourceRect.Height;

            //SpriteWidth = spriteWidth;
            //SpriteHeight = spriteHeight;

            Frame = sourceRect;

            Origin = Center = new Vector2(Width * 0.5f, Height * 0.5f);

            Texture = texture;

        }
        public TiledSprite(Texture2D texture, Rectangle sourceRect, Vector2 tiles) : this(texture, sourceRect, sourceRect.Width * (int)tiles.X, sourceRect.Height * (int)tiles.Y) { }
        public TiledSprite(string texture_path, Rectangle sourceRect, Vector2 tiles) : this(Resources.Get<Texture2D>(texture_path), sourceRect, tiles) { }
        public TiledSprite(string texture_path, Rectangle sourceRect, int regionWidth, int regionHeight) : this(Resources.Get<Texture2D>(texture_path), sourceRect, regionWidth, regionHeight) { }
       
        Vector2 Offset;
        public override void Render()
        {
            base.Render();

            for(int w = 0; w < TilesWide; w++)
            {
                for(int h = 0; h < TilesHigh; h++ )
                {
                    Offset = new Vector2(w * Frame.Width, h * Frame.Height);

                    Graphics.Draw(
                        Texture,
                        Entity.Transform.Position + Offset,
                        Frame,
                        Color * (Opacity / 100f),
                        Entity.Transform.Rotation,
                        Origin,
                        Entity.Transform.Scale,
                        Effects,
                        0);
                }
            }
        }

        #region Utils

        public override string ToString()
        {
            return string.Format("[TiledSprite]: Component: {0} Texture: {1} Width: {2} Height: {3}", Frame, Texture, Width, Height);
        }

        #endregion

        #region Origin

        public void SetOrigin(Anchor anchor)
        {
            // Notes: Origin is relative to local position, hence X: 0 and Y: 0.
            Rectangle ClipRect = new Rectangle(0, 0, Width, Height);

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
    }
}

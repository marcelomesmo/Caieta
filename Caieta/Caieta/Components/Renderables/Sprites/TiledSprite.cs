using System;
using Caieta.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Caieta.Components.Renderables.Sprites
{
    public class TiledSprite : Sprite
    {
        public int RegionWidth, RegionHeight;
        public int TilesWide => RegionWidth / CurrentAnimation.FrameWidth;
        public int TilesHigh => RegionHeight / CurrentAnimation.FrameHeight;

        public TiledSprite(int regionWidth, int regionHeight) : base()
        {
            RegionWidth = regionWidth;
            RegionHeight = regionHeight;

            //TilesWide = regionWidth / sourceRect.Width;
            //TilesHigh = regionHeight / sourceRect.Height;
        }
        public TiledSprite(Texture2D texture, Rectangle sourceRect, int regionWidth, int regionHeight) : base(texture, sourceRect)
        {
            RegionWidth = regionWidth;
            RegionHeight = regionHeight;

           //TilesWide = regionWidth / sourceRect.Width;
            //TilesHigh = regionHeight / sourceRect.Height;
        }
        public TiledSprite(Texture2D texture, Rectangle sourceRect, Vector2 tiles) : this(texture, sourceRect, sourceRect.Width * (int)tiles.X, sourceRect.Height * (int)tiles.Y) { }
        public TiledSprite(string texture_path, Rectangle sourceRect, Vector2 tiles) : this(Resources.Get<Texture2D>(texture_path), sourceRect, tiles) { }
        public TiledSprite(string texture_path, Rectangle sourceRect, int regionWidth, int regionHeight) : this(Resources.Get<Texture2D>(texture_path), sourceRect, regionWidth, regionHeight) { }
       
        Vector2 Offset;
        public override void Render()
        {
            base.Render();

            if (!IsVisible || CurrentAnimation == null)
                return;

            for (int w = 0; w < TilesWide; w++)
            {
                for(int h = 0; h < TilesHigh; h++ )
                {
                    Offset = new Vector2(w * CurrentAnimation.Frame.Width, h * CurrentAnimation.Frame.Height);

                    if (Engine.IsPixelPerfect)
                        DrawPosition = new Vector2((float)Math.Round(Entity.Transform.Position.X), (float)Math.Round(Entity.Transform.Position.Y));
                    else
                        DrawPosition = Entity.Transform.Position;

                        Graphics.Draw(CurrentAnimation.Sheet, DrawPosition + Offset, CurrentAnimation.Frame, Color * (Opacity / 100f), Entity.Transform.Rotation, CurrentAnimation.Origin, Entity.Transform.Scale, Effects, 0);
                }
            }
        }

        #region Utils

        public new TiledSprite Add(Animation animation)
        {
            if (Animations.ContainsKey(animation.Name))
                Debug.ErrorLog("[TiledSprite]: Animation with name '" + animation.Name + "' already added.");
            else
                Animations.Add(animation.Name, animation);

            return this;
        }

        //public override string ToString()
        //{
        //    return string.Format("[TiledSprite]: Component: {0} Texture: {1} RegionWidth: {2} RegionHeight: {3}", RegionWidth, RegionHeight);
        //}

        #endregion
    }
}

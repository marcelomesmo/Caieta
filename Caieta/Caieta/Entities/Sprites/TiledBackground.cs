using System;
using Caieta.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Caieta
{
    public class TiledBackground : Entity
    {
        public int Width;   // Make new if need to override
        public int Height;

        //public Sprite Sprite { get; private set; }
        public Texture2D Texture { get; private set; }

        public TiledBackground(string entityname, Texture2D texture, float width, float height, bool initial_visibility = true) : base(entityname, initial_visibility)
        {
            Width = (int)width;
            Height = (int)height;

            //Transform.Position = new Vector2(Graphics.Width / 2, Graphics.Height / 2);

            Debug.Log("[TiledBackground]: " + entityname + " Width " + Width + " Height " + Height);

            //Sprite = new Sprite(texture);
            //Sprite.CurrentAnimation.AdjustRect(new Rectangle(0, 0, Width, Height));

            //Debug.Log("[TiledBackground]: Sprite " + Sprite);
            //Add(Sprite);

            Texture = texture;

        }
        public TiledBackground(string entityname, Texture2D texture, Vector2 tiles, bool initial_visibility = true) : this(entityname, texture, texture.Width * tiles.X, texture.Height * tiles.Y, initial_visibility) { }

        public override void Update()
        {
            base.Update();

        }

        public override void Render()
        {
            base.Render();

            // Notes: Gambiarra
            Graphics.SpriteBatch.End();

            if (Engine.IsPixelPerfect)
                Graphics.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Engine.SceneManager.Camera.Matrix * Engine.ScreenMatrix);
            else
                Graphics.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.None, RasterizerState.CullNone, null, Engine.SceneManager.Camera.Matrix * Engine.ScreenMatrix);
            // Notes: Gambiarra


            // Real Drawing
            Graphics.Draw(Texture, Transform.Position, new Rectangle(0, 0, Width, Height), Color.White, Transform.Rotation, Vector2.Zero, Transform.Scale, SpriteEffects.None, 0);


            // Notes: Gambiarra
            Graphics.SpriteBatch.End();

            if(Engine.IsPixelPerfect)
                Graphics.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Engine.SceneManager.Camera.Matrix * Engine.ScreenMatrix);
            else
                Graphics.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Engine.SceneManager.Camera.Matrix * Engine.ScreenMatrix);
            // Notes: Gambiarra
        }
    }
}

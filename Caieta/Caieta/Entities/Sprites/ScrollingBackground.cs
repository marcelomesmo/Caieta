using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Caieta
{
    // Flappy Bird'like scrolling bg
    public class ScrollingBackground : TiledBackground
    {
        public Vector2 Speed;
        public Vector2 Direction;

        public ScrollingBackground(string entityname, Texture2D texture, Vector2 dir, Vector2 speed, bool initial_visibility = true) : base(entityname, texture, 0, 0, initial_visibility)
        {
            Direction.X = MathHelper.Clamp(dir.X, -1f, 1f);
            Direction.Y = MathHelper.Clamp(dir.Y, -1f, 1f);
            Speed = speed;

            if (Direction.X != 0f)
            {
                if (texture.Width < Graphics.Width)
                    Width = texture.Width * 3;
                else
                    Width = texture.Width * 2;
            }
            else
                Width = texture.Width;

            if(Direction.Y != 0f)
            {
                if (texture.Height < Graphics.Height)
                    Height = texture.Height * 3;
                else
                    Height = texture.Height * 2;
            }
            else
                Height = texture.Height;

            //speed.X != 0f ? 2 * texture.Width : texture.Width, speed.Y != 0f ? 2 * texture.Height : texture.Height

            // Fix positive direction (Right and Down)
            // Moving right
            if (Direction.X > 0)
                Transform.X -= texture.Width;
            // Moving down
            if (Direction.Y > 0)
                Transform.Y -= texture.Height;

            Debug.Log("[ScrollingBackground]: " + entityname + " Dir " + Direction + " Speed " + Speed + Transform);
        }

        public override void Update()
        {
            base.Update();

            // Notes: Could add a Direction Vector
            Transform.X += Direction.X * Speed.X * Engine.Instance.DeltaTime;
            Transform.Y += Direction.Y * Speed.Y * Engine.Instance.DeltaTime;
            //_sourceRect.X = (int)_scrollX;
            //_sourceRect.Y = (int)_scrollY;

            // Clamp to Texture size
            //Transform.X = MathHelper.Clamp(Transform.X, -Texture.Width, 0);
            //Transform.Y = MathHelper.Clamp(Transform.Y, -Texture.Height, 0);

            // Check if out of screen
            // Moving Right
            if (Direction.X > 0)
            {
                if (Transform.Position.X > -1)
                    Transform.X -= Texture.Width;
            }
            // Moving Left
            else if(Direction.X < 0)
            {
                if (Transform.Position.X < -Texture.Width)
                    Transform.X += Texture.Width;
            }

            // Moving Down
            if (Direction.Y > 0)
            {
                if (Transform.Position.Y > -1)
                    Transform.Y -= Texture.Height;
            }
            // Moving Up
            else if (Direction.Y < 0)
            {
                if (Transform.Position.Y < -Texture.Height)
                    Transform.Y += Texture.Height;
            }

            //Debug.Log("[ScrollingBackground]: Pos " + Transform.Position + " Texture Width " + Texture.Width + " Height " + Texture.Height);
        }
    }
}

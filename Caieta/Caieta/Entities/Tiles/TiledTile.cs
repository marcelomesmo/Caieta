using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Caieta
{
    public class TiledTile : Renderable
    {
        public Vector2 Position;
        public Rectangle ClipRect;

        TiledMap Parent;

        public TiledTile(TiledMap parent)
        {
            Parent = parent;
        }

        public override void Render()
        {
            base.Render();

            Graphics.Draw(
                Parent.Tileset,
                new Rectangle((int)Position.X, (int)Position.Y, Parent.TileWidth, Parent.TileHeight),
                ClipRect,
                Color * Opacity,
                Entity.Transform.Rotation,
                Entity.Transform.Position,
                Effects,
                0);
        }
    }
}

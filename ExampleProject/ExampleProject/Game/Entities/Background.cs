using System;
using Caieta;
using Caieta.Entities;
using Microsoft.Xna.Framework.Graphics;

namespace ExampleProject
{
    public class Background : Entity
    {
        public Background(string entityname, bool initial_visibility = true) : base(entityname, initial_visibility)
        {
        }

        public override void Create()
        {
            base.Create();

            var bg = Engine.Instance.Content.Load<Texture2D>("BgResTest");
            Add(new Sprite(bg));

        }
    }
}

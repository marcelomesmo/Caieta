using System;
using Caieta;
using Caieta.Entities;
using Microsoft.Xna.Framework;
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

            Transform.Position = new Vector2(Graphics.Width / 2, Graphics.Height / 2);

            //var bg = Engine.Instance.Content.Load<Texture2D>("BgResTest");
            //Add(new Sprite(bg));
            Add(new Sprite(Resources.Get<Texture2D>("BgResTest")));

            // Notes:
            //Add(new BoxCollider(Get<Sprite>().Width, Get<Sprite>().Height, -Get<Sprite>().Origin.X, -Get<Sprite>().Origin.Y));

            //Add(new BoxCollider(Get<Sprite>()));
            Add(new BoxCollider(20, 20, -100, -50));
            Add(new BoxCollider(30, 10, -50, -10));
        }

        public override void Update()
        {
            base.Update();

            //Debug.Log("Bg Box: Pos " + Transform.Position.X + " (X) " + Transform.Position.Y + " (Y) " +
            //                "Screen Pos " + Transform.ScreenPosition.X + " (X) " + Transform.ScreenPosition.Y + " (Y) ");
        }
    }
}

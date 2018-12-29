using System;
using Caieta;
using Caieta.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExampleProject
{
    public class Logo : Entity
    {
        Sprite Splash;

        public Logo(string entityname, bool initial_visibility = true) : base(entityname, initial_visibility)
        {
        }

        public override void Create()
        {
            base.Create();

            Transform.SetPosition(Graphics.Width / 2, Graphics.Height / 2);

            var sprite = Engine.Instance.Content.Load<Texture2D>("SplashScene/Tapioca_logo");
            Splash = new Sprite(sprite);
            // Splash = new Sprite(Resources.Get<Image>("logo"));
            // Splash = Resources.Get<Sprite>("logo");

            //Splash.SetOrigin(Renderable.Anchor.TOP_RIGHT);

            Add(Splash);
        }

        public override void Render()
        {
            base.Render();

            //Graphics.Draw(sprite);
        }
    }
}

using System;
using Caieta;
using Caieta.Entities;
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

            var sprite = Engine.Instance.Content.Load<Texture2D>("SplashScene/Tapioca_logo");

            Splash = new Sprite(sprite);
            Splash.Transform.SetPosition(Graphics.Width/2, Graphics.Height/2);

            Add(Splash);
        }

        public override void Render()
        {
            base.Render();

            //Graphics.Draw(sprite);
        }
    }
}

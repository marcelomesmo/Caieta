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

        BoxCollider Hitbox;

        public Logo(string entityname, bool initial_visibility = true) : base(entityname, initial_visibility)
        {
        }

        public override void Create()
        {
            base.Create();

            Transform.SetPosition(Graphics.Width / 2, Graphics.Height / 2);

            Splash = new Sprite(Resources.Get<Texture2D>("SplashScene/Tapioca_logo"));
            // Splash = Resources.Get<Sprite>("logo");

            //Splash.SetOrigin(Renderable.Anchor.TOP_RIGHT);
            Add(Splash);

            Hitbox = new BoxCollider(Splash);
            Add(Hitbox);
        }

        public override void Update()
        {
            base.Update();

            Hitbox.OnHold = () =>
            {
                Debug.Log("Hitbox Hold");
            };

            Hitbox.OnTap = () =>
            {
                Debug.Log("Hitbox Tap");
            };

            Hitbox.OnDoubleTap = () =>
            {
                Debug.Log("Hitbox Double Tap");
            };

            Hitbox.OnTouchedObject = () =>
            {
                Debug.Log("Hitbox Touched");
                Destroy();
            };

            Hitbox.OnMouseEnter = () =>
            {
                Debug.Log("Hitbox Mouse Enter");
            };

            Hitbox.OnMouseExit = () =>
            {
                Debug.Log("Hitbox Mouse Exit");
            };
        }
    }
}

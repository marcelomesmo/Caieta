using System;
using Caieta;
using Caieta.Entities;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ExampleProject
{
    public class Player : Entity
    {
        Sprite sprite;

        private float startX = 100, startY = 100;

        public Player(string entityname, bool initial_visibility = true) : base(entityname, initial_visibility)
        {
            OnCreate += Player_OnCreate;
        }

        private void Player_OnCreate()
        {

            /*
             * Platform
             */
            Add(new Platform());

            Get<Platform>().Transform.SetPosition(startX, startY);
            Get<Platform>().AddCollider("hitbox1", new BoxCollider(24, 24, 0, 0));
            Get<Platform>().AddCollider("hitbox2", new BoxCollider(5, 5, 5, 5));
            Get<Platform>().DefaultControls = true;

            /*
             * Sprite
             */
            sprite = new Sprite();
            sprite.Transform.SetPosition(startX, startY);

            var sheet1 = Engine.Instance.Content.Load<Texture2D>("Sheet/Sheet1");
            sprite.Add(new Animation("idle", sheet1, 1, 4, 300));

            var sheet2 = Engine.Instance.Content.Load<Texture2D>("Sheet/Sheet2");
            sprite.Add(new Animation("walk", sheet2, 1, 5, 300, 100, 50, 300, 300));

            sprite.SetAnimation("idle");

            sprite.SetOrigin(Renderable.Anchor.BOTTOM_LEFT);

            //Debug.Log(sprite);

            sprite.Transform.PinTo(Get<Platform>().Transform);

            Add(sprite);

            sprite.OnFinish += OnFinish;


            //Debug.Log(Get<Platform>());


            Debug.Log("[Player]: Created.");
        }

        public override void Update()
        {
            base.Update();

            if(Input.Keyboard.Pressed(Keys.Left) && !sprite.IsMirrored)
                sprite.IsMirrored = true;

            if (Input.Keyboard.Pressed(Keys.Right) && sprite.IsMirrored)
                sprite.IsMirrored = false;


            Get<Platform>().OnMove = () =>
            {
                if(!sprite.IsPlaying("walk"))
                    sprite.SetAnimation("walk");
            };

            Get<Platform>().OnStop = () =>
            {
                if (sprite.IsPlaying("walk"))
                    sprite.SetAnimation("idle");
            };
        }

        public void OnFinish(string name)
        {
            /*
            if (name.Equals("idle"))
                sprite.SetAnimation("walk");
            else if (name.Equals("walk"))
                sprite.SetAnimation("idle");
                */               
        }

        public override void Render()
        {
            base.Render();
        }
    }
}

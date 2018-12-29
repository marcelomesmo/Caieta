using System;
using Caieta;
using Caieta.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ExampleProject
{
    public class Player : Entity
    {
        Sprite sprite;

        //private readonly BoxCollider Hitbox = new BoxCollider(8, 11, -4, -11);
        //private readonly BoxCollider Duckbox = new BoxCollider(8, 6, -4, -6);

        BoxCollider Hitbox = new BoxCollider(12, 16);
        BoxCollider Anotherbox = new BoxCollider(5, 5, 5, 5);

        private float startX = 100, startY = 100;

        public Player(string entityname, bool initial_visibility = true) : base(entityname, initial_visibility)
        {
            OnCreate += Player_OnCreate;
        }

        private void Player_OnCreate()
        {
            Transform.Position = new Vector2(startX, startY);

            /*
             * Platform
             */
            Add(new Platform());
            Get<Platform>().DefaultControls = true;

            // Notes: Create these as objects to maintain reference inside object
            //          (instead of using a dictionary with a string)
            Hitbox.SetOrigin(Animation.Anchor.CENTER);

            Add(Hitbox);
            Add(Anotherbox);

            //

            /*
             * Sprite
             */
            sprite = new Sprite();

            var sheet1 = Engine.Instance.Content.Load<Texture2D>("Sheet/Sheet1");
            sprite.Add(new Animation("idle", sheet1, 1, 4, 300));

            var sheet2 = Engine.Instance.Content.Load<Texture2D>("Sheet/Sheet2");
            sprite.Add(new Animation("walk", sheet2, 1, 5, 300, 100, 50, 300, 300));

            /*
            sprite = new Sprite()
                .Add(new Animation("idle", Resources.Get<Texture2D>("sheet1"), 1, 4, 300))
                .Add(new Animation("walk", Resources.Get<Texture2D>("sheet2"), 1, 5, 300, 100, 50, 300, 300));
                */

            sprite.SetAnimation("idle");

            //sprite.SetOrigin(Animation.Anchor.LEFT, Animation.AnchorPolicy.AllAnimations);
            //sprite.SetOrigin(Animation.Anchor.TOP_LEFT);
            //sprite.SetOrigin(Animation.Anchor.CENTER);

            Add(sprite);


            Debug.Log("Player 1 " + sprite);

            sprite.OnFinish += OnFinish;

            Debug.Log("[Player]: Created.");
        }

        public override void Update()
        {
            base.Update();

            if (Input.Keyboard.Pressed(Keys.Left) && !sprite.IsMirrored)
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

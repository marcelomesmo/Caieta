using System;
using Caieta;
using Caieta.Components.Attributes;
using Caieta.Components.Utils;
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

        public BoxCollider Hitbox = new BoxCollider(12, 16, -6, -4);
        BoxCollider Anotherbox = new BoxCollider(5, 5);
        BoxCollider Otherbox = new BoxCollider(5, 5, -10, -10);

        private const float startX = 100, startY = 100;

        Timer TestTimer;

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
            Get<Platform>().DoubleJump = true;
            //Get<Platform>().JumpStrength = Get<Platform>().JumpStrength * 2;

            Add(Hitbox);
            Add(Anotherbox);
            Add(Otherbox);

            /*
             * Sprite
             */
            sprite = new Sprite();

            //var sheet1 = Engine.Instance.Content.Load<Texture2D>("Sheet/Sheet1");
            //var sheet1 = Resources.Load<Texture2D>("Sheet/Sheet1");
            //sprite.Add(new Animation("idle", sheet1, 1, 4, 300));

            //var sheet2 = Engine.Instance.Content.Load<Texture2D>("Sheet/Sheet2");
            //sprite.Add(new Animation("walk", sheet2, 1, 5, 300, 100, 50, 300, 300));

            sprite = new Sprite()
                .Add(new Animation("idle", Resources.Get<Texture2D>("Sheet/Sheet1"), 1, 4, 300))
                .Add(new Animation("walk", Resources.Get<Texture2D>("Sheet/Sheet2"), 1, 5, 300, 100, 50, 300, 300));

            sprite.SetAnimation("idle");

            //sprite.SetOrigin(Animation.Anchor.LEFT, Animation.AnchorPolicy.AllAnimations);
            //sprite.SetOrigin(Animation.Anchor.TOP_LEFT);
            //sprite.SetOrigin(Animation.Anchor.CENTER);

            Add(sprite);
            Debug.Log("Player 1 " + sprite);

            sprite.OnFinish += OnFinish;

            /*
             *      TIMER
             */
            TestTimer = new Timer(500);
            // Notes: Repeat with new Timer(500, true) or new Timer(500); TestTimer.IsRepeating = true;
            Add(TestTimer);


            /*
             *      CAMERA
             */
            //Add(new FollowCamera(FollowCamera.CameraPolicy.LERP));
            Add(new FollowCamera());

            Add(new DestroyOutside());

            Debug.Log("[Player]: Created.");
        }

        public override void Update()
        {
            base.Update();


            /*
             *      TEST TIMER
             * 
             */
            TestTimer.OnTime = () =>
            {
                Debug.Log("[Player]: Timer reached.");
            };

            if (Input.Keyboard.Pressed(Keys.U))
                TestTimer.Start();

            if (Input.Keyboard.Pressed(Keys.I))
                TestTimer.Pause();

            if (Input.Keyboard.Pressed(Keys.O))
                TestTimer.Resume();

            if (Input.Keyboard.Pressed(Keys.P))
                TestTimer.Stop();

            if ((TestTimer.IsRunning && !TestTimer.IsPaused) || Input.Keyboard.Pressed(Keys.I) || Input.Keyboard.Pressed(Keys.O) || Input.Keyboard.Pressed(Keys.P))
            {
                Debug.Log("[Player]: Timer is running: " + TestTimer.IsRunning + " paused: " + TestTimer.IsPaused + " shall repeat: " + TestTimer.IsRepeating +
               "\n with target time: " + TestTimer.TargetTime + "ms and current time: " + TestTimer.ElapsedTime + "ms.");
            }

            /*var from = SceneManager.Camera.Position;
            var target = CameraTarget;
            var multiplier = StateMachine.State == StTempleFall ? 8 : 1f;

            level.Camera.Position = from + (target - from) * (1f - (float)Math.Pow(0.01f / multiplier, Engine.DeltaTime));
            */

            // TODO: REMAKE THIS
            //Engine.SceneManager.Camera.Position = new Vector2(this.Transform.Position.X - Graphics.Width / 2, this.Transform.Position.Y - Graphics.Height / 2);
            //Engine.SceneManager.Camera.RoundPosition();

            Hitbox.OnTap = () =>
            {
                Debug.Log("Jump 1");
            };

            // Working
            /*if(Input.Touch.IsTouching(Hitbox))
            {
                Debug.Log("Jump 3");
            }*/

            Anotherbox.OnTap = () =>
            {
                Debug.Log("Jump 2");
            };

            // Mirror image
            if (Input.Keyboard.Pressed(Keys.Left) && !sprite.IsMirrored)
            {
                sprite.IsMirrored = true;

                //Anotherbox.IsMirrored = true;
                MirrorColliders(true);
            }

            if (Input.Keyboard.Pressed(Keys.Right) && sprite.IsMirrored)
            {
                sprite.IsMirrored = false;

                //Anotherbox.IsMirrored = false;
                MirrorColliders(false);
            }

            // Flip image
            if (Input.Keyboard.Pressed(Keys.Down) && !sprite.IsFlipped)
            {
                sprite.IsFlipped = true;

                Otherbox.IsFlipped = true;
            }

            if (Input.Keyboard.Pressed(Keys.Up) && sprite.IsFlipped)
            {
                sprite.IsFlipped = false;

                Otherbox.IsFlipped = false;
            }

            // Platform Movements
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

            Get<Platform>().OnJump = () =>
            {
                Debug.Log("[Player]: Action Jump.");
            };

            Get<Platform>().OnLand = () =>
            {
                Debug.Log("[Player]: Action Land.");
            };

            Get<Platform>().OnFall = () =>
            {
                Debug.Log("[Player]: Action Fall.");
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

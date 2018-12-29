using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Caieta
{
    public class Platform : Component
    {
        /*
         *      Movement
         */
        public Vector2 Velocity;

        /*
         *      (X) Horizontal Movement
         */
        public float MaxSpeed;
        public float Acceleration;
        public float Deceleration;

        /*
         *      (Y) Vertical Movement
         */
        public float JumpStrength;
        public float Gravity;
        public float MaxFallSpeed;

        public bool DoubleJump;
        public float JumpSustain;

        /*
         *      Movement Verifiers
         */
        public bool IsMoving { get; private set; }
        public bool IsJumping { get; private set; }
        public bool IsFalling { get; private set; }
        public bool IsOnFloor { get; private set; }
        public bool IsByWall { get; private set; }

        /*
         *      Controls
         */
        public bool DefaultControls;
        public bool IgnoreInput;

        /*
         *      Events
         */
        public Action OnJump;
        public Action OnFall;
        public Action OnLand;

        public Action OnMove;
        public Action OnStop;

        public Platform()
        {
            MaxSpeed = 90; //330;
            Acceleration = 1000; //1500;
            Deceleration = 1000; //1500;

            JumpStrength = -105; //650;
            Gravity = 900; //1500;
            MaxFallSpeed = 160; //1000;

            DoubleJump = false;
            JumpSustain = 0;

            DefaultControls = false;
            IgnoreInput = false;

            OnJump = null;
            OnFall = null;
            OnLand = null;

            OnMove = null;
            OnStop = null;
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update()
        {
            base.Update();

            //if(!IgnoreInput)
            if (DefaultControls)
            {
                Vector2 previousPosition = Entity.Transform.Position;

                if (Input.Keyboard.IsMoving())
                {
                    // Make it only move if Left or Right
                    if (!IsMoving)
                        IsMoving = true;

                    // Trigger On Movement
                    if(OnMove != null)
                    {
                        OnMove();
                        OnMove = null;
                    }

                    Velocity.X += Input.Keyboard.Direction.X * Acceleration * Engine.Instance.DeltaTime;
                    Velocity.X = MathHelper.Clamp(Velocity.X, -MaxSpeed, MaxSpeed);
                }
                else
                {
                    if (IsMoving)
                        IsMoving = false;

                    // Trigger On Stop
                    if(OnStop != null)
                    {
                        OnStop();
                        OnStop = null;
                    }

                    if (Velocity.X > 0)
                    {
                        Velocity.X -= Deceleration * Engine.Instance.DeltaTime;
                        Velocity.X = MathHelper.Clamp(Velocity.X, -MaxSpeed, MaxSpeed);
                    }
                    else if(Velocity.X < 0)
                    {
                        Velocity.X = 0;
                    }
                }

                // velocity.Y = DoJump(velocity.Y, gameTime);
                //Velocity.Y = MathHelper.Clamp(Velocity.Y + Gravity * Engine.Instance.DeltaTime, -MaxFallSpeed, MaxFallSpeed);
                // 
                // Prevent the player from running faster than his top speed.            
                //velocity.X = MathHelper.Clamp(velocity.X, -MaxMoveSpeed, MaxMoveSpeed);

                Entity.Transform.Position += Velocity * Engine.Instance.DeltaTime;
                //Debug.Log(Transform);

                /*foreach (BoxCollider box in Colliders.Values)
                    box.Update(Transform);

                // If the player is now colliding with the level, separate them.
                //HandleCollisions();
                foreach (var _CurrLayer in Engine.SceneManager.SceneLayers())
                {
                    foreach (var ent in _CurrLayer.Entities)
                    {
                        if (ent.Get<Solid>() != null)
                        {
                            foreach (BoxCollider box in Colliders.Values)
                                if (box.IsOverlapping(ent.Get<Solid>().box))
                                {
                                    Transform.Position = previousPosition;

                                    // Trigger On Colision
                                    if (OnCollision != null)
                                    {
                                        OnColission(ent.Name);
                                        OnColission = null;
                                    }
                                }
                        }
                    }
                }

                // If the collision stopped us from moving, reset the velocity to zero.
                if (Transform.Position.X == previousPosition.X)
                    Velocity.X = 0;

                if (Transform.Position.Y == previousPosition.Y)
                    Velocity.Y = 0;*/
            }
        }

        public override void Render()
        {
            base.Render();

            // Notes: Could implement a predictable route and speed vectors here in the future.
        }

        #region Movement

        public void Move(Vector2 MoveDirection)
        {
            // If there are no colliders: move indistinguible (?)
            //if (Entity.GetAll<Collider>().Count <= 0)
            //{
                Entity.Transform.Position += MoveDirection * Engine.Instance.DeltaTime;
              //  return;
            //}
        }
        public void MoveX(int x)
        {
            Move(new Vector2(x, 0));
        }
        public void MoveY(int y)
        {
            Move(new Vector2(0, y));
        }

        // Notes: Need to implement this.
        public void SimulateControl(InputDirection direction)
        {
            switch(direction)
            {
                case InputDirection.LEFT:
                    break;
                case InputDirection.RIGHT:
                    break;
                // DoJump
                case InputDirection.UP:
                    break;
                // Force fall from a jump-thru
                case InputDirection.DOWN:
                    break;
            }
        }

        #endregion

        #region Utils

        public override string ToString()
        {
            return string.Format("[Platform]: Colliders: {0} Transform:\n {1} ", Entity.Transform);
        }

        #endregion
    }
}

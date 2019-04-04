using System;
using System.Collections.Generic;
using Caieta.Entities;
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
        public Vector2 MoveDirection;

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

        public bool CanJump;
        public float JumpSustain;
        private float _JumpTime;
        public float JumpControlPower;

        public bool DoubleJump; // Notes: Turn ON DoubleJump and change JumpCount to allow for multiple jumps.
        private int _JumpCount;
        public int MAX_JUMP_COUNT = 2;
        public bool CanJumpWhileFalling; // Notes: Turn ON to allow player to jump while falling from ledges.

        /*
         *      Movement Verifiers
         */
        public bool IsMoving { get; private set; }
        public bool IsJumping { get; private set; }
        public bool IsFalling { get; private set; }
        public bool IsOnFloor { get; private set; }
        public Dictionary<string, bool> IsByWall { get; private set; }

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
            Gravity = 300; //900; //1500;
            MaxFallSpeed = 160; //1000;

            CanJump = true;
            DoubleJump = false;
            CanJumpWhileFalling = false;
            _JumpCount = 0;
            JumpSustain = 0; //100f;
            _JumpTime = 0;

            DefaultControls = false;
            IgnoreInput = false;

            OnJump = null;
            OnFall = null;
            OnLand = null;

            OnMove = null;
            OnStop = null;

            IsByWall = new Dictionary<string, bool>();
            IsByWall.Add("Left", false);
            IsByWall.Add("Right", false);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update()
        {
            base.Update();

            /* 
             *      DEFAULT CONTROLS
             */
            if (DefaultControls && !IgnoreInput)
            {
                if (Input.Keyboard.IsMoving())
                {
                    MoveDirection = Input.Keyboard.Direction;
                }

                if (Input.GamePads[0].IsMoving())
                {
                    MoveDirection = Input.GamePads[0].Direction;
                }

                if (Input.Keyboard.Hold(Keys.Space) || Input.GamePads[0].Hold(Buttons.A))
                {
                    MoveDirection.Y = -1;
                }

                //Debug.Log("[Platform]: Move Direction " + MoveDirection.X + " " + MoveDirection.Y + " (X,Y)");
            }

            /* 
             *      HORIZONTAL MOVEMENT
             */
            if (MoveDirection.X != 0 && !IsMoving)
            {
                IsMoving = true;

                // Trigger On Movement
                OnMove?.Invoke();
                //Debug.Log("[Platform]: On Move trigger.");
            }
            else if (MoveDirection.X == 0)
            {
                if (Velocity.X > 0)
                    Velocity.X -= Deceleration * Engine.Instance.DeltaTime;
                else if (Velocity.X < 0)
                {
                    Velocity.X = 0;

                    IsMoving = false;

                    // Trigger On Stop
                    OnStop?.Invoke();
                    //Debug.Log("[Platform]: On Stop trigger.");
                }
            }

            Velocity.X += MoveDirection.X * Acceleration * Engine.Instance.DeltaTime;
            // Prevent the player from running faster than his top speed.  
            Velocity.X = MathHelper.Clamp(Velocity.X, -MaxSpeed, MaxSpeed);

            /* 
             *      VERTICAL MOVEMENT
             */
            // Notes: Can only jump if player has released the buttons.
            // Jump check
            if(MoveDirection.Y == 0)
                CanJump = true;

            // Jump
            if (MoveDirection.Y == -1 && CanJump &&
                (IsOnFloor || (DoubleJump && _JumpCount < MAX_JUMP_COUNT)) )
            {
                CanJump = false;

                // Notes: Add double jump and max jump here
                _JumpCount++;
                //Debug.Log("[Platform]: Double Jump: " + DoubleJump + " Jump count: " + _JumpCount);

                IsJumping = true;
                IsOnFloor = false;

                // Trigger On Jump
                OnJump?.Invoke();
                //Debug.Log("[Platform]: On Jump trigger.");

                Velocity.Y = JumpStrength;
            }

            if (IsJumping)
            {
                _JumpTime += Engine.Instance.DeltaTime * 1000;

                // If we are in the ascent of the jump
                if (_JumpTime > 0.0f && _JumpTime <= JumpSustain && MoveDirection.Y == -1)
                    // Fully override the vertical velocity with a power curve that gives players more control over the top of the jump
                    Velocity.Y = JumpStrength * (_JumpTime / JumpSustain);
                else
                {
                    // Reached the apex of the jump
                    _JumpTime = 0.0f;

                    // Fall
                    if(Velocity.Y > 0)
                    {
                        IsJumping = false;
                        IsFalling = true;

                        // Trigger On Fall
                        OnFall?.Invoke();
                        //Debug.Log("[Platform]: On Fall trigger.");
                    }
                }
            }

            // Apply Gravity
            Velocity.Y = MathHelper.Clamp(Velocity.Y + Gravity * Engine.Instance.DeltaTime, -MaxFallSpeed, MaxFallSpeed);
            // Prevent the player from falling faster than gravity;

            // Notes: Raycast check if falling from ledges.
            // Fall Threshold = MaxFall / 4
            if (Velocity.Y > MaxFallSpeed/4 && IsOnFloor && !IsJumping)
            {
                IsOnFloor = false;
                IsFalling = true;

                // Trigger On Fall
                OnFall?.Invoke();

                if(!CanJumpWhileFalling) _JumpCount = MAX_JUMP_COUNT;
            }

            /* 
             *      MOVE PLAYER
             */
            // Notes: Could make this into a private later to optimize/improve performance.
            var Colliders = Entity.GetAll<Collider>();

            // If there are no colliders: move indistinguible (??)
            if (Colliders.Count <= 0)
            {
                Entity.Transform.Position += MoveDirection * Velocity * Engine.Instance.DeltaTime;
                return;
            }

            // Try to move Entity
            Vector2 previousPosition = Entity.Transform.Position;
            Entity.Transform.Position += Velocity * Engine.Instance.DeltaTime;
            //Entity.Transform.Position = new Vector2((float)Math.Round(Entity.Transform.Position.X), (float)Math.Round(Entity.Transform.Position.Y));
            //Debug.Log(Transform);

            /* 
            *      HANDLE COLLISIONS
            */
            // Check collision for each collider
            foreach (Collider c in Colliders)
            {
                // Check collision with other entities
                foreach (var ent in Engine.SceneManager.SceneEntities())
                {
                    // Notes: Add ignore tag here.
                    // Ignore Tag

                    // Ignore Self
                    if (ent.Name == Entity.Name)
                        continue;

                    var ent_colliders = ent.GetAll<Collider>();
                     
                    foreach (Collider ec in ent_colliders)
                    {
                        if (c.IsOverlapping(ec))
                        {
                            // Trigger On Colision
                            c.OnCollision?.Invoke(ent);
                            //Debug.Log("[Platform]: On Collision trigger with '" + ent.Name + "'.");
                            // Notes: We can know exactly which collider hit the entity,
                            //          but this way we cant know which collider we collided with.

                            // Notes: Check these ones only if ec Collider is Solid
                            //          i.e: if(c is Solid && ec is Solid).
                            // Prevent Movement if both colliders are solid
                            if (!c.IsTrigger && !ec.IsTrigger)
                            {
                                // Notes: Make this relative to proper X or Y collision.
                                //Entity.Transform.Position = previousPosition;
                                // Notes: Make this proportional to the ec Collider position.

                                // If on bounds of object & not on floor
                                if(previousPosition.X + c.Origin.X + c.Width > ec.AbsolutePosition.X &&
                                    previousPosition.X + c.Origin.X < ec.AbsolutePosition.X + ec.Width)
                                {
                                    // Check if collision was from Top
                                    if (previousPosition.Y + c.Origin.Y + c.Height < ec.AbsolutePosition.Y)
                                    {
                                        // Collide on Y
                                        //previousPosition.Y = ec.AbsolutePosition.Y - 1;
                                        Entity.Transform.Position = new Vector2(Entity.Transform.Position.X, previousPosition.Y);

                                        IsOnFloor = true;

                                        // Trigger On Landed
                                        if (IsFalling || IsJumping)
                                        {
                                            _JumpCount = 0;

                                            IsJumping = false;
                                            IsFalling = false;

                                            OnLand?.Invoke();
                                            //Debug.Log("[Platform]: On Land trigger.");
                                        }
                                    }
                                    // Collide from Bottom
                                    else if (previousPosition.Y + c.Origin.Y > ec.AbsolutePosition.Y + ec.Height)
                                    {
                                        // Collide on Y
                                        //previousPosition.Y = ec.AbsolutePosition.Y + ec.Height + 1;
                                        Entity.Transform.Position = new Vector2(Entity.Transform.Position.X, previousPosition.Y);
                                        //Debug.Log("[Platform]: Hit Roof.");
                                    }
                                }

                                // Check if collision was from Left
                                if (previousPosition.X + c.Origin.X > ec.AbsolutePosition.X + ec.Width)
                                {
                                    //previousPosition.X = ec.AbsolutePosition.X + ec.Width + 1;
                                    Entity.Transform.Position = new Vector2(previousPosition.X, Entity.Transform.Position.Y);
                                    IsByWall["Left"] = true;
                                    //Debug.Log("[Platform]: Hit Wall Left.");
                                }
                                // Check if collision was from Right
                                else if (previousPosition.X + c.Origin.X + c.Width < ec.AbsolutePosition.X)
                                {
                                    //previousPosition.X = ec.AbsolutePosition.X - 1;
                                    Entity.Transform.Position = new Vector2(previousPosition.X, Entity.Transform.Position.Y);
                                    IsByWall["Right"] = true;
                                    //Debug.Log("[Platform]: Hit Wall Right.");
                                }
                            }

                            // If the collision stopped us from moving, reset the velocity to zero.
                            if(Entity.Transform.Position.X == previousPosition.X)
                                Velocity.X = 0;

                            if(Entity.Transform.Position.Y == previousPosition.Y)
                                Velocity.Y = 0;
                        }
                    }
                }
            }

            // Clean Wall connection if moving left or right
            if (Velocity.X > 0 && IsByWall["Left"])
                IsByWall["Left"] = false;
            if (Velocity.X < 0 && IsByWall["Right"])
                IsByWall["Right"] = false;

            // Clean Direction for next Update
            MoveDirection = new Vector2(0,0);
        }

        public override void Render()
        {
            base.Render();

            // Notes: Could implement a predictable route and speed vectors here in the future.
        }

        #region Movement

        // Notes: Need to implement this.
        public void SimulateControl(InputDirection direction)
        {
            switch(direction)
            {
                case InputDirection.LEFT:
                    MoveDirection.X = -1;
                    break;

                case InputDirection.RIGHT:
                    MoveDirection.X = 1;
                    break;
                // DoJump
                case InputDirection.UP:
                    MoveDirection.Y = -1;
                    break;
                // Force fall from a jump-thru
                case InputDirection.DOWN:
                    MoveDirection.Y = 1;
                    break;
            }
        }

        #endregion

        #region Utils

        public override string ToString()
        {
            return string.Format("[Platform]: Velocity: {0} Move Direction: {1} MaxSpeed: {2} Acceleration {3} Deceleration {4} Jump Strength: {5} Gravity {6} Max Fall Speed: {7} Jump Sustain: {8} Jump Control Power: {9} Double Jump: {10} Max Jump Ammount: {11} Can Jump Falling from Ledge: {12}\n    Is Moving {13} IsJumping {14} IsFalling {15} IsOnFloor {16} Is By Wall LEFT {17} Is By Wall RIGHT {18}\n Default Controls: {19} Ignore Input: {20}",
            Velocity, MoveDirection, MaxSpeed, Acceleration, Deceleration, JumpStrength, Gravity, MaxFallSpeed, JumpSustain, JumpControlPower, DoubleJump, MAX_JUMP_COUNT, CanJumpWhileFalling, IsMoving, IsJumping, IsFalling, IsOnFloor, IsByWall["left"], IsByWall["right"], DefaultControls, IgnoreInput);
        }

        #endregion
    }
}

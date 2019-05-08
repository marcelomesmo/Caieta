using System;
using System.Collections.Generic;
using Caieta.Components.Attributes;
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

        public Vector2 Force { get; private set; }
        private float _ForceTime;
        private float _ForceTotalTime;
        private bool isApplyingForce;

        /*
         *      (X) Horizontal Movement
         */
        public float MaxSpeed { get; set; }
        public float Acceleration { get; set; }
        public float Deceleration { get; set; }

        /*
         *      (Y) Vertical Movement
         */
        public float JumpStrength { get; set; }
        public float Gravity { get; set; }
        public float MaxFallSpeed { get; set; }

        public bool CanJump { get; set; }
        public float JumpSustain { get; set; }
        private float _JumpTime;
        public float JumpControlPower;
        public bool CanJumpWhileFalling; // Notes: Turn ON to allow player to jump while falling from ledges.

        public bool CanDoubleJump; // Notes: Turn ON DoubleJump and change MAX_JUMP_COUNT to allow for multiple jumps.
        public int JumpCount;
        public int MAX_JUMP_COUNT = 2;

        public bool CanSlide;
        public float MaxSlideSpeed;
        public bool CanWallJump;
        public float WallJumpStrength;


        /*
         *      Movement Verifiers
         */
        public bool IsMoving { get; private set; }
        public bool IsJumping { get; private set; }
        public bool IsFalling { get; private set; }
        public bool IsSliding { get; private set; }
        public bool IsOnFloor { get; private set; }
        public bool IsByWallLeft { get; private set; }
        public bool IsByWallRight { get; private set; }

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

        /*
         *      Colliders
         */
        private List<Collider> Colliders;

        public Platform()
        {
            MaxSpeed = 160; //330;
            Acceleration = 1000; //1500;
            Deceleration = 750; //1500;

            _ForceTime = 0;
            _ForceTotalTime = 200;

            JumpStrength = -190; //650;
            Gravity = 500; //900; //1500;
            MaxFallSpeed = 1000; //1000;
            MaxSlideSpeed = 500;
            WallJumpStrength = JumpStrength;

            CanJump = true;
            CanDoubleJump = false;
            CanJumpWhileFalling = false;
            JumpCount = 0;
            JumpSustain = 0; //100f;
            _JumpTime = 0;

            DefaultControls = false;
            IgnoreInput = false;

            OnJump = null;
            OnFall = null;
            OnLand = null;

            OnMove = null;
            OnStop = null;

            IsByWallLeft = false;
            IsByWallRight = false;
        }

        public override void Initialize()
        {
            base.Initialize();

        }

        public override void Update()
        {
            base.Update();

            UpdateDefaultControls();

            UpdateForce();

            UpdateHorizontalMove();

            Velocity.X += MoveDirection.X * Acceleration * Engine.Instance.DeltaTime;
            // Prevent the player from running faster than his top speed.  
            Velocity.X = MathHelper.Clamp(Velocity.X, -MaxSpeed, MaxSpeed);

            UpdateVerticalMove();

            // Prevent the player from falling faster than gravity;
            Velocity.Y = MathHelper.Clamp(Velocity.Y + Gravity * Engine.Instance.DeltaTime, IsSliding ? -MaxSlideSpeed : -MaxFallSpeed, IsSliding ? MaxSlideSpeed : MaxFallSpeed);

            Colliders = Entity.GetAll<Collider>();

            // If there are no colliders: move indistinguible
            if (Colliders.Count <= 0)
            {
                Entity.Transform.Position += MoveDirection * (Force + Velocity) * Engine.Instance.DeltaTime;
                return;
            }

            HandleCollisions();

            // Clean Direction for next Update
            MoveDirection = new Vector2(0, 0);
            if (!isApplyingForce)
                Force = new Vector2(0, 0);
        }

        public override void Render()
        {
            base.Render();

            // Notes: Could implement a predictable route, raycast and speed vectors here in the future.

            // Raycast
            //Graphics.DrawLine(Entity.Transform.Position, LeftRay, Color.White);
        }

        #region Collisions
        Vector2 previousPosition;
        bool hasWallLeft, hasWallRight;
        private void HandleCollisions()
        {
            hasWallLeft = false;
            hasWallRight = false;

            // Try to move Entity
            previousPosition = Entity.Transform.Position;
            Entity.Transform.Position += (Force + Velocity) * Engine.Instance.DeltaTime;

            // Check collision for each collider
            foreach (Collider collider in Colliders)
            {
                // Check collision with other entities
                // Notes: Should get neighbors here
                foreach (var neighbor in Engine.SceneManager.SceneEntities())
                {
                    // Notes: Add ignore tag here.
                    // Ignore Tag

                    // Ignore Self
                    if (neighbor.Name == Entity.Name)
                        continue;

                    var neighborColliders = neighbor.GetAll<Collider>();

                    foreach (Collider otherCollider in neighborColliders)
                    {
                        if (collider.IsOverlapping(otherCollider))
                        {
                            collider.OnCollision?.Invoke(neighbor);
                            //Debug.Log("[Platform]: On Collision trigger with '" + neighbor.Name + "'.");

                            CheckOneWayPlatform(collider, otherCollider);

                            CheckSolid(collider, otherCollider);

                            // If the collision stopped us from moving, reset the velocity to zero.
                            if (Entity.Transform.Position.X == previousPosition.X)
                                Velocity.X = 0;

                            if (Entity.Transform.Position.Y == previousPosition.Y)
                                Velocity.Y = 0;
                        }

                        if (!collider.IsTrigger && !otherCollider.IsTrigger)
                            CastRays(collider, otherCollider);
                    }
                }
            }

            CheckWallCollision();
        }

        private void CheckOneWayPlatform(Collider collider, Collider otherCollider)
        {
            // Check if its one way platform
            if (!collider.IsTrigger && otherCollider.GetType() == typeof(OneWayPlatform))
            {
                // If on bounds of object & not on floor
                if (previousPosition.X + collider.Origin.X + collider.Width > otherCollider.AbsolutePosition.X &&
                    previousPosition.X + collider.Origin.X < otherCollider.AbsolutePosition.X + otherCollider.Width)
                {
                    // Will only collide from top and if not holding down
                    if (previousPosition.Y + collider.Origin.Y + collider.Height < otherCollider.AbsolutePosition.Y)
                    {
                        OneWayPlatform oneway = (OneWayPlatform)otherCollider;
                        if (!Input.Keyboard.Hold(Keys.Down) || IgnoreInput || oneway.FallDown == false)
                        {
                            // Collide on Y
                            Entity.Transform.Position = new Vector2(Entity.Transform.Position.X, previousPosition.Y);

                            IsOnFloor = true;

                            // Trigger On Landed
                            if (IsFalling || IsJumping)
                            {
                                JumpCount = 0;

                                IsJumping = false;
                                IsFalling = false;
                                IsSliding = false;

                                OnLand?.Invoke();
                                //Debug.Log("[Platform]: On Land trigger.");
                            }
                        }
                    }
                }
            }
        }

        private void CheckSolid(Collider collider, Collider otherCollider)
        {
            // Prevent Movement if both colliders are solid
            if (!collider.IsTrigger && !otherCollider.IsTrigger)
            {
                // If on bounds of object & not on floor
                if (previousPosition.X + collider.Origin.X + collider.Width > otherCollider.AbsolutePosition.X &&
                    previousPosition.X + collider.Origin.X < otherCollider.AbsolutePosition.X + otherCollider.Width)
                {
                    // Check if collision was from Top
                    if (previousPosition.Y + collider.Origin.Y + collider.Height < otherCollider.AbsolutePosition.Y)
                    {
                        Entity.Transform.Position = new Vector2(Entity.Transform.Position.X, previousPosition.Y);

                        IsOnFloor = true;
                        if (IsFalling || IsJumping)
                        {
                            JumpCount = 0;

                            IsJumping = false;
                            IsFalling = false;
                            IsSliding = false;

                            OnLand?.Invoke();
                            //Debug.Log("[Platform]: On Land trigger.");
                        }
                    }
                    // Collide from Bottom
                    else if (previousPosition.Y + collider.Origin.Y > otherCollider.AbsolutePosition.Y + otherCollider.Height)
                    {
                        Entity.Transform.Position = new Vector2(Entity.Transform.Position.X, previousPosition.Y);
                        //Debug.Log("[Platform]: Hit Roof.");
                    }
                    // Is inside Collider
                    else
                    {
                        // Closer to Top
                        //if (Math.Abs(previousPosition.Y - otherCollider.AbsolutePosition.Y) < Math.Abs(previousPosition.Y - otherCollider.AbsolutePosition.Y + otherCollider.Height))
                        previousPosition.Y = otherCollider.AbsolutePosition.Y - collider.Height - collider.Origin.Y - 0.1f;
                        // CloserToBottom
                        //else
                        //    previousPosition.Y = otherCollider.AbsolutePosition.Y + otherCollider.Height + 0.1f;

                        Entity.Transform.Position = new Vector2(Entity.Transform.Position.X, previousPosition.Y);
                    }
                }

                // Check if collision was from Left
                if (previousPosition.X + collider.Origin.X > otherCollider.AbsolutePosition.X + otherCollider.Width)
                {
                    Entity.Transform.Position = new Vector2(previousPosition.X, Entity.Transform.Position.Y);
                    IsByWallLeft = true;
                    //Debug.Log("[Platform]: Hit Wall Left.");
                }
                // Check if collision was from Right
                else if (previousPosition.X + collider.Origin.X + collider.Width < otherCollider.AbsolutePosition.X)
                {
                    Entity.Transform.Position = new Vector2(previousPosition.X, Entity.Transform.Position.Y);
                    IsByWallRight = true;
                    //Debug.Log("[Platform]: Hit Wall Right.");
                }
            }
        }

        public Vector2 LeftRay, RightRay;
        private void CastRays(Collider collider, Collider otherCollider)
        {
            // Raycast left and right and see if theres still a collision there
            LeftRay = new Vector2(collider.AbsolutePosition.X - 2, Entity.Transform.Position.Y);
            if (otherCollider.IsOverlapping(LeftRay))
            {
                IsByWallLeft = true;
                hasWallLeft = true;
                //Debug.Log("Ray hit left");
            }

            RightRay = new Vector2(collider.AbsolutePosition.X + collider.Width + 2, Entity.Transform.Position.Y);
            if (otherCollider.IsOverlapping(RightRay))
            {
                IsByWallRight = true;
                hasWallRight = true;
                //Debug.Log("Ray hit right");
            }
        }

        private void CheckWallCollision()
        {
            // Clean Wall connection if no raycast found
            if (!hasWallLeft)
                IsByWallLeft = false;
            if (!hasWallRight)
                IsByWallRight = false;

            // Clean Wall connection if moving left or right
            if (Velocity.X > 0 && IsByWallLeft)
                IsByWallLeft = false;
            if (Velocity.X < 0 && IsByWallRight)
                IsByWallRight = false;
        }

        #endregion

        #region Applying extra Force

        public void ApplyForceX(float force) => ApplyForce(new Vector2(force, Force.Y));

        public void ApplyForceY(float force) => ApplyForce(new Vector2(Force.X, force));

        public void ApplyForce(Vector2 force)
        {
            Force = force;
            isApplyingForce = true;
        }

        private void UpdateForce()
        {
            if (isApplyingForce && _ForceTime < _ForceTotalTime)
                _ForceTime += Engine.Instance.DeltaTime * 1000;

            if (_ForceTime >= _ForceTotalTime)
            {
                _ForceTime = 0;
                isApplyingForce = false;
            }
        }

        #endregion

        #region Movement

        private void UpdateDefaultControls()
        {
            if (!DefaultControls || IgnoreInput)
                return;

            if (Input.Keyboard.IsMoving())
                MoveDirection = Input.Keyboard.Direction;

            if (Input.GamePads[0].IsMoving())
                MoveDirection = Input.GamePads[0].Direction;

            if (Input.Keyboard.Hold(Keys.Space) || Input.GamePads[0].Hold(Buttons.A))
                MoveDirection.Y = -1;

            //Debug.Log("[Platform]: Move Direction " + MoveDirection.X + " " + MoveDirection.Y + " (X,Y)");\
        }

        private void UpdateHorizontalMove()
        {
            if (MoveDirection.X != 0 && !IsMoving)
            {
                IsMoving = true;

                OnMove?.Invoke();
                //Debug.Log("[Platform]: On Move trigger.");
            }
            else if (MoveDirection.X == 0)
            {
                if (Velocity.X > 0)
                    Velocity.X -= Deceleration * Engine.Instance.DeltaTime;
                else if (Velocity.X < 0 || (Velocity.X == 0 && IsMoving))
                {
                    Velocity.X = 0;

                    IsMoving = false;

                    OnStop?.Invoke();
                    //Debug.Log("[Platform]: On Stop trigger.");
                }
            }
        }

        private void UpdateVerticalMove()
        {
            DoJump();

            DoFall();

            DoSlide();
        }

        private void DoJump()
        {
            // Jump check. Can only jump if player has released the buttons.
            if (MoveDirection.Y == 0)
                CanJump = true;

            if (MoveDirection.Y == -1 && CanJump &&
                (IsOnFloor || (CanDoubleJump && JumpCount < MAX_JUMP_COUNT) && !IsSliding))
            {
                CanJump = false;

                JumpCount++;

                IsJumping = true;
                IsOnFloor = false;

                OnJump?.Invoke();
                //Debug.Log("[Platform]: On Jump trigger.");

                Velocity.Y = JumpStrength;
            }
        }

        private void DoFall()
        {
            if (IsJumping)
            {
                //if(JumpSustain > 0)
                _JumpTime += Engine.Instance.DeltaTime * 1000;

                // If we are in the ascent of the jump
                // Fully override the vertical velocity with a power curve that gives players more control over the top of the jump
                if (_JumpTime > 0.0f && _JumpTime <= JumpSustain && MoveDirection.Y == -1)
                    Velocity.Y = JumpStrength * (_JumpTime / JumpSustain);
                else
                {
                    // Reached the apex of the jump
                    _JumpTime = 0.0f;

                    if (Velocity.Y > 0)
                    {
                        IsJumping = false;
                        IsFalling = true;

                        OnFall?.Invoke();
                        //Debug.Log("[Platform]: On Fall trigger.");
                    }
                }
            }

            // Notes: Raycast check if falling from ledges.
            // Fall Threshold = MaxFall / 4
            if (Velocity.Y > MaxFallSpeed / 10 && IsOnFloor && !IsJumping)
            {
                IsOnFloor = false;
                IsFalling = true;

                OnFall?.Invoke();
                //Debug.Log("[Platform]: On Fall trigger.");

                if (!CanJumpWhileFalling) JumpCount = MAX_JUMP_COUNT;
            }
        }

        private void DoSlide()
        {
            // Slide check
            if (!IsByWallLeft && !IsByWallRight && IsSliding)
                IsSliding = false;

            if (CanSlide && IsFalling && !IsSliding && !IsOnFloor && (IsByWallLeft || IsByWallRight))
            {
                IsSliding = true;

                // Give one extra jump
                if (JumpCount > 0)
                    JumpCount--;
            }

            if (MoveDirection.Y == -1 && CanJump && CanWallJump && IsSliding && ((IsByWallLeft && MoveDirection.X == 1) || (IsByWallRight && MoveDirection.X == -1)))
            {
                //IsOnFloor = false;
                CanJump = false;

                JumpCount++;

                IsJumping = true;
                IsOnFloor = false;
                IsSliding = false;

                OnJump?.Invoke();

                //Debug.Log("[Platform]: On Wall Jump trigger.");
                Velocity.Y = WallJumpStrength;
            }
        }

        // Notes: Need to implement this.
        /*public void SimulateControl(InputDirection direction)
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
        }*/

        #endregion

        #region Utils

        public override string ToString()
        {
            return string.Format("[Platform]: Velocity: {0} Move Direction: {1} MaxSpeed: {2} Acceleration {3} Deceleration {4} Jump Strength: {5} Gravity {6} Max Fall Speed: {7} Jump Sustain: {8} Jump Control Power: {9} Double Jump: {10} Max Jump Ammount: {11} Can Jump Falling from Ledge: {12}\n    Is Moving {13} IsJumping {14} IsFalling {15} IsOnFloor {16} Is By Wall LEFT {17} Is By Wall RIGHT {18}\n Default Controls: {19} Ignore Input: {20}",
            Velocity, MoveDirection, MaxSpeed, Acceleration, Deceleration, JumpStrength, Gravity, MaxFallSpeed, JumpSustain, JumpControlPower, CanDoubleJump, MAX_JUMP_COUNT, CanJumpWhileFalling, IsMoving, IsJumping, IsFalling, IsOnFloor, IsByWallLeft, IsByWallRight, DefaultControls, IgnoreInput);
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Caieta
{
    public class Platform : Component
    {
        /*
         *      Position
         */
        public Transform Transform;

        /*
         *      Movement
         */
        public bool IsMoving;
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
         *      Controls
         */
        public bool DefaultControls;

        /*
         *      Collisions
         */
        private Dictionary<string, BoxCollider> Colliders;
        //private readonly BoxCollider Hitbox = new BoxCollider(8, 11, -4, -11);
        //private readonly BoxCollider Duckbox = new BoxCollider(8, 6, -4, -6);

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

            Transform = new Transform();

            Colliders = new Dictionary<string, BoxCollider>();

            OnMove = null;
        }

        public void AddCollider(string boxName, BoxCollider box)
        {
            if (!Colliders.ContainsKey(boxName))
                Colliders.Add(boxName, box);
            else
                Debug.ErrorLog("[Platform]: Box Collider '"+ boxName + "' already exist.");
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update()
        {
            base.Update();

            if (DefaultControls)
            {
                Vector2 previousPosition = Transform.Position;

                if (Input.Keyboard.IsMoving())
                {
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

                Transform.Position += Velocity * Engine.Instance.DeltaTime;
                //Debug.Log(Transform);

                foreach (BoxCollider box in Colliders.Values)
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
                                    /*if (OnCollision != null)
                                    {
                                        OnColission(ent.Name);
                                        OnColission = null;
                                    }*/
                                }
                        }
                    }
                }

                // If the collision stopped us from moving, reset the velocity to zero.
                if (Transform.Position.X == previousPosition.X)
                    Velocity.X = 0;

                if (Transform.Position.Y == previousPosition.Y)
                    Velocity.Y = 0;
            }
        }

        public void Move(Vector2 MoveDirection)
        {
            if (Colliders.Count <= 0)
            {
                Transform.Position += MoveDirection * Engine.Instance.DeltaTime;
                return;
            }
        }

        public override void Render()
        {
            base.Render();

            foreach(BoxCollider box in Colliders.Values)
            {
                if (box.IsVisible)
                    box.Render(Transform);
            }
        }

        #region Utils

        public void ShowColliders()
        {
            foreach (BoxCollider box in Colliders.Values)
            {
                box.IsVisible = true;
            }
        }

        public void HideColliders()
        {
            foreach (BoxCollider box in Colliders.Values)
            {
                box.IsVisible = false;
            }
        }

        public override string ToString()
        {
            return string.Format("[Platform]: Colliders: {0} Transform:\n {1} ", Colliders.Count, Transform);
        }

        #endregion
    }
}

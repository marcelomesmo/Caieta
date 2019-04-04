using System;
using Microsoft.Xna.Framework;

namespace Caieta
{
    public class Bullet : Component
    {

        /*
         *      Movement
         */
        public bool IsMoving { get; private set; }
        private Vector2 MoveDirection;// { get; private set; }

        public Vector2 Velocity;

        public float MaxSpeed;
        public float Acceleration;

        public float MaxFallSpeed;
        public float Gravity;

        /*
         *      Direction
         */
        public float Angle {
            get { return _angle; }
            set {
                if (Entity != null && RotateEntityToAngle)
                    Entity.Transform.Rotation = value;

                _angle = value;

                // Notes: Calc only on Angle change.
                //MoveDirection.X = Math.Cos(Angle);
                //MoveDirection.Y = Math.Sin(Angle);
                MoveDirection = Calc.AngleToVector(MathHelper.ToRadians(_angle));

                //Debug.Log("[Bullet]: Angle: " + _angle + "º Move Dir: X " + MoveDirection.X + " Y " + MoveDirection.Y);
            }
        }
        private float _angle;

        /*
         *      Travel
         */
        public float TravelDistance { get; private set; }

        /*
         *      Properties
         */
        public bool Bounce;
        public bool RotateEntityToAngle;

        // public bool Step;
        // public Action OnStep;

        public Bullet()
        {
            IsMoving = true;

            Angle = 0;
            // Ja calcula quando faz Angle = 0. MoveDirection = new Vector2(1, 0);

            MaxSpeed = Acceleration = 200;//400;
            //Acceleration = 100;

            MaxFallSpeed = 200;
            Gravity = 0;

            TravelDistance = 0;

            Bounce = false;
            RotateEntityToAngle = false;
        }

        public override void Initialize()
        {
            base.Initialize();

            //Angle = Entity.Transform.Rotation;
        }

        public override void Update()
        {
            base.Update();

            Velocity.X += MoveDirection.X * Acceleration * Engine.Instance.DeltaTime;
            // Prevent the player from running faster than his top speed.  
            Velocity.X = MathHelper.Clamp(Velocity.X, -MaxSpeed, MaxSpeed);

            // Apply Gravity
            Velocity.Y += MoveDirection.Y * Acceleration * Engine.Instance.DeltaTime;
            Velocity.Y = MathHelper.Clamp(Velocity.Y, -MaxSpeed, MaxSpeed);
            // Prevent the player from falling faster than gravity;
            Velocity.Y = MathHelper.Clamp(Velocity.Y + Gravity * Engine.Instance.DeltaTime, -MaxFallSpeed, MaxFallSpeed);

            // Move
            //TravelDistance += Velocity.X; 
            // NOT RIGHT OK


            /* 
             *      MOVE ENTITY
             */
            // Notes: Could make this into a private later to optimize/improve performance.
            var Colliders = Entity.GetAll<Collider>();

            var PrevPosition = Entity.Transform.Position;

            // Move Entity
            Entity.Transform.Position += Velocity * Engine.Instance.DeltaTime;

            // Calculate Travel Distance
            TravelDistance += (float)Math.Sqrt(Math.Pow(Entity.Transform.Position.X - PrevPosition.X, 2) + Math.Pow(Entity.Transform.Position.Y - PrevPosition.Y, 2));
            //Debug.Log("Travel distance: " + TravelDistance);

            // If there are no colliders: move indistinguible (??)
            if (Colliders.Count <= 0)
                return;

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
                            /// TODO: Will need to check if this is first collision? Its colliding multiple times.
                            // if(PreviousPosition !c.IsOverlapping(ec))
                            // Trigger On Colision
                            c.OnCollision?.Invoke(ent);
                            Debug.Log("[Bullet]: '" + Entity.Name + "' On Collision trigger with '" + ent.Name + "'.");
                            // Notes: We can know exactly which collider hit the entity,
                            //          but this way we cant know which collider we collided with.

                            // Notes: Calc reflection here.
                            // Ref: https://gamedev.stackexchange.com/questions/23672/determine-resulting-angle-of-wall-collision
                            // Velocity = −(2 * Matrix.Dot(ec.NormalizedVector, Velocity) * ec.NormalizedVector − Velocity);
                            // Ref2: http://www.3dkingdoms.com/weekly/weekly.php?a=2
                            // Vnew = b * ( -2*(V dot N)*N + V ) where b is bounce loss.
                            // Simple reflection alternative:
                            // Hitting a solid and bounce is ON.
                            /*if (!ec.IsTrigger && Bounce)
                            {
                                // Hit vertical wall (top or bottom)
                                if (Entity.Transform.Y + c.Origin.Y + c.Height < ec.AbsolutePosition.Y
                                    || Entity.Transform.Y + c.Origin.Y > ec.AbsolutePosition.Y + ec.Height)
                                {
                                    //Velocity.X *= -1;
                                    // Calc new Angle
                                    Angle = 360 - Angle;

                                    Debug.Log("[Bullet]: Bounce off vertical collider.");
                                }
                                // Hit horizontal wall (left or right)
                                if (Entity.Transform.X + c.Origin.X > ec.AbsolutePosition.X + ec.Width
                                    || Entity.Transform.X + c.Origin.X + c.Width < ec.AbsolutePosition.X)
                                {
                                    //Velocity.Y *= -1;
                                    // Calc new Angle
                                    Angle = 180 - Angle;

                                    Debug.Log("[Bullet]: Bounce off horizontal collider.");
                                } 
                            }*/

                            break;
                            // Notes: Will trigger OnCollision multiple times if collided entity has 
                            //          more than one collider. With break will trigger for the first collider only.

                        }
                    }

                }
            }
                      
        }

        public override void Render()
        {
            base.Render();

            // Notes: Could implement a predictable route and speed vectors here in the future.
        }

        #region Utils

        public override string ToString()
        {
            return string.Format("[Bullet]: Angle {0} Velocity {1} MaxSpeed {2} Acceleration {3} Gravity {4}",
                                MathHelper.ToDegrees(Angle), Velocity, MaxSpeed, Acceleration, Gravity);
        }

        #endregion
    }
}

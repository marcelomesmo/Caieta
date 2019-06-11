using System;
using Microsoft.Xna.Framework;

namespace Caieta.Components.Attributes
{
    public class FollowCamera : Component
    {
        public enum CameraPolicy { STRAIGHT, LERP };
        public CameraPolicy Policy;

        public Vector2 StartMovePosition;
        public bool IsMoving;
        public Vector2 LerpMove;
        public Vector2 LerpStrength = Vector2.One;
        private Vector2 previousPos;

        public FollowCamera(CameraPolicy policy = CameraPolicy.STRAIGHT)
        {
            Policy = policy;
        }

        public override void Update()
        {
            base.Update();

            switch(Policy)
            {
                case CameraPolicy.STRAIGHT:

                    //Engine.SceneManager.Camera.Position = new Vector2(Entity.Transform.Position.X, Entity.Transform.Position.Y);
                    //Engine.SceneManager.Camera.CenterOrigin();
                    Engine.SceneManager.Camera.Position = new Vector2(Entity.Transform.Position.X, Entity.Transform.Position.Y);
                    //Engine.SceneManager.Camera.Position = new Vector2(Entity.Transform.Position.X - Graphics.Width / 2, Entity.Transform.Position.Y - Graphics.Height / 2);
                    Engine.SceneManager.Camera.RoundPosition();
                    //Engine.SceneManager.Camera.CenterOrigin();
                    Engine.SceneManager.Camera.ClampToLayout();

                    break;

                //case CameraPolicy.LERP:

                    //Vector2 position = Engine.SceneManager.Camera.Position;
                    //Vector2 target = new Vector2(Entity.Transform.Position.X - Graphics.Width / 2, Entity.Transform.Position.Y - Graphics.Height / 2);

                    //if (position == target)
                    //{
                    //    IsMoving = false;
                    //    break;
                    //}

                    //if (position != target && !IsMoving)
                    //{
                    //    StartMovePosition = position;
                    //    IsMoving = true;
                    //}

                    //Vector2 lerpStrength = Vector2.One;
                    //if(position.X != target.X)
                    //    lerpStrength.X = 1- Math.Abs((target.X - position.X) / (target.X - StartMovePosition.X)) + LerpStrength.X/20;//(position.X - target.X) * Engine.SceneManager.Camera.LerpStrength.X;

                    //if (position.Y != target.Y)
                    //    lerpStrength.Y = 1 - Math.Abs((target.Y - position.Y) / (target.Y - StartMovePosition.Y)) + LerpStrength.Y / 20;//(position.Y - target.Y) * Engine.SceneManager.Camera.LerpStrength.Y;
                 
                    //lerpStrength.X = MathHelper.Clamp(lerpStrength.X, 0, 1);
                    //lerpStrength.Y = MathHelper.Clamp(lerpStrength.Y, 0, 1);

                    //Vector2 Target;
                    //Target.X = MathHelper.Lerp(StartMovePosition.X, target.X, lerpStrength.X);
                    //Target.Y = MathHelper.Lerp(StartMovePosition.Y, target.Y, lerpStrength.Y);

                    //Engine.SceneManager.Camera.Position = new Vector2(Target.X, Target.Y);
                    //Engine.SceneManager.Camera.RoundPosition();
                    //Engine.SceneManager.Camera.ClampToLayout();

                    ////Debug.Log("Start moving from: " + StartMovePosition + " Entity at: " + Entity.Transform.Position + " Camera at: " + Engine.SceneManager.Camera.Position +
                    ////" Target: " + target + " Strenght: " + lerpStrength);

                    //break;

                case CameraPolicy.LERP:
                    Vector2 cameraPos = Engine.SceneManager.Camera.Position;
                    Vector2 targetPos = new Vector2((float)Math.Round(Entity.Transform.Position.X), (float)Math.Round(Entity.Transform.Position.Y));
                    //Vector2 targetPos = new Vector2((float)Math.Round(Entity.Transform.Position.X) - Graphics.Width / 2, (float)Math.Round(Entity.Transform.Position.Y) - Graphics.Height / 2);

                    if (cameraPos == targetPos || (previousPos == targetPos && LerpMove == Vector2.One))
                    {
                        IsMoving = false;
                        //Debug.Log("Stopped");
                        //Engine.SceneManager.Camera.ClampToLayout();
                        break;
                    }

                    if (cameraPos != targetPos && !IsMoving)
                    {
                        StartMovePosition = cameraPos;
                        IsMoving = true;
                        LerpMove = Vector2.Zero;
                    }

                    if (cameraPos.X != targetPos.X)
                        LerpMove.X += LerpStrength.X * Engine.Instance.DeltaTime;

                    if (cameraPos.Y != targetPos.Y)
                        LerpMove.Y += LerpStrength.Y * Engine.Instance.DeltaTime;

                    LerpMove.X = MathHelper.Clamp(LerpMove.X, 0, 1);
                    LerpMove.Y = MathHelper.Clamp(LerpMove.Y, 0, 1);

                    Vector2 Movement;
                    Movement.X = MathHelper.Lerp(StartMovePosition.X, targetPos.X, LerpMove.X);
                    Movement.Y = MathHelper.Lerp(StartMovePosition.Y, targetPos.Y, LerpMove.Y);

                    Engine.SceneManager.Camera.Position = new Vector2(Movement.X, Movement.Y);
                    Engine.SceneManager.Camera.RoundPosition();
                    Engine.SceneManager.Camera.ClampToLayout();

                    //Debug.Log("Start moving from: " + StartMovePosition + " Entity at: " + Calc.Round(Entity.Transform.Position) + " Camera at: " + Engine.SceneManager.Camera.Position +
                    //" Target: " + targetPos + " Strenght: " + LerpMove);

                    previousPos = targetPos;

                    break;

                default:
                    Debug.ErrorLog("[FollowCamera]: Invalid Camera Policy '" + Policy + "'.");
                    break;
            }
        }
    
        public void LookAt()
        {
            Engine.SceneManager.Camera.Position = new Vector2(Entity.Transform.Position.X, Entity.Transform.Position.Y);
            //Engine.SceneManager.Camera.Position = new Vector2(Entity.Transform.Position.X - Graphics.Width / 2, Entity.Transform.Position.Y - Graphics.Height / 2);
            Engine.SceneManager.Camera.RoundPosition();
            Engine.SceneManager.Camera.ClampToLayout();
        }

        public FollowCamera SetLerp(float strength)
        {
            return SetLerp(new Vector2(strength, strength));
        }

        public FollowCamera SetLerp(Vector2 strength)
        {
            if (Policy != CameraPolicy.LERP)
            {
                Debug.ErrorLog("[FollowCamera]: Setting Lerp Strength to " + strength + " but camera not set to Lerp. Setting camera to Lerp mode.");
                Policy = CameraPolicy.LERP;
            }

            LerpStrength = strength;

            return this;
        }
    }
}


// Zoom

//var newZoom = 1 + ZoomTimer.Progress / 10;

//Engine.SceneManager.Camera.Zoom = newZoom;
//Engine.SceneManager.Camera.Position = new Vector2(Transform.Position.X, Transform.Position.Y);
//Engine.SceneManager.Camera.CenterOrigin();

//var msx = Transform.Position.X - Graphics.Width / 2;
//var msy = Transform.Position.Y - Graphics.Height / 2;

//var width = Engine.SceneManager.Camera.Matrix.Translation.X - msx;
//var height = Engine.SceneManager.Camera.Matrix.Translation.Y + msy;

//var zoomOffset = 1 - newZoom / Engine.SceneManager.Camera.Zoom;

//Engine.SceneManager.Camera.Position -= new Vector2(width * zoomOffset, height * zoomOffset);

//Engine.SceneManager.Camera.Zoom = newZoom;


//Debug.Log("W: " + width + " H: " + height + " zoom: " + newZoom + " zoomoff: " + zoomOffset + Engine.SceneManager.Camera);

using System;
using Microsoft.Xna.Framework;

namespace Caieta.Components.Attributes
{
    public class FollowCamera : Component
    {
        public enum CameraPolicy { STRAIGHT, LERP };
        public CameraPolicy Policy;

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
                    Engine.SceneManager.Camera.Position = new Vector2(Entity.Transform.Position.X - Graphics.Width / 2, Entity.Transform.Position.Y - Graphics.Height / 2);
                    Engine.SceneManager.Camera.RoundPosition();
                    Engine.SceneManager.Camera.ClampToLayout();

                    break;

                case CameraPolicy.LERP:

                    Vector2 position = Engine.SceneManager.Camera.Position;
                    Vector2 target = new Vector2(Entity.Transform.Position.X - Graphics.Width / 2, Entity.Transform.Position.Y - Graphics.Height / 2);

                    Vector2 lerpStrength;
                    lerpStrength.X = Engine.SceneManager.Camera.LerpStrength.X/5;//(position.X - target.X) * Engine.SceneManager.Camera.LerpStrength.X;
                    lerpStrength.Y = Engine.SceneManager.Camera.LerpStrength.Y/5;//(position.Y - target.Y) * Engine.SceneManager.Camera.LerpStrength.Y;

                    Vector2 Target;
                    Target.X = MathHelper.Lerp(position.X, target.X, lerpStrength.X);
                    Target.Y = MathHelper.Lerp(position.Y, target.Y, lerpStrength.Y);

                    Engine.SceneManager.Camera.Position = new Vector2(Target.X, Target.Y);
                    Engine.SceneManager.Camera.RoundPosition();
                    Engine.SceneManager.Camera.ClampToLayout();

                    //Debug.Log("Player at: " + Entity.Transform.Position + " Camera at: " + Engine.SceneManager.Camera.Position +
                    //" Target: " + Target);

                    break;

                default:
                    Debug.ErrorLog("[FollowCamera]: Invalid Camera Policy '" + Policy + "'.");
                    break;
            }
        }
    }
}

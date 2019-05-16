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

                    Engine.SceneManager.Camera.Position = new Vector2(Entity.Transform.Position.X - Graphics.Width / 2, Entity.Transform.Position.Y - Graphics.Height / 2);
                    Engine.SceneManager.Camera.RoundPosition();
                    Engine.SceneManager.Camera.ClampToLayout();

                    break;

                case CameraPolicy.LERP:

                    Vector2 Target;
                    Target.X = MathHelper.Lerp(Engine.SceneManager.Camera.Position.X, Entity.Transform.Position.X - Graphics.Width / 2, Engine.SceneManager.Camera.LerpStrength.X / 10);
                    Target.Y = MathHelper.Lerp(Engine.SceneManager.Camera.Position.Y, Entity.Transform.Position.Y - Graphics.Height / 2, Engine.SceneManager.Camera.LerpStrength.Y / 10);

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

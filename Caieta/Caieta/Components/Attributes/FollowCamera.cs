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

                    // TODO: Fix this math.
                    Vector2 Target;
                    Target.X = Entity.Transform.Position.X - Graphics.Width / 2;
                    //Target.X = MathHelper.Lerp(Target.X, Engine.SceneManager.Camera.Origin.X, Engine.SceneManager.Camera.LerpStrength.X);
                    Target.X = MathHelper.Lerp(Engine.SceneManager.Camera.Position.X, Target.X, Engine.SceneManager.Camera.LerpStrength.X);

                    Target.Y = Entity.Transform.Position.Y - Graphics.Height / 2;
                    //Target.Y = MathHelper.Lerp(Target.Y, Engine.SceneManager.Camera.Origin.Y, Engine.SceneManager.Camera.LerpStrength.Y);
                    Target.Y = MathHelper.Lerp(Engine.SceneManager.Camera.Position.Y, Target.Y, Engine.SceneManager.Camera.LerpStrength.Y);

                    Engine.SceneManager.Camera.Position = new Vector2(Target.X, Target.Y);
                    //Engine.SceneManager.Camera.RoundPosition();
                    Engine.SceneManager.Camera.ClampToLayout();

                    break;

                default:
                    Debug.ErrorLog("[FollowCamera]: Invalid Camera Policy '" + Policy + "'.");
                    break;
            }
        }
    }
}

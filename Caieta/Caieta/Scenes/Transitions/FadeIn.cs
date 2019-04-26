using Caieta.Components.Utils;
using Microsoft.Xna.Framework;

namespace Caieta.Scenes.Transitions
{
    public class FadeIn : Transition
    {
        Color FadeColor;

        public FadeIn(float time, Color fadeColor, string entityname = "FadeIn", bool initial_visibility = true) : base(time, entityname, initial_visibility)
        {
            FadeColor = fadeColor;
        }
        public FadeIn(float time, string entityname = "FadeIn", bool initial_visibility = true) : this(time, Color.Black, entityname, initial_visibility){ }

        public override void Update()
        {
            base.Update();

            if (Progress < 100)
            {
                Progress = Timer.TargetTime == 0f ? 100 : (int)((Timer.ElapsedTime / Timer.TargetTime) * 100f);

                /*if (Timer.IsRunning)
                    Debug.Log("[FadeIn]: Progress: expected " + (Timer.ElapsedTime / Timer.TargetTime) * 100f + " real " + Progress + "%");
                    */                   
            }
        }

        public override void Render()
        {
            base.Render();

            if(Progress < 100)
            {
                Graphics.DrawRect(new Rectangle((int)Engine.SceneManager.Camera.X, (int)Engine.SceneManager.Camera.Y, Graphics.ViewWidth, Graphics.ViewHeight), FadeColor, (100 - Progress), FillType.FILL);
                //Graphics.DrawRect(new Rectangle(0, 0, Graphics.ViewWidth, Graphics.ViewHeight), FadeColor, (100 - Progress), FillType.FILL);
            }
        }
    }
}

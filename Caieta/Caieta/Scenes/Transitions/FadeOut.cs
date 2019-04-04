using System;
using Microsoft.Xna.Framework;

namespace Caieta.Scenes.Transitions
{
    public class FadeOut : Transition
    {
        public Color FadeColor;

        public FadeOut(float time, Color fadeColor, string entityname = "FadeOut", bool initial_visibility = true) : base(time, entityname, initial_visibility)
        {
            FadeColor = fadeColor;
        }
        public FadeOut(float time, string entityname = "FadeOut", bool initial_visibility = true) : this(time, Color.Black, entityname, initial_visibility) { }

        public override void Update()
        {
            base.Update();

            if (Progress < 100)
            {
                Progress = (int)((Timer.ElapsedTime / Timer.TargetTime) * 100f);

                if(Timer.IsRunning)
                    Debug.Log("[FadeOut]: Progress: expected " + (Timer.ElapsedTime / Timer.TargetTime) * 100f + " real " + Progress + "%");
            }
        }

        public override void Render()
        {
            base.Render();

            if (Progress < 100)
                Graphics.DrawRect(new Rectangle((int)Engine.SceneManager.Camera.X, (int)Engine.SceneManager.Camera.Y, Graphics.ViewWidth, Graphics.ViewHeight), FadeColor, Progress, FillType.FILL);
        }
    }
}

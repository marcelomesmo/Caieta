using System;
using Caieta.Components.Utils;
using Microsoft.Xna.Framework;

namespace Caieta.Scenes.Transitions
{
    public class Fade : Transition
    {
        private Color _FadeInColor;
        private Color _FadeOutColor;
        private float _WaitTime;
        private float _OutTime;

        Action OnFadeFinish;

        // Default is InTimer;
        Timer WaitTimer;
        Timer OutTimer;
        private int _OutProgress;

        public Fade(float in_time, float wait_time, float out_time, Color fadeInColor, Color fadeOutColor, Action OnFinish = null, string entityname = "Fade", bool initial_visibility = true) : base (in_time, entityname, initial_visibility)
        {
            _FadeInColor = fadeInColor;
            _FadeOutColor = fadeOutColor;
            _WaitTime = wait_time;
            _OutTime = out_time;

            _OutProgress = 0;

            OnFadeFinish = OnFinish;
        }
        public Fade(float in_time, float wait_time, float out_time, Action OnFinish = null, string entityname = "Fade", bool initial_visibility = true) : this(in_time, wait_time, out_time, Color.Black, Color.Black, OnFinish, entityname, initial_visibility){ }

        public override void Create()
        {
            base.Create();

            WaitTimer = new Timer(_WaitTime);
            Add(WaitTimer);

            OutTimer = new Timer(_OutTime);
            Add(OutTimer);

            // Fade In Finish
            OnFinish = WaitTimer.Start;
            WaitTimer.OnTime = OutTimer.Start;
            OutTimer.OnTime = OnFadeFinish;
        }

        public override void Update()
        {
            base.Update();

            if (Progress < 100)
                Progress = Timer.TargetTime == 0f ? 100 : (int)((Timer.ElapsedTime / Timer.TargetTime) * 100f);

            if(OutTimer.IsRunning)
                _OutProgress = OutTimer.TargetTime == 0f ? 100 : (int)((OutTimer.ElapsedTime / OutTimer.TargetTime) * 100f);
        }

        public override void Render()
        {
            base.Render();

            if(Progress < 100)
                Graphics.DrawRect(new Rectangle((int)Engine.SceneManager.Camera.X, (int)Engine.SceneManager.Camera.Y, Graphics.ViewWidth, Graphics.ViewHeight), _FadeInColor, (100 - Progress), FillType.FILL);

            if (OutTimer.IsRunning && _OutProgress < 100)
                Graphics.DrawRect(new Rectangle((int)Engine.SceneManager.Camera.X, (int)Engine.SceneManager.Camera.Y, Graphics.ViewWidth, Graphics.ViewHeight), _FadeOutColor, _OutProgress, FillType.FILL);

        }
    }
}

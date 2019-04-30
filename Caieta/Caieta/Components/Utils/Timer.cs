using System;

namespace Caieta.Components.Utils
{
    public class Timer : Component
    {
        public Action OnTime;

        public bool IsRunning { get; protected set; }
        public bool IsPaused { get; protected set; }
        public bool IsRepeating;
        public bool IgnoreTimeRate;

        public int TimesPlayed;

        public float TargetTime { get; protected set; }
        public float ElapsedTime { get; protected set; }

        public Timer(float time, bool repeat = false)
        {
            OnTime = null;

            TargetTime = time >= 0f ? time : 0f;

            IsRepeating = repeat;
        }

        public override void Update()
        {
            base.Update();

            // Run timer if not paused
            if(IsRunning && !IsPaused)
            {
                if (IgnoreTimeRate)
                    ElapsedTime += Engine.Instance.RawDeltaTime * 1000;
                else
                    ElapsedTime += Engine.Instance.DeltaTime * 1000;
            }

            // Finish timer on target time reached
            if(IsRunning && ElapsedTime >= TargetTime)
            {
                ElapsedTime = TargetTime;   // Cap timer

                TimesPlayed++;

                if (IsRepeating)
                    Start();
                else
                    Stop();

                OnTime?.Invoke();
                //Debug.Log("[Timer]: On Time finished trigger.");
            }

        }

        public void AdjustTimer(float time)
        {
            Stop();
            TargetTime = time >= 0f ? time : 0f;
            Start();
        }

        public void Start()
        {
            IsRunning = true;
            ElapsedTime = 0;
            Resume();
        }

        public void Stop()
        {
            IsRunning = false;
            TimesPlayed = 0;
        }

        public void Pause()
        {
            IsPaused = true;
        }

        public void Resume()
        {
            IsPaused = false;
        }

        public void Reset()
        {
            Start();
        }
    }
}

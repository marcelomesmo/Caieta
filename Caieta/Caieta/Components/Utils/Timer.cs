using System;

namespace Caieta.Components.Utils
{
    public class Timer : Component
    {
        public Action OnTime;

        public bool IsRunning { get; private set; }
        public bool IsPaused { get; private set; }
        public bool IsRepeating;
        public bool IgnoreTimeRate;

        public float TargetTime { get; private set; }
        public float ElapsedTime { get; private set; }

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

                //Debug.Log("[Timer]: On Time finished trigger.");

                OnTime?.Invoke();

                if (IsRepeating)
                    Start();
                else
                    Stop();
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
            ElapsedTime = 0f;
            Resume();
        }

        public void Stop()
        {
            IsRunning = false;
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

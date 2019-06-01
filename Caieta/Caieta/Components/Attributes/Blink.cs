using System;
using Caieta.Components.Utils;

namespace Caieta.Components.Attributes
{
    public class Blink : Timer
    {
        public Action OnFinish;

        public float TimeON { get; protected set; }
        public float TimeOFF { get; protected set; }
        public float TotalTime { get; protected set; }
        private float elapsedBlinkTime;

        public bool IsON = true;

        public Blink(float onTime, float offTime, float blinkTime, bool repeat = false) : base(onTime, repeat)
        {
            TimeON = onTime >= 0f ? onTime : 0f;
            TimeOFF = offTime >= 0f ? offTime : 0f;

            TotalTime = blinkTime < TimeON + TimeOFF ? TimeON + TimeOFF : blinkTime;
        }

        public override void Initialize()
        {
            base.Initialize();

            OnTime = () =>
            {
                if(elapsedBlinkTime >= TotalTime)
                {
                    elapsedBlinkTime = TotalTime;

                    Stop();
                    Entity.IsVisible = true;

                    OnFinish?.Invoke();
                    //Debug.Log("[Blink]: On Blink finished trigger.");
                }

                if (IsON)
                    SetBlinkOff();
                else
                    SetBlinkOn();
            };
        }

        public new void Start()
        {
            elapsedBlinkTime = 0;
            base.Start();
        }

        private void SetBlinkOn()
        {
            Entity.IsVisible = true;
            IsON = true;
            AdjustTimer(TimeON);
        }

        private void SetBlinkOff()
        {
            Entity.IsVisible = false;
            IsON = false;
            AdjustTimer(TimeOFF);
        }

        public new void Stop()
        {
            elapsedBlinkTime = 0;
            Entity.IsVisible = true;
            base.Stop();
        }

        public override void Update()
        {
            base.Update();

            if(IsRunning)
                elapsedBlinkTime += ElapsedTime;
        }

        #region Utils

        public override string ToString()
        {
            return string.Format("[Blink]: Is On: {0} Entity Visible: {1} On Time: {2} Off Time: {3} Total Time: {4}/{5}", IsON,
             Entity.IsVisible, TimeON, TimeOFF, elapsedBlinkTime, TotalTime);
        }

        #endregion


    }
}

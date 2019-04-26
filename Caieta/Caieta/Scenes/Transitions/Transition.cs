using System;
using Caieta.Components.Utils;
using Caieta.Entities;

namespace Caieta.Scenes.Transitions
{
    public abstract class Transition : Entity
    {
        public Action OnFinish;

        public Timer Timer;
        public int Progress;

        protected Transition(float time, string entityname = "Transition", bool initial_visibility = true) : base(entityname, initial_visibility)
        {
            Timer = new Timer(time);
            Timer.IgnoreTimeRate = true;

            Timer.OnTime = () =>
            {
                OnFinish?.Invoke();

                Debug.Log("[Transition]: Finished Transition '" + Name + "'.");
            };

            Add(Timer);
        }

        public void Start()
        {
            Timer.Start();

            Progress = 0;

            Debug.Log("[Transition]: Started Transition '" + Name + "'. Time to finish: " + Timer.TargetTime + "ms");
        }
    }
}

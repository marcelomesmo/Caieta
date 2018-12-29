using System;
using Caieta.Entities;

namespace Caieta
{
    public abstract class Component
    {
        public Entity Entity;

        public bool IsActive;
        /*{
            get { return IsActive; }
            set
            {
                if (IsActive != value)
                {
                    IsActive = value;

                    if (IsActive)
                        OnEnable();
                    else
                        OnDisabled();
                }
            }

        }
        private bool _active;*/

        public virtual void Initialize()
        {

        }

        public virtual void Update()
        {

        }

        public virtual void Render()
        {

        }

        public virtual void Unload()
        {

        }

        /*
        public virtual void OnEnable()
        {

        }

        public virtual void OnDisabled()
        {

        }*/

        public override string ToString()
        {
            return string.Format("[Component]: Type: {0} Active: {1}", this.GetType(), IsActive);
        }
    }
}

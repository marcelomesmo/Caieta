using System;
using Microsoft.Xna.Framework.Input;

namespace Caieta
{
    public class Touch
    {
        public bool IsActive { get; set; }

        private MouseState PreviousState;
        private MouseState CurrentState;
        //private TouchInput touch;

        internal Touch()
        {
            IsActive = true;

            // Notes: Is this necessary?
            PreviousState = new MouseState();
            CurrentState = new MouseState();

            Debug.Log("[Touch]: Touch initialized.");
        }

        internal void Update()
        {
            // Update Touch & Mouse inputs
            PreviousState = CurrentState;
            if (IsActive)
                CurrentState = Microsoft.Xna.Framework.Input.Mouse.GetState();
            else
                CurrentState = new MouseState();
        }

    }
}

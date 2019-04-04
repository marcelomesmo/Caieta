using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Caieta
{
    public class DebugConsole
    {
        public bool IsEnabled = true;
        public bool IsOpen;

        private Keyboard Keyboard;

        public DebugConsole()
        {
            IsOpen = false;
        }

        internal void Initialize()
        {
            Keyboard = new Keyboard();

        }

        internal void Update()
        {
            // Update Inputs
            Keyboard.Update();

            if (IsOpen)
                UpdateOpen();
            else if (IsEnabled)
                UpdateClosed();
        }

        private void UpdateOpen()
        {
            if (Input.Keyboard.Pressed(Keys.OemTilde))
            {
                IsOpen = false;

                Input.Enable();
            }
        }

        private void UpdateClosed()
        {
            if (Input.Keyboard.Pressed(Keys.OemTilde))
            {
                IsOpen = true;

                Input.Disable();
            }
        }

        internal void Render()
        {
            int screenWidth = Graphics.ViewWidth;
            int screenHeight = Graphics.ViewHeight;

            Graphics.DrawRect(10, 40, screenWidth - 20, screenHeight - 50, Color.Black, 80, FillType.FILL);

            Graphics.DrawText(">", new Vector2(20, screenHeight - 42), Color.White);
        }
    }
}

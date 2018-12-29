using System;
using Microsoft.Xna.Framework;

namespace Caieta
{
    public class DebugConsole
    {
        public bool IsEnabled = true;
        public bool IsOpen;

        public DebugConsole()
        {
            IsOpen = false;
        }

        internal void Initialize()
        {

        }

        internal void Update()
        {
            if (IsOpen)
                UpdateOpen();
            else if (IsEnabled)
                UpdateClosed();
        }

        private void UpdateOpen()
        {

        }

        private void UpdateClosed()
        {

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

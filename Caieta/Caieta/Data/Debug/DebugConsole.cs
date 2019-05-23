using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Caieta
{
    public class DebugConsole
    {
        public bool IsEnabled = true;
        public bool IsOpen;

        private Keyboard Keyboard;

        private SpriteFont ConsoleFont;

        public DebugConsole()
        {
            IsOpen = false;
        }

        internal void Initialize()
        {
            Keyboard = new Keyboard();

            //ConsoleFont = Resources.Get<SpriteFont>("Fonts/PressStart2P");
            ConsoleFont = Resources.Get<SpriteFont>("Fonts/MonogramExtended");

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
            if (Keyboard.Pressed(Keys.OemTilde))
            {
                IsOpen = false;

                Input.Enable();
            }
        }

        private void UpdateClosed()
        {
            if (Keyboard.Pressed(Keys.OemTilde))
            {
                IsOpen = true;

                Input.Disable();
            }
        }

        int screenWidth, screenHeight;
        int consoleWidth, consoleHeight;
        int startConsoleX, startConsoleY;

        internal void Render()
        {
            screenWidth = Graphics.ViewWidth;
            screenHeight = Graphics.ViewHeight;
            consoleWidth = screenWidth - 20;
            consoleHeight = screenHeight / 3;
            startConsoleX = 10;
            startConsoleY = 2 * screenHeight / 3 - 20;

            // Start a new batch to draw relative to screensize
            Graphics.SpriteBatch.Begin();
            //Graphics.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, null);

            Graphics.DrawRect(0, 0, screenWidth, screenHeight, Color.Black, 50, FillType.FILL);

            Graphics.DrawRect(startConsoleX, startConsoleY, consoleWidth, consoleHeight, Color.Black, 80, FillType.FILL);

            Graphics.DrawText(ConsoleFont, "> ", new Vector2(20, screenHeight - 40), Color.White);

            Graphics.DrawText(ConsoleFont, "Type [help] for command list. ", new Vector2(startConsoleX + 10, startConsoleY + 10), Color.White);//, Vector2.Zero, new Vector2(1.5f, 1.5f), 0);

            Graphics.SpriteBatch.End();
        }
    }
}

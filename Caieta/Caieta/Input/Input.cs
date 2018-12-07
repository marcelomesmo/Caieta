using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Caieta
{

    public enum InputDirection
    {
        NONE,
        LEFT,
        RIGHT,
        UP,
        DOWN
    }

    public static class Input
    {
        public static Keyboard Keyboard { get; private set; }
        public static Touch Touch { get; private set; }
        public static GamePad[] GamePads { get; private set; }

        public const int MAX_SUPPORTED_GAMEPADS = 4;

        internal static void Initialize()
        {
            //Init devices
            Keyboard = new Keyboard();
            Touch = new Touch();
            GamePads = new GamePad[MAX_SUPPORTED_GAMEPADS];
            for (int i = 0; i < MAX_SUPPORTED_GAMEPADS; i++)
                GamePads[i] = new GamePad((PlayerIndex)i);

            //Debug.Log("[Input]: Joystick connected : " + Joystick.GetCapabilities(0).Identifier + " gp " + Joystick.GetCapabilities(0).IsGamepad);

            Debug.Log("[Input]: Input complete initialized.");
            Debug.LogLine();

        }

        internal static void Update()
        {
            Keyboard.Update();
            Touch.Update();

            for (int i = 0; i < MAX_SUPPORTED_GAMEPADS; i++)
                GamePads[i].Update();
        }


        #region Utils

        public static void Disable()
        {
            Keyboard.IsActive = false;
            Touch.IsActive = false;

            for (int i = 0; i < MAX_SUPPORTED_GAMEPADS; i++)
            {
                GamePads[i].IsActive = false;
                GamePads[i].StopRumble();
            }

            Debug.Log("[Input]: Input Disabled.");
        }

        public static void Enable()
        {
            Keyboard.IsActive = true;
            Touch.IsActive = true;

            for (int i = 0; i < MAX_SUPPORTED_GAMEPADS; i++)
                GamePads[i].IsActive = true;

            Debug.Log("[Input]: Input Enabled.");
        }

        public static int GamepadsConnected()
        {
            int connected_gamepads = 0;

            for (int i = 0; i < MAX_SUPPORTED_GAMEPADS; i++)
            {
                if (GamePads[i].IsAttached)
                    connected_gamepads++;
            }

            return connected_gamepads;
        }
        #endregion

    }
}

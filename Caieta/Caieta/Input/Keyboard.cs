using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Caieta
{
    public class Keyboard
    {
        public bool IsActive { get; set; }

        private KeyboardState PreviousState;
        private KeyboardState CurrentState;

        // Notes: Reset on Update.
        public Vector2 Direction { get; private set; }
        private const int _directionIncrement = 1;

        internal Keyboard()
        {
            IsActive = true;

            PreviousState = new KeyboardState();
            CurrentState = new KeyboardState();

            Debug.Log("[Keyboard]: Keyboard initialized.");
        }

        internal void Update()
        {
            Direction = new Vector2(0, 0);

            PreviousState = CurrentState;
            if (IsActive)
                CurrentState = Microsoft.Xna.Framework.Input.Keyboard.GetState();
            else
                CurrentState = new KeyboardState();
        }

        #region Utils

        public string GetHoldKey()
        {
            if (CurrentState.GetPressedKeys().Length > 0)
            {
                string result = "";
                Keys[] keys = CurrentState.GetPressedKeys();

                for (int i = 0; i < CurrentState.GetPressedKeys().Length; i++)
                    result += keys[i].ToString() + " ";

                return result;
            }
            return "-";
        }

        public string GetPressedKey()
        {
            if (CurrentState.GetPressedKeys().Length > 0)
            {
                string result = "";
                Keys[] curr = CurrentState.GetPressedKeys();

                foreach (var key in curr)
                    if (!PreviousState.IsKeyDown(key))       // Pressed(key)
                        result += key.ToString() + " ";

                if (result.Equals(""))
                    return "-";

                return result;
            }
            return "-";
        }

        public string GetReleasedKey()
        {
            if (PreviousState.GetPressedKeys().Length > 0)
            {
                string result = "";
                Keys[] curr = PreviousState.GetPressedKeys();

                foreach (var key in curr)
                    if (!CurrentState.IsKeyDown(key))       // Released(key)
                        result += key.ToString() + " ";

                if (result.Equals(""))
                    return "-";

                return result;
            }
            return "-";
        }

        public Keys GetLastReleasedKey()
        {
            if (PreviousState.GetPressedKeys().Length > 0)
            {
                foreach (var key in PreviousState.GetPressedKeys())
                    if (!CurrentState.IsKeyDown(key))       // Released(key)
                        return key;
            }
            return Keys.None;
        }

        public string GetModifierKey()
        {
            if (CurrentState.GetPressedKeys().Length > 0)
            {
                string result = "";

                if (!IsShiftDown() && !IsAltDown() && !IsControlDown() && !IsCommandDown())
                    return "-";

                if (IsShiftDown())
                    result += "SHIFT" + " ";

                if (IsAltDown())
                    result += "ALT" + " ";

                if (IsControlDown())
                    result += "CTRL" + " ";

                if (IsCommandDown())
                    result += "CMD" + " ";

                return result;
            }
            return "-";
        }

        #endregion

        #region Modifiers

        public bool IsShiftDown()
        {
            return CurrentState.IsKeyDown(Keys.LeftShift) || CurrentState.IsKeyDown(Keys.RightShift);
        }

        public bool IsAltDown()
        {
            return CurrentState.IsKeyDown(Keys.LeftAlt) || CurrentState.IsKeyDown(Keys.RightAlt);
        }

        public bool IsControlDown()
        {
#if OSX
            return CurrentState.IsKeyDown(Keys.LeftWindows) || CurrentState.IsKeyDown(Keys.RightWindows);
#endif
            return CurrentState.IsKeyDown(Keys.LeftControl) || CurrentState.IsKeyDown(Keys.RightControl);
        }

        public bool IsCommandDown()
        {
            return CurrentState.IsKeyDown(Keys.LeftWindows) || CurrentState.IsKeyDown(Keys.RightWindows);
        }

        #endregion

        #region Key Checks

        public bool Hold(Keys key)
        {
            return CurrentState.IsKeyDown(key);
        }

        public bool Pressed(Keys key)
        {
            return CurrentState.IsKeyDown(key) && !PreviousState.IsKeyDown(key);
        }

        public bool Released(Keys key)
        {
            return !CurrentState.IsKeyDown(key) && PreviousState.IsKeyDown(key);
        }

        #endregion

        #region Multiple Key Checks

        // Convenience methods
        // Notes: This is O(n)
        // Notes: Allows for Hold() -> always true
        public bool Hold(params Keys[] keys)
        {
            bool hold = true;

            foreach (var _key in keys)
            {
                hold = hold && Hold(_key);
                if (!hold) return false;
            }

            return true;
        }

        // Notes: This is O(1)
        public bool Hold(Keys keyA, Keys keyB)
        {
            return Hold(keyA) || Hold(keyB);
        }

        public bool Pressed(Keys keyA, Keys keyB)
        {
            return Pressed(keyA) || Pressed(keyB);
        }

        public bool Released(Keys keyA, Keys keyB)
        {
            return Released(keyA) || Released(keyB);
        }

        #endregion

        #region Direction

        public string GetDirection()
        {
            string result = "";

            if (CurrentState.IsKeyDown(Keys.Up))
            {
                if (CurrentState.IsKeyDown(Keys.Down))
                    result += "STALL ";
                else
                    result += "UP ";
            }
            else if (CurrentState.IsKeyDown(Keys.Down))
                result += "DOWN ";
            else
                result += "- ";

            if (CurrentState.IsKeyDown(Keys.Left))
            {
                if (CurrentState.IsKeyDown(Keys.Right))
                    result += "STALL ";
                else
                    result += "LEFT ";
            }
            else if (CurrentState.IsKeyDown(Keys.Right))
                result += "RIGHT ";
            else
                result += "- ";

            return result;
        }

        /*
            Notes:
            We can later do something like this:
                if (Keyboard.DirectionCheck(Keys.A, Keys.D, Horizontal) || Keyboard.AxisCheck(Keys.W, Keys.S, Vertical))
                    Velocity += Keyboard.Direction * Speed;

            The idea is to update the Direction in DirectionCheck by passing Horizontal or Vertical and then set the Velocity Vector2 based on it.
        */
        public int DirectionCheck(Keys negative, Keys positive, int both = 0)
        {
            if (Hold(negative))
            {
                if (Hold(positive))
                    return both;        // PARADO - negativa + positiva ao mesmo tempo
                else
                    return -1;
            }
            else if (Hold(positive))
                return 1;
            else
                return 0;               // PARADO - sem teclas
        }

        /*
            Notes: I want to do the following
                if(Keyboard.IsMoving())
                    Velocity += Keyboard.Direction * Speed;

            The idea is to get the Direction in IsMoving() and then set the Velocity Vector2 based on it.
        */
        public bool IsMoving()
        {
            if (CurrentState.IsKeyDown(Keys.Up) || CurrentState.IsKeyDown(Keys.Down) ||
               CurrentState.IsKeyDown(Keys.Left) || CurrentState.IsKeyDown(Keys.Right))
            {
                Direction = new Vector2(DirectionCheck(Keys.Left, Keys.Right), DirectionCheck(Keys.Up, Keys.Down));

                return true;
            }

            return false;
        }

        #endregion
    }
}

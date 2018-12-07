using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace Caieta
{
    public enum Stick
    {
        LEFT,
        RIGHT
    }

    public class GamePad
    {
        public bool IsActive { get; set; }
        public bool IsAttached { get; set; }

        public PlayerIndex PlayerIndex { get; private set; }
        private GamePadState PreviousState;
        private GamePadState CurrentState;

        public bool InvertLeftStickY { get; set; }
        public bool InvertRightStickY { get; set; }

        private List<Rumble> Rumbles;
        private float _maxRumbleLeft, _maxRumbleRight;

        public const float DEFAULT_DEADZONE = 0.1f;
        public const float DEFAULT_DIAGONAL_AVOIDANCE = 0f;
        public const float DEFAULT_TRIGGER_THRESHOLD = 0.2f;

        public event Action OnConnect;
        public event Action OnDisconnect;

        internal GamePad(PlayerIndex player)
        {
            IsActive = false;
            IsAttached = false;

            PlayerIndex = player;

            PreviousState = new GamePadState();
            CurrentState = new GamePadState();

            InvertLeftStickY = false;
            InvertRightStickY = false;

            Rumbles = new List<Rumble>();
            _maxRumbleLeft = 0;
            _maxRumbleRight = 0;

            Debug.Log("[GamePad]: GamePad " + player + " initialized.");
        }

        internal void Update()
        {
            /*
             * Update GamePads
             */
            PreviousState = CurrentState;
            if (IsActive)
                CurrentState = Microsoft.Xna.Framework.Input.GamePad.GetState(PlayerIndex);

            /*
             *  Check Gamepad State
             */
            if (PreviousState.IsConnected != CurrentState.IsConnected)
            {
                IsAttached = IsActive = CurrentState.IsConnected;

                if(IsAttached)
                {
                    Debug.Log("[GamePad]: GamePad '" + PlayerIndex + "' connected.");

                    if (OnConnect != null)
                    {
                        Debug.Log("[GamePad]: On Connect trigger for Gamepad '" + PlayerIndex + "'.");

                        OnConnect();
                        OnConnect = null;
                    }
                }
                else
                {
                    Debug.Log("[GamePad]: GamePad '" + PlayerIndex + "' disconnected.");

                    if (OnDisconnect != null)
                    {
                        Debug.Log("[GamePad]: On Disconnect trigger for Gamepad '" + PlayerIndex + "'.");

                        OnDisconnect();
                        OnDisconnect = null;
                    }
                }
            }
            // Notes: To use this, on your game class make Input.GamePad.OnConnect += YourMethod();
            // and implement the method as you wish.
            // i.e. Input.GamePad[i].OnConnect += PlayerStart(i);
            //      where i is foreach Gamepad loop PlayerIndex

            /*
             * Check Rumble state
             */
            foreach (var r in Rumbles)
            {
                if (r.Duration > 0)
                    r.Duration -= Engine.Instance.DeltaTime;
            }

            // Remove expired Rumbles
            if (Rumbles.RemoveAll(r => r.Duration <= 0) > 0)
            {
                if (Rumbles.Count == 0)     // Stop rumble if empty
                    Microsoft.Xna.Framework.Input.GamePad.SetVibration(PlayerIndex, 0, 0);
                else                        // Start new if still remaining
                {
                    _maxRumbleLeft = Rumbles.Max(r => r.StrengthL);
                    _maxRumbleRight = Rumbles.Max(r => r.StrengthR);

                    Microsoft.Xna.Framework.Input.GamePad.SetVibration(PlayerIndex, _maxRumbleLeft, _maxRumbleRight);
                }
            }

        }

        #region Utils

        public void Rumble(float strength, float duration)
        {
            Rumble(strength, strength, duration);
        }

        public void Rumble(float strengthL, float strengthR, float duration)
        {
            Rumbles.Add(new Rumble(strengthL, strengthR, duration));

            // Sort Rumble list by Strength
            //Rumbles = Rumbles.OrderBy(r => r.StrengthL).ToList();
            if (strengthL > _maxRumbleLeft) _maxRumbleLeft = strengthL;
            if (strengthR > _maxRumbleRight) _maxRumbleRight = strengthR;

            // Play first
            Microsoft.Xna.Framework.Input.GamePad.SetVibration(PlayerIndex, _maxRumbleLeft, _maxRumbleRight);

            // Can only set Rumble if it is currently OFF or values are HIGHER than max strength. 
            /*if (rumbleTime <= 0 || 
                strengthL > rumbleStrength || strengthR > rumbleStrength ||
                (strengthL == rumbleStrength && strengthR == rumbleStrength && duration > rumbleTime)
               )
            {
                Microsoft.Xna.Framework.Input.GamePad.SetVibration(PlayerIndex, strengthL, strengthR);
                rumbleStrength = strengthL > strengthR ? strengthL : strengthR;
                rumbleTime = duration;
            }*/
        }

        public void StopRumble()
        {
            Microsoft.Xna.Framework.Input.GamePad.SetVibration(PlayerIndex, 0, 0);
            Rumbles = new List<Rumble>();
        }


        public string GetHoldButton()
        {
            string result = "";

            if (Hold(Buttons.A)) result += "A ";
            if (Hold(Buttons.B)) result += "B ";
            if (Hold(Buttons.Back)) result += "Back ";
            if (Hold(Buttons.LeftShoulder)) result += "LS ";
            if (Hold(Buttons.LeftStick)) result += "LeftStick ";
            if (Hold(Buttons.LeftTrigger) || LeftTriggerHold()) result += "LT ";
            if (Hold(Buttons.RightShoulder)) result += "RS ";
            if (Hold(Buttons.RightStick)) result += "RightStick ";
            if (Hold(Buttons.RightTrigger) || RightTriggerHold()) result += "RT ";
            if (Hold(Buttons.Start)) result += "Start ";
            if (Hold(Buttons.X)) result += "X ";
            if (Hold(Buttons.Y)) result += "Y ";

            return "-";
        }

        public string GetPressedButton()
        {
            return "-";
        }

        public string GetReleasedButton()
        {
            return "-";
        }

        #endregion

        #region Button Checks

        public bool Hold(Buttons button)
        {
            if (!IsActive)
                return false;

            return CurrentState.IsButtonDown(button);
        }

        public bool Pressed(Buttons button)
        {
            if (!IsActive)
                return false;

            return CurrentState.IsButtonDown(button) && !PreviousState.IsButtonDown(button);
        }

        public bool Released(Buttons button)
        {
            if (!IsActive)
                return false;

            return !CurrentState.IsButtonDown(button) && PreviousState.IsButtonDown(button);
        }

        #endregion

        #region Stick Checks

        /*
         * Get Stick direction in Vecto2
         */
        public Vector2 GetLeftStick(float deadzone = DEFAULT_DEADZONE)
        {
            if (!IsActive)
                return Vector2.Zero;

            Vector2 ret = CurrentState.ThumbSticks.Left;
            if (ret.LengthSquared() < deadzone * deadzone)
                ret = Vector2.Zero;
            else if (InvertLeftStickY)
                ret.Y = -ret.Y;
            return ret;
        }

        public Vector2 GetRightStick(float deadzone = DEFAULT_DEADZONE)
        {
            if (!IsActive)
                return Vector2.Zero;

            Vector2 ret = CurrentState.ThumbSticks.Right;
            if (ret.LengthSquared() < deadzone * deadzone)
                ret = Vector2.Zero;
            else if (InvertRightStickY)
                ret.Y = -ret.Y;
            return ret;
        }

        /*
         * Get if Stick is in the direction while Hold, Pressed or Released
         */
        public bool LeftStickHold(InputDirection direction, float deadzone = DEFAULT_DEADZONE)
        {
            if (!IsActive)
                return false;

            switch (direction)
            {
                case InputDirection.LEFT:
                    return CurrentState.ThumbSticks.Left.X <= -deadzone;

                case InputDirection.RIGHT:
                    return CurrentState.ThumbSticks.Left.X >= deadzone;

                case InputDirection.UP:
                    return CurrentState.ThumbSticks.Left.Y >= deadzone;

                case InputDirection.DOWN:
                    return CurrentState.ThumbSticks.Left.Y <= -deadzone;

                default:
                    Debug.ErrorLog("[GamePad]: Left Stick Hold. Invalid direction '" + direction + "' for GamePad '" + PlayerIndex + "'.");
                    return false;
            }
        }

        public bool LeftStickReleased(InputDirection direction, float deadzone = DEFAULT_DEADZONE)
        {
            if (!IsActive)
                return false;

            switch (direction)
            {
                case InputDirection.LEFT:
                    return CurrentState.ThumbSticks.Left.X > -deadzone && PreviousState.ThumbSticks.Left.X <= -deadzone;

                case InputDirection.RIGHT:
                    return CurrentState.ThumbSticks.Left.X < deadzone && PreviousState.ThumbSticks.Left.X >= deadzone;

                case InputDirection.UP:
                    return CurrentState.ThumbSticks.Left.Y < deadzone && PreviousState.ThumbSticks.Left.Y >= deadzone;

                case InputDirection.DOWN:
                    return CurrentState.ThumbSticks.Left.Y > -deadzone && PreviousState.ThumbSticks.Left.Y <= -deadzone;

                default:
                    Debug.ErrorLog("[GamePad]: Left Stick Released. Invalid direction '" + direction + "' for GamePad '" + PlayerIndex + "'.");
                    return false;
            }
        }

        public bool LeftStickPressed(InputDirection direction, float deadzone = DEFAULT_DEADZONE)
        {
            if (!IsActive)
                return false;

            switch (direction)
            {
                case InputDirection.LEFT:
                    return CurrentState.ThumbSticks.Left.X <= -deadzone && PreviousState.ThumbSticks.Left.X > -deadzone;

                case InputDirection.RIGHT:
                    return CurrentState.ThumbSticks.Left.X >= deadzone && PreviousState.ThumbSticks.Left.X < deadzone;

                case InputDirection.UP:
                    return CurrentState.ThumbSticks.Left.Y >= deadzone && PreviousState.ThumbSticks.Left.Y < deadzone;

                case InputDirection.DOWN:
                    return CurrentState.ThumbSticks.Left.Y <= -deadzone && PreviousState.ThumbSticks.Left.Y > -deadzone;

                default:
                    Debug.ErrorLog("[GamePad]: Left Stick Pressed. Invalid direction '" + direction + "' for GamePad '" + PlayerIndex + "'.");
                    return false;
            }
        }


        public bool RightStickHold(InputDirection direction, float deadzone = DEFAULT_DEADZONE)
        {
            if (!IsActive)
                return false;

            switch (direction)
            {
                case InputDirection.LEFT:
                    return CurrentState.ThumbSticks.Right.X <= -deadzone;

                case InputDirection.RIGHT:
                    return CurrentState.ThumbSticks.Right.X >= deadzone;

                case InputDirection.UP:
                    return CurrentState.ThumbSticks.Right.Y >= deadzone;

                case InputDirection.DOWN:
                    return CurrentState.ThumbSticks.Right.Y <= -deadzone;

                default:
                    Debug.ErrorLog("[GamePad]: Right Stick Hold. Invalid direction '" + direction + "' for GamePad '" + PlayerIndex + "'.");
                    return false;
            }
        }

        public bool RightStickReleased(InputDirection direction, float deadzone = DEFAULT_DEADZONE)
        {
            if (!IsActive)
                return false;

            switch (direction)
            {
                case InputDirection.LEFT:
                    return CurrentState.ThumbSticks.Right.X > -deadzone && PreviousState.ThumbSticks.Right.X <= -deadzone;

                case InputDirection.RIGHT:
                    return CurrentState.ThumbSticks.Right.X < deadzone && PreviousState.ThumbSticks.Right.X >= deadzone;

                case InputDirection.UP:
                    return CurrentState.ThumbSticks.Right.Y < deadzone && PreviousState.ThumbSticks.Right.Y >= deadzone;

                case InputDirection.DOWN:
                    return CurrentState.ThumbSticks.Right.Y > -deadzone && PreviousState.ThumbSticks.Right.Y <= -deadzone;

                default:
                    Debug.ErrorLog("[GamePad]: Right Stick Released. Invalid direction '" + direction + "' for GamePad '" + PlayerIndex + "'.");
                    return false;
            }
        }

        public bool RightStickPressed(InputDirection direction, float deadzone = DEFAULT_DEADZONE)
        {
            if (!IsActive)
                return false;

            switch (direction)
            {
                case InputDirection.LEFT:
                    return CurrentState.ThumbSticks.Right.X <= -deadzone && PreviousState.ThumbSticks.Right.X > -deadzone;

                case InputDirection.RIGHT:
                    return CurrentState.ThumbSticks.Right.X >= deadzone && PreviousState.ThumbSticks.Right.X < deadzone;

                case InputDirection.UP:
                    return CurrentState.ThumbSticks.Right.Y >= deadzone && PreviousState.ThumbSticks.Right.Y < deadzone;

                case InputDirection.DOWN:
                    return CurrentState.ThumbSticks.Right.Y <= -deadzone && PreviousState.ThumbSticks.Right.Y > -deadzone;

                default:
                    Debug.ErrorLog("[GamePad]: Right Stick Pressed. Invalid direction '" + direction + "' for GamePad '" + PlayerIndex + "'.");
                    return false;
            }
        }

        /*
         * Get Stick direction in InputDirection
         */
        public InputDirection LeftStickDirection(float deadzone = DEFAULT_DEADZONE, float diagonalavoidance = DEFAULT_DIAGONAL_AVOIDANCE)
        {
            if (!IsActive)
                return InputDirection.NONE;

            // Get the length and prevent something from happening
            // if it's in our deadzone.
            var length = CurrentState.ThumbSticks.Left.Length();
            if (length < deadzone)
                return InputDirection.NONE;

            var absX = Math.Abs(CurrentState.ThumbSticks.Left.X);
            var absY = Math.Abs(CurrentState.ThumbSticks.Left.Y);
            var absDiff = Math.Abs(absX - absY);

            // We don't like values that are too close to each other
            // i.e. borderline diagonal.
            if (absDiff < length * diagonalavoidance)
                return InputDirection.NONE;

            if (absX > absY)
            {
                if (CurrentState.ThumbSticks.Left.X > 0)
                    return InputDirection.RIGHT;
                else
                    return InputDirection.LEFT;
            }
            else
            {
                if (CurrentState.ThumbSticks.Left.Y > 0)
                {
                    if (InvertLeftStickY)
                        return InputDirection.DOWN;
                    else
                        return InputDirection.UP;
                }
                else
                {
                    if (InvertLeftStickY)
                        return InputDirection.UP;
                    else
                        return InputDirection.DOWN;
                }
            }
        }

        public InputDirection RightStickDirection(float deadzone = DEFAULT_DEADZONE, float diagonalavoidance = DEFAULT_DIAGONAL_AVOIDANCE)
        {
            if (!IsActive)
                return InputDirection.NONE;

            // Get the length and prevent something from happening
            // if it's in our deadzone.
            var length = CurrentState.ThumbSticks.Right.Length();
            if (length < deadzone)
                return InputDirection.NONE;

            var absX = Math.Abs(CurrentState.ThumbSticks.Right.X);
            var absY = Math.Abs(CurrentState.ThumbSticks.Right.Y);
            var absDiff = Math.Abs(absX - absY);

            // We don't like values that are too close to each other
            // i.e. borderline diagonal.
            if (absDiff < length * diagonalavoidance)
                return InputDirection.NONE;

            if (absX > absY)
            {
                if (CurrentState.ThumbSticks.Right.X > 0)
                    return InputDirection.RIGHT;
                else
                    return InputDirection.LEFT;
            }
            else
            {
                if (CurrentState.ThumbSticks.Right.Y > 0)
                {
                    if (InvertRightStickY)
                        return InputDirection.DOWN;
                    else
                        return InputDirection.UP;
                }
                else
                {
                    if (InvertRightStickY)
                        return InputDirection.UP;
                    else
                        return InputDirection.DOWN;
                }
            }
        }

        /*
         * Get Stick direction percentage in Float (0 to 1f).
         */
        public float LeftStickHorizontal(float deadzone = DEFAULT_DEADZONE)
        {
            if (!IsActive)
                return 0;

            float horizontalDistance = CurrentState.ThumbSticks.Left.X;
            if (Math.Abs(horizontalDistance) < deadzone)
                return 0;
            else
                return horizontalDistance;
        }

        public float LeftStickVertical(float deadzone = DEFAULT_DEADZONE)
        {
            if (!IsActive)
                return 0;

            float verticalDistance = CurrentState.ThumbSticks.Left.Y;
            if (Math.Abs(verticalDistance) < deadzone)
                return 0;
            else if (InvertLeftStickY)
                return -verticalDistance;
            else
                return verticalDistance;
        }

        public float RightStickHorizontal(float deadzone = DEFAULT_DEADZONE)
        {
            if (!IsActive)
                return 0;

            float horizontalDistance = CurrentState.ThumbSticks.Right.X;
            if (Math.Abs(horizontalDistance) < deadzone)
                return 0;
            else
                return horizontalDistance;
        }

        public float RightStickVertical(float deadzone = DEFAULT_DEADZONE)
        {
            if (!IsActive)
                return 0;

            float verticalDistance = CurrentState.ThumbSticks.Right.Y;
            if (Math.Abs(verticalDistance) < deadzone)
                return 0;
            else if (InvertRightStickY)
                return -verticalDistance;
            else
                return verticalDistance;
        }

        #endregion

        #region Trigger Checks

        public float LeftTrigger()
        {
            if (!IsActive)
                return 0;

            return CurrentState.Triggers.Left;
        }


        public float RightTrigger()
        {
            if (!IsActive)
                return 0;

            return CurrentState.Triggers.Right;
        }

        public bool LeftTriggerHold(float threshold = DEFAULT_TRIGGER_THRESHOLD)
        {
            if (!IsActive)
                return false;

            return CurrentState.Triggers.Left >= threshold;
        }

        public bool LeftTriggerPressed(float threshold = DEFAULT_TRIGGER_THRESHOLD)
        {
            if (!IsActive)
                return false;

            return CurrentState.Triggers.Left >= threshold && PreviousState.Triggers.Left < threshold;
        }

        public bool LeftTriggerReleased(float threshold = DEFAULT_TRIGGER_THRESHOLD)
        {
            if (!IsActive)
                return false;

            return CurrentState.Triggers.Left < threshold && PreviousState.Triggers.Left >= threshold;
        }

        public bool RightTriggerHold(float threshold = DEFAULT_TRIGGER_THRESHOLD)
        {
            if (!IsActive)
                return false;

            return CurrentState.Triggers.Right >= threshold;
        }

        public bool RightTriggerPressed(float threshold = DEFAULT_TRIGGER_THRESHOLD)
        {
            if (!IsActive)
                return false;

            return CurrentState.Triggers.Right >= threshold && PreviousState.Triggers.Right < threshold;
        }

        public bool RightTriggerReleased(float threshold = DEFAULT_TRIGGER_THRESHOLD)
        {
            if (!IsActive)
                return false;

            return CurrentState.Triggers.Right < threshold && PreviousState.Triggers.Right >= threshold;
        }

        #endregion

        #region DPad Checks

        public int DPadHorizontal
        {
            get
            {
                return CurrentState.DPad.Right == ButtonState.Pressed ? 1 : (CurrentState.DPad.Left == ButtonState.Pressed ? -1 : 0);
            }
        }

        public int DPadVertical
        {
            get
            {
                return CurrentState.DPad.Down == ButtonState.Pressed ? 1 : (CurrentState.DPad.Up == ButtonState.Pressed ? -1 : 0);
            }
        }

        public Vector2 DPad
        {
            get
            {
                return new Vector2(DPadHorizontal, DPadVertical);
            }
        }
        //public Vector2 GetDPad(){ return new Vector2(DPadHorizontal, DPadVertical); }

        public bool DPadHold(InputDirection direction)
        {
            switch(direction)
            {
                case InputDirection.LEFT:
                    return CurrentState.DPad.Left == ButtonState.Pressed;

                case InputDirection.RIGHT:
                    return CurrentState.DPad.Right == ButtonState.Pressed;

                case InputDirection.UP:
                    return CurrentState.DPad.Up == ButtonState.Pressed;

                case InputDirection.DOWN:
                    return CurrentState.DPad.Down == ButtonState.Pressed;

                default:
                    Debug.ErrorLog("[GamePad]: DPad Hold. Invalid direction '" + direction + "' for GamePad '" + PlayerIndex + "'.");
                    return false;
            }

        }

        public bool DPadPressed(InputDirection direction)
        {
            switch (direction)
            {
                case InputDirection.LEFT:
                    return CurrentState.DPad.Left == ButtonState.Pressed && PreviousState.DPad.Left == ButtonState.Released;

                case InputDirection.RIGHT:
                    return CurrentState.DPad.Right == ButtonState.Pressed && PreviousState.DPad.Right == ButtonState.Released;

                case InputDirection.UP:
                    return CurrentState.DPad.Up == ButtonState.Pressed && PreviousState.DPad.Up == ButtonState.Released;

                case InputDirection.DOWN:
                    return CurrentState.DPad.Down == ButtonState.Pressed && PreviousState.DPad.Down == ButtonState.Released;

                default:
                    Debug.ErrorLog("[GamePad]: DPad Pressed. Invalid direction '" + direction + "' for GamePad '" + PlayerIndex + "'.");
                    return false;
            }
        }

        public bool DPadReleased(InputDirection direction)
        {
            switch (direction)
            {
                case InputDirection.LEFT:
                    return CurrentState.DPad.Left == ButtonState.Released && PreviousState.DPad.Left == ButtonState.Pressed;

                case InputDirection.RIGHT:
                    return CurrentState.DPad.Right == ButtonState.Released && PreviousState.DPad.Right == ButtonState.Pressed;

                case InputDirection.UP:
                    return CurrentState.DPad.Up == ButtonState.Released && PreviousState.DPad.Up == ButtonState.Pressed;

                case InputDirection.DOWN:
                    return CurrentState.DPad.Down == ButtonState.Released && PreviousState.DPad.Down == ButtonState.Pressed;

                default:
                    Debug.ErrorLog("[GamePad]: DPad Released. Invalid direction '" + direction + "' for GamePad '" + PlayerIndex + "'.");
                    return false;
            }
        }

        #endregion
    }

}

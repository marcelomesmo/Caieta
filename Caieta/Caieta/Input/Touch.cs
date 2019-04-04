using System;
using System.Collections.Generic;
using Caieta.Components.Attributes;
using Caieta.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace Caieta
{
    public enum MouseButton { Left, Right, Middle };

    public class Touch
    {
        [Flags]
        enum TouchAction : short
        {
            NONE = 0,
            TAP = 1,
            DOUBLE_TAP = 1 << 1,
            HOLD = 1 << 2,
            TOUCHED_OBJ = 1 << 3
        }
        private TouchAction _TouchAction;

        public bool IsActive { get; set; }

        // Mouse
        private MouseState PreviousState;
        private MouseState CurrentState;

        // Touch
        public TouchCollection PreviousTouches { get; private set; }
        public TouchCollection CurrentTouches { get; private set; }
        public List<GestureSample> PreviousGestures { get; private set; }
        public List<GestureSample> CurrentGestures { get; private set; }

        public bool IsTouchAvailable { get; private set; }
        private bool _IsTouching;
        private float _TouchTime;

        private bool HasTap;
        private float _TapWindowTime;
        private const float TAP_DELAY = 150;
        private Vector2 _TapPosition;  // Last tap/click position

        private bool _IsHolding;
        //private const float HOLD_DELAY = 150;

        public float TouchSpeed { get; private set; }
        public int TouchCount { get; private set; }

        public Action OnTap;
        public Action OnDoubleTap;
        public Action OnHold;

        public Action OnTouchStart;
        public Action OnTouchEnd;

        public List<Entity> CollisionList = new List<Entity>();

        internal Touch()
        {
            IsActive = true;

            // Notes: Is this necessary?
            PreviousState = new MouseState();
            CurrentState = new MouseState();

            PreviousTouches = new TouchCollection();
            CurrentTouches = new TouchCollection();

            PreviousGestures = new List<GestureSample>();
            CurrentGestures = new List<GestureSample>();

            OnTap = null;
            OnDoubleTap = null;
            OnHold = null;

            OnTouchStart = null;
            OnTouchEnd = null;

            Debug.Log("[Touch]: Touch initialized.");
        }

        public void CleanActions()
        {
            OnTap = null;
            OnDoubleTap = null;
            OnHold = null;

            OnTouchStart = null;
            OnTouchEnd = null;
        }
        /// 
        /// 
        /// Use this reference:
        /// https://gregfmartin.com/2017/12/27/monogame-working-with-touch/
        /// 
        ///
        internal void Update()
        {
            // Update Touch connectivity
            if (CurrentTouches.IsConnected && !IsTouchAvailable)
            {
                IsTouchAvailable = true;

                Debug.Log("[Touch]: Connected. Touch is now available.");
            }
            else if (!CurrentTouches.IsConnected && IsTouchAvailable)
            {
                IsTouchAvailable = false;

                Debug.Log("[Touch]: Disconnected. Touch is now unavailable.");
            }

            // Update Touch & Mouse inputs
            PreviousState = CurrentState;
            PreviousTouches = CurrentTouches;
            PreviousGestures = CurrentGestures;
            CurrentGestures.Clear();

            if (IsActive)
            {
                CurrentState = Microsoft.Xna.Framework.Input.Mouse.GetState();

                CurrentTouches = TouchPanel.GetState();

                while (TouchPanel.IsGestureAvailable)
                    CurrentGestures.Add(TouchPanel.ReadGesture());
            }
            else
            {
                CurrentState = new MouseState();
                CurrentTouches = new TouchCollection();
                CurrentGestures = new List<GestureSample>();
            }

            /*
             *  CHECK TOUCHES AND COLLISIONS
             */
            // Touches
            foreach (TouchLocation tl in CurrentTouches)
            {
                switch (tl.State)
                {
                    // Touch Hold
                    case TouchLocationState.Moved:
                        _IsTouching = true;
                        break;

                    // Touch Pressed
                    case TouchLocationState.Pressed:
                        _IsTouching = true;

                        TouchCount++;

                        if (OnTouchStart != null)
                        {
                            Debug.Log("[Touch]: On Touch Start trigger.");

                            OnTouchStart();
                        }

                        _TouchAction |= TouchAction.TOUCHED_OBJ;

                        break;

                    // Touch Released
                    case TouchLocationState.Released:
                        _IsTouching = false;
                        _IsHolding = false;

                        TouchCount--;

                        if (OnTouchEnd != null)
                        {
                            Debug.Log("[Touch]: On Touch End trigger.");

                            OnTouchEnd();
                        }

                        break;

                    // Touch Invalid
                    case TouchLocationState.Invalid:
                        Debug.ErrorLog("[Touch]: Invalid Touch Location.");
                        break;
                }
            }

            // Gestures
            foreach (GestureSample gesture in CurrentGestures)
            {
                switch (gesture.GestureType)
                {
                    case GestureType.Hold:

                        if(!_IsHolding)
                        {
                            _IsHolding = true;

                            if (OnHold != null)
                            {
                                Debug.Log("[Touch]: On Hold gesture trigger.");

                                OnHold();
                            }

                            _TouchAction |= TouchAction.HOLD;
                        }

                        break;

                    case GestureType.DoubleTap:

                        if (OnDoubleTap != null)
                        {
                            Debug.Log("[Touch]: On Double-Tap gesture trigger.");

                            OnDoubleTap();
                        }

                        _TouchAction |= TouchAction.DOUBLE_TAP;

                        break;

                    case GestureType.Tap:

                        if (OnTap != null)
                        {
                            Debug.Log("[Touch]: On Tap gesture trigger.");

                            OnTap();
                        }

                        _TouchAction |= TouchAction.TAP;

                        break;
                }
            }

            /*
             *  CHECK MOUSE AND COLLISIONS
             */
            if(IsMousePressed(MouseButton.Left))
            {
                _IsTouching = true;

                if (OnTouchStart != null)
                {
                    Debug.Log("[Touch]: On Touch Start trigger.");

                    OnTouchStart();
                }

                _TouchAction |= TouchAction.TOUCHED_OBJ;
            }
            else if (IsMouseReleased(MouseButton.Left))
            {
                _IsTouching = false;
                _IsHolding = false;

                if (_TouchTime < TAP_DELAY)
                {
                    if(HasTap)
                    {
                        HasTap = false;
                        _TapWindowTime = 0;

                        if (OnDoubleTap != null)
                        {
                            Debug.Log("[Touch]: On Double-Tap gesture trigger.");

                            OnDoubleTap();
                        }

                        _TouchAction |= TouchAction.DOUBLE_TAP;
                    }
                    else {
                        HasTap = true;

                        //Debug.Log("Waiting for second tap.");
                    }

                    _TapPosition = Position;
                }

                _TouchTime = 0;

                if (OnTouchEnd != null)
                {
                    Debug.Log("[Touch]: On Touch End trigger.");

                    OnTouchEnd();
                }
            }
            else if (IsMouseHold(MouseButton.Left))
            {
                if(_TouchTime >= TAP_DELAY && !_IsHolding)
                {
                    _IsHolding = true;

                    if (OnHold != null)
                    {
                        Debug.Log("[Touch]: On Hold gesture trigger.");

                        OnHold();
                    }

                    _TouchAction |= TouchAction.HOLD;
                }
            }

            if(_IsTouching)
            {
                _TouchTime += Engine.Instance.RawDeltaTime * 1000;
                _TouchTime = MathHelper.Clamp(_TouchTime, 0, TAP_DELAY);
                //Debug.Log("touch " + _TouchTime);
            }

            if(HasTap && !_IsTouching)
            {
                _TapWindowTime += Engine.Instance.RawDeltaTime * 1000;
                //Debug.Log("tap window " + _TapWindowTime);
            }

            if (_TapWindowTime >= TAP_DELAY && HasTap)
            {
                HasTap = false;
                _TapWindowTime = 0;

                //Debug.Log("No second tap appeared.");

                if (OnTap != null)
                {
                    Debug.Log("[Touch]: On Tap gesture trigger.");

                    OnTap();
                }

                _TouchAction |= TouchAction.TAP;
            }

            /*
             *  Notes: COLLISIONS
             */
            // Check for Input collisions with colliders
            if(IsActive)
            {
                if (CurrentTouches.Count > 0 || CurrentGestures.Count > 0 ||
                    IsMouseHold(MouseButton.Left) ||
                    IsMousePressed(MouseButton.Left) ||
                    IsMouseReleased(MouseButton.Left) ||
                    IsMoving || 
                    !(_TouchAction == TouchAction.NONE)
                    )
                {
                    /// TODO: Replace by Raycast check 
                   
                    // Check collision with all Entities in the Scene
                    foreach (var entity in Engine.SceneManager.SceneEntities())
                    {
                        // Check all colliders for that Entity
                        var colliders = entity.GetAll<Collider>();

                        foreach (Collider ec in colliders)
                        {
                            // Notes: This might bug if we have a Touch available
                            if (IsOverObject(ec))
                            {
                                // On Enter
                                if (!ec.IsMouseOver)
                                {
                                    ec.IsMouseOver = true;
                                    ec.OnMouseEnter?.Invoke();
                                }
                            }
                            else
                            {
                                // On Exit
                                if (ec.IsMouseOver)
                                {
                                    ec.IsMouseOver = false;
                                    ec.OnMouseExit?.Invoke();
                                }
                            }

                            // Check Tap and Double-Tap based on last tap position
                            if (ec.IsOverlapping(Engine.SceneManager.Camera.ScreenToCamera(_TapPosition)))
                            {
                                if ((_TouchAction & TouchAction.TAP) == TouchAction.TAP && ec.OnTap != null) ec.OnTap();
                                if ((_TouchAction & TouchAction.DOUBLE_TAP) == TouchAction.DOUBLE_TAP && ec.OnDoubleTap != null) ec.OnDoubleTap();
                            }

                            if (IsTouching(ec))
                            {
                                if ((_TouchAction & TouchAction.HOLD) == TouchAction.HOLD && ec.OnHold != null) ec.OnHold();
                                if ((_TouchAction & TouchAction.TOUCHED_OBJ) == TouchAction.TOUCHED_OBJ && ec.OnTouchedObject != null) ec.OnTouchedObject();
                            }
                        }
                    }
                }

                // Tap or Double-Tap has been registered
                if((_TouchAction & TouchAction.TAP) == TouchAction.TAP
                || (_TouchAction & TouchAction.DOUBLE_TAP) == TouchAction.DOUBLE_TAP)
                {
                    _TapPosition = Vector2.Zero;
                }

                _TouchAction = TouchAction.NONE;
            }

            // End Update
        }

        #region Touch

        public bool IsTouching()
        {
            return _IsTouching;
        }

        public bool IsHolding()
        {
            return _IsHolding;
        }

        // Check if touching entity collider
        public bool IsTouching(Collider collider)
        {
            /// TODO: Replace by Raycast check

            // Check if Overlapping by Mouse click
            if (collider.IsOverlapping(Engine.SceneManager.Camera.ScreenToCamera(Position)) && IsMouseHold(MouseButton.Left))
                return true;

            // Check if Overlapping by any Touch
            foreach (TouchLocation tl in CurrentTouches)
            {
                if (collider.IsOverlapping(Engine.SceneManager.Camera.ScreenToCamera(tl.Position)))
                    return true;
            }

            return false;
        }

        // Check if touching ANY entity collider
        public bool IsTouching(Entity entity)
        {
            var colliders = entity.GetAll<Collider>();

            /// TODO: Replace by Raycast check 

            foreach (Collider ec in colliders)
            {
                if (IsTouching(ec))
                    return true;
            }

            return false;
        }

        #endregion

        #region Mouse Buttons

        public bool IsMouseHold(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    return CurrentState.LeftButton == ButtonState.Pressed;

                case MouseButton.Right:
                    return CurrentState.RightButton == ButtonState.Pressed;

                case MouseButton.Middle:
                    return CurrentState.MiddleButton == ButtonState.Pressed;

                default:
                    Debug.ErrorLog("[Touch]: Mouse Button Hold. Invalid Mouse Button '" + button + "'.");
                    return false;
            }
        }

        public bool IsMousePressed(MouseButton button)
        {
            switch(button)
            {
                case MouseButton.Left: 
                    return CurrentState.LeftButton == ButtonState.Pressed && PreviousState.LeftButton == ButtonState.Released;

                case MouseButton.Right:
                    return CurrentState.RightButton == ButtonState.Pressed && PreviousState.RightButton == ButtonState.Released;

                case MouseButton.Middle:
                    return CurrentState.MiddleButton == ButtonState.Pressed && PreviousState.MiddleButton == ButtonState.Released;
        
                default:
                    Debug.ErrorLog("[Touch]: Mouse Button Pressed. Invalid Mouse Button '" + button + "'.");
                    return false;
            }
        }

        public bool IsMouseReleased(MouseButton button)
        {
            switch (button)
            {
                case MouseButton.Left:
                    return CurrentState.LeftButton == ButtonState.Released && PreviousState.LeftButton == ButtonState.Pressed;

                case MouseButton.Right:
                    return CurrentState.RightButton == ButtonState.Released && PreviousState.RightButton == ButtonState.Pressed;

                case MouseButton.Middle:
                    return CurrentState.MiddleButton == ButtonState.Released && PreviousState.MiddleButton == ButtonState.Pressed;

                default:
                    Debug.ErrorLog("[Touch]: Mouse Button Released. Invalid Mouse Button '" + button + "'.");
                    return false;
            }
        }

        #endregion

        #region Mouse Wheel

        public int Wheel
        {
            get { return CurrentState.ScrollWheelValue; }
        }

        public int WheelDelta
        {
            get { return CurrentState.ScrollWheelValue - PreviousState.ScrollWheelValue; }
        }

        #endregion

        #region Mouse Position

        public bool IsMoving
        {
            get
            {
                if (IsTouchAvailable && CurrentTouches.Count > 0)
                    return CurrentTouches[0].State == TouchLocationState.Moved;

                return CurrentState.X != PreviousState.X
                    || CurrentState.Y != PreviousState.Y;
            }
        }

        public float X
        {
            get { return Position.X; }
            //set { Position = new Vector2(value, Position.Y); }
        }

        public float Y
        {
            get { return Position.Y; }
            //set { Position = new Vector2(Position.X, value); }
        }

        public Vector2 Position
        {
            get
            {
                if (IsTouchAvailable && CurrentTouches.Count > 0)
                    return Vector2.Transform(new Vector2(CurrentTouches[0].Position.X, CurrentTouches[0].Position.Y), Matrix.Invert(Engine.ScreenMatrix));

                return Vector2.Transform(new Vector2(CurrentState.X, CurrentState.Y), Matrix.Invert(Engine.ScreenMatrix));
            }
            /*
            set
            {
                var vector = Vector2.Transform(value, Engine.ScreenMatrix);
                Microsoft.Xna.Framework.Input.Mouse.SetPosition((int)Math.Round(vector.X), (int)Math.Round(vector.Y));
            }*/
        }


        #endregion

        #region Position Utils

        public float XFrom(int index)
        {
            if (IsTouchAvailable && CurrentTouches.Count > index)
                return PositionFrom(index).X;

            return 0;
        }

        public float YFrom(int index)
        {
            if (IsTouchAvailable && CurrentTouches.Count > index)
                return PositionFrom(index).Y;

            return 0;
        }

        public Vector2 PositionFrom(int index)
        {
            if (IsTouchAvailable && CurrentTouches.Count > index)
                return Vector2.Transform(new Vector2(CurrentTouches[index].Position.X, CurrentTouches[index].Position.Y), Matrix.Invert(Engine.ScreenMatrix));

            return Vector2.Zero;
        }

        #endregion

        #region Orientation & Motion

        /*

        Compare the current device's motion as its acceleration on each axis in m/s^2 (meters per second per second). 
        The effect of gravity can be included or excluded, but note that some devices only support accelerometer values 
        including the effect of gravity and will always return 0 for acceleration excluding gravity.

        public bool CompareAccel(Axis);

        Compare the device's current orientation, if the device has a supported inclinometer. Alpha is the compass 
        direction in degrees. Beta is the device front-to-back tilt in degrees (i.e. tilting forwards away from you if
        holding in front of you). A positive value indicates front tilt and a negative value indicates back tilt. 
        Gamma is the device left-to-right tilt in degrees (i.e. twisting if holding in front of you). A positive value 
        indicates right tilt and a negative value indicates left tilt.

        public bool CompareOrientation(Alpha, Beta, Gamma); // Tilt of device

        */

        #endregion

        #region Cursor

        public bool IsOverObject(Collider collider)
        {
            // Check if Overlapping by Mouse
            if (collider.IsOverlapping(Engine.SceneManager.Camera.ScreenToCamera(Position)))
                return true;

            return false;
        }

        public bool IsOverObject(Entity entity)
        {
            var colliders = entity.GetAll<Collider>();

            foreach (Collider ec in colliders)
            {
                if (IsOverObject(ec))
                    return true;
            }

            return false;
        }

        /*
        public void ChangeCursos(Sprite sprite);
        public void ChangeCursor(CursorType type);       
        */
        #endregion

    }
}

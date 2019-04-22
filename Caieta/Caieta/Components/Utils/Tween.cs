using System;
using Microsoft.Xna.Framework;

namespace Caieta.Components.Utils
{
    public enum TweenMode { Persist, OneShot, Loop, Yoyo, Restart };
    public enum TweenProperty { X, Y, /*Position, Width, Height, Size,*/ Scale, Angle, Opacity };
   
    public class Tween : Timer
    {
        public Action OnStart;
        public Action OnFinish;

        public TweenMode Mode { get; private set; }
        public bool IsReverse { get; private set; }
        public TweenProperty Property { get; private set; }
        public float Eased { get; private set; }

        public float InitialValue { get; private set; }
        public float CurrentValue
        {
            get
            {
                if (Entity == null) // TODO
                    return 0;

                switch (Property)
                {
                    case TweenProperty.X:
                        return Entity.Transform.X;

                    case TweenProperty.Y:
                        return Entity.Transform.Y;

                    case TweenProperty.Scale:
                        return Entity.Transform.Scale.X;

                    case TweenProperty.Angle:
                        return Entity.Transform.Rotation;

                    case TweenProperty.Opacity:
                        if (Entity.Get<Sprite>() != null)
                            return Entity.Get<Sprite>().Opacity;
                        break;

                    default:
                        Debug.ErrorLog("[Tween]: Invalid Tween Property '" + Property + "'.");
                        break;
                }

                return 0;
            }
            private set
            {
                if (Entity == null) // TODO
                    return;

                switch (Property)
                {
                    case TweenProperty.X:
                        Entity.Transform.X = MathHelper.Lerp(_position.X, TargetValue, value);
                        break;

                    case TweenProperty.Y:
                        Entity.Transform.Y = MathHelper.Lerp(_position.Y, TargetValue, value);
                        break;

                    case TweenProperty.Scale:
                        Entity.Transform.Scale = Vector2.Lerp(_scale, new Vector2(TargetValue, TargetValue), value);
                        break;

                    case TweenProperty.Angle:
                        Entity.Transform.Rotation = MathHelper.Lerp(_angle, MathHelper.ToRadians(TargetValue), value);
                        break;

                    case TweenProperty.Opacity:
                        if (Entity.Get<Sprite>() != null)
                            Entity.Get<Sprite>().Opacity = MathHelper.Lerp(_opacity, TargetValue, value);
                        break;

                    default:
                        Debug.ErrorLog("[Tween]: Invalid Tween Property '" + Property + "'.");
                        break;
                }
            }
        }
        public float TargetValue { get; private set; }

        public EaseFunction.Ease Ease;
        public float Duration => TargetTime;
        public float Percent { get; private set; }

        public Tween(TweenMode mode, TweenProperty property, float value, EaseFunction.Ease easer, float duration) : base(duration)
        {
            OnStart = null;
            OnFinish = null;

            Mode = mode;
            IsReverse = false;

            Property = property;
            TargetValue = value;

            Ease = easer;
            Eased = Percent = 0;

            OnTime = Finish;

            if (duration <= 0)
                Debug.Log("[Tween]: Duration must be a positive integer. Setting from '" + duration + "'to 0 (zero).");
        }

        public override void Initialize()
        {
            base.Initialize();

            if (!InitProperties())
                return;

            StartTween();
        }

        public override void Update()
        {
            base.Update();

            // Update the percentage and eased percentage
            Percent = MathHelper.Clamp(Math.Min(ElapsedTime, TargetTime) / TargetTime, 0, 1);

            if (IsReverse)
                Percent = 1 - Percent;

            //Debug.Log("[Tween]: Tween Progress =>\n Clock Is Running " + IsRunning + "\n \tTarget Time: " + TargetTime + " Elapsed Time: " + ElapsedTime + " Percent: " + Percent + " Ease value: " + Eased);

            // Update Tween value
            Increment();
        }

        private void Increment()
        {
            if (Ease != null)
                Eased = Ease(Percent);
            else
                Eased = Percent;

            CurrentValue = Eased;
        }

        private void Finish()
        {
            Percent = 1;

            // Update Tween value one last time to avoid skipping last iteration
            Increment();

            //Debug.Log("[Tween]: OnFinish Tween trigger.");
            OnFinish?.Invoke();

            switch (Mode)
            {
                // Stop on Finish, no repeat
                case TweenMode.Persist:
                    IsActive = false;
                    break;

                // Stop and Destroy entity on Finish, no repeat
                case TweenMode.OneShot:
                    IsActive = false;
                    if (Entity != null)
                        Entity.Destroy();
                    break;

                // Loop, back and forth
                case TweenMode.Loop:
                    TargetValue = InitialValue;
                    if (!InitProperties())
                        return;
                    StartTween();
                    break;

                // Loop, reverse back
                case TweenMode.Yoyo:
                    StartTween();
                    IsReverse = !IsReverse;
                    break;

                // Loop, from the start
                case TweenMode.Restart:
                    StartTween();
                    break;

                default:
                    Debug.Log("[Tween]: Invalid Tween Mode '" + Mode + "'.");
                    break;
            }
        }

        public void StartTween()
        {
            base.Start();

            //Debug.Log("[Tween]: OnStart Tween trigger.");
            OnStart?.Invoke();

            Eased = Percent = 0;
        }

        /*/
         *      Entity starting Properties
        /*/
        private Vector2 _position;
        private Vector2 _scale;
        private float _angle;
        private Vector2 _size;
        private float _opacity;

        public bool InitProperties()
        {
            if (Entity != null)
            {
                InitialValue = CurrentValue;
                _position = Entity.Transform.Position;
                _scale = Entity.Transform.Scale;
                _angle = Entity.Transform.Rotation;

                if (Entity.Get<Sprite>() != null)
                {
                    _size = new Vector2(Entity.Get<Sprite>().Width, Entity.Get<Sprite>().Height);
                    _opacity = Entity.Get<Sprite>().Opacity;
                }
                else if (/*Property == TweenProperty.Width || Property == TweenProperty.Height ||*/ Property == TweenProperty.Opacity)
                {
                    Debug.ErrorLog("[Tween]: Couldn't start Tween property '" + Property + "'. Entity Sprite is null.");
                    return false;
                }
            }
            else
            {
                Debug.ErrorLog("[Tween]: Couldn't start Tween property '" + Property + "'. Entity is null.");
                return false;
            }

            return true;
        }

        #region Utils

        public override string ToString()
        {
            return string.Format("[Tween]: Mode: {0} Property: {1} Target Value: {2} Ease Function: {3} Duration {4}", Mode, Property, TargetValue, Ease, Duration);
        }

        #endregion
    }
}

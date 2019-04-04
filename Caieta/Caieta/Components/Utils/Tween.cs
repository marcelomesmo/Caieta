using System;
using Microsoft.Xna.Framework;

namespace Caieta.Components.Utils
{
    public enum TweenMode { Persist, OneShot, Loop, Yoyo };
    public enum TweenProperty { X, Y, /*Width, Height,*/ Scale, Angle, Opacity };
   
    public class Tween : Timer
    {
        public Action OnStart;
        public Action OnFinish;

        public TweenMode Mode { get; private set; }
        public bool Reverse { get; private set; }

        public TweenProperty Property { get; private set; }
        public float Eased { get; private set; }
        public float TargetValue { get; private set; }

        public EaseFunction.Ease Ease;
        public float Duration {
            get { return TargetTime; }
        }
        public float Percent { get; private set; }


        public Tween(TweenMode mode, TweenProperty property, float value, EaseFunction.Ease easer, float duration) : base(duration)
        {
            OnStart = null;
            OnFinish = null;

            Mode = mode;
            Reverse = false;

            Property = property;
            Eased = Percent = 0;
            TargetValue = value;

            Ease = easer;
            if (duration <= 0)
                Debug.Log("[Tween]: Duration must be a positive integer. Setting to 0 (zero).");

        }

        /*
         *      Entity starting Properties
         */
        private Vector2 _position;
        private Vector2 _scale;
        private float _angle;
        private Vector2 _size;
        private float _opacity;

        public override void Initialize()
        {
            base.Initialize();

            if (Entity != null)
            {
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
                    Debug.ErrorLog("[Tween]: Couldnt start Tween property '" + Property + "'. Entity Sprite is null.");
                    return;
                }
            }
            else
            {
                Debug.ErrorLog("[Tween]: Couldnt start Tween property '" + Property + "'. Entity is null.");
                return;
            }

            StartTween();
        }

        public override void Update()
        {
            base.Update();

            // Update the percentage and eased percentage
            Percent = Math.Min(ElapsedTime, TargetTime) / TargetTime;

            //if (Reverse)
            //    Percent = 1 - Percent;

            Debug.Log("[Tween]: Tween Progress =>\n Clock Is Running " + IsRunning + "\n \tTarget Time: " + TargetTime + " Elapsed Time: " + ElapsedTime + " Percent: " + Percent + " Ease value: " + Eased);

            if (Ease != null)
                Eased = Ease(Percent);
            else
                Eased = Percent;

            switch(Property)
            {
                case TweenProperty.X:
                    if(Entity != null)
                        Entity.Transform.X = MathHelper.Lerp(_position.X, TargetValue, Eased);
                    break;

                case TweenProperty.Y:
                    if (Entity != null)
                        Entity.Transform.Y = MathHelper.Lerp(_position.Y, TargetValue, Eased);
                    break;

                /*case TweenProperty.Position:
                    Entity.Transform.Position = Vector2.Lerp(_position, TargetPosition, Eased);
                    break;*/

                /*case TweenProperty.Width:
                    break;

                case TweenProperty.Height:
                    break;*/

                case TweenProperty.Scale:
                    if (Entity != null)
                        Entity.Transform.Scale = Vector2.Lerp(_scale, new Vector2(TargetValue, TargetValue), Eased);
                    break;

                case TweenProperty.Angle:
                    if (Entity != null)
                        Entity.Transform.Rotation = MathHelper.Lerp(_angle, MathHelper.ToRadians(TargetValue), Eased);
                    break;

                case TweenProperty.Opacity:
                    if (Entity != null && Entity.Get<Sprite>() != null)
                        Entity.Get<Sprite>().Opacity = MathHelper.Lerp(_opacity, TargetValue, Eased);
                    break;

                default:
                    Debug.Log("[Tween]: Invalid Tween Property '" + Property + "'.");
                    break;
            }

            //When finished...
            OnTime = () =>
            {
                Debug.Log("[Tween]: OnFinish Tween trigger.");
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
                        StartTween();
                        Reverse = !Reverse;
                        break;

                    // Loop, from the start
                    case TweenMode.Yoyo:
                        Debug.Log("Test");
                        StartTween();
                        break;

                    default:
                        Debug.Log("[Tween]: Invalid Tween Mode '" + Mode + "'.");
                        break;
                }
            };
        }

        public void StartTween()
        {
            base.Start();

            Debug.Log("[Tween]: OnStart Tween trigger.");
            OnStart?.Invoke();

           IsRunning = true;

            // TODO:: MUSICS ARE BUGGING TOO AND GOING OFF SCENES FROM SCENE TO SCENE
        }

        #region Utils

        public override string ToString()
        {
            return string.Format("[Tween]: Mode: {0} Property: {1} Target Value: {2} Ease Function: {3} Duration {4}", Mode, Property, TargetValue, Ease, Duration);
        }

        #endregion
    }
}

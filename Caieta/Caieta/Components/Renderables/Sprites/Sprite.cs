using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Caieta
{
    public class Sprite : Renderable
    {
        public Texture2D Texture;

        public Dictionary<string, Vector2> ImagePoint;

        public Dictionary<string, Animation> Animations;
        private Animation _CurrentAnimation;

        public Action<string> OnFinish;
        public Action<string> OnFrameChange;

        public float AnimationTimer;
        public bool IgnoreTimeRate;

        public Sprite(Texture2D texture, Rectangle sourceRect) : base(sourceRect)
        {
            Texture = texture;

            ImagePoint = new Dictionary<string, Vector2>();

            Animations = new Dictionary<string, Animation>(StringComparer.OrdinalIgnoreCase);

            _CurrentAnimation = null;
            //_CurrentAnimationName = "";
            /*OnFinish = null;
            OnLoop = null;
            OnFrameChange = null;
            OnChange = null;*/
            //IsAnimating = false;

            IgnoreTimeRate = false;
        }

        public Sprite(Texture2D texture) : this(texture, new Rectangle(0, 0, texture.Width, texture.Height)) { }

        public Sprite(Texture2D texture, int x, int y, int width, int height) : this(texture, new Rectangle(x, y, width, height)) { }

        public Sprite() : this(null, new Rectangle(0,0,0,0)) {  }

        public override void Update()
        {
            base.Update();

            if (_CurrentAnimation != null && _CurrentAnimation.IsActive)
            {
                // Update Current Animation
                if (IgnoreTimeRate)
                    AnimationTimer += Engine.Instance.RawDeltaTime * 1000;
                else
                    AnimationTimer += Engine.Instance.DeltaTime * 1000;

                _CurrentAnimation.Update();

                //Debug.Log("Current time " + AnimationTimer);

                // Next Frame
                if (AnimationTimer >= _CurrentAnimation.CurrentFrameDuration())
                {
                    //Debug.Log("Next Frame at " + AnimationTimer);
                    //OnFrameChange?.Invoke(_CurrentAnimation.Name);
                    if (OnFrameChange != null)
                        OnFrameChange(_CurrentAnimation.Name);

                    _CurrentAnimation.CurrentFrame += _CurrentAnimation.FrameDirection;

                    AnimationTimer = 0;

                    // End of Animation
                    if(_CurrentAnimation.HasEnded())
                    {
                        _CurrentAnimation.TimesPlayed++;

                        //OnFinish?.Invoke(_CurrentAnimation.Name);
                        if (OnFinish != null)
                            OnFinish(_CurrentAnimation.Name);

                        _CurrentAnimation.CalculateNextFrame();
                    }

                    _CurrentAnimation.Update();
                }
            }
        }

        public override void Render()
        {
            base.Render();

            if (IsVisible)
            {
                if(Texture != null && _CurrentAnimation == null)
                    Graphics.Draw(Texture, Transform.Position, ClipRect, Color, Transform.Rotation, Origin, Transform.Scale, Effects, 0);
                else if(_CurrentAnimation != null) {
                    Graphics.Draw(_CurrentAnimation.Sheet, Transform.Position, _CurrentAnimation.SpriteInSheet, Color, Transform.Rotation, Origin, Transform.Scale, Effects, 0);
                }
            }
        }

        public override void Unload()
        {
            ClearAnimations();
            if(Texture != null) Texture.Dispose();
            Texture = null;
        }

        // NOTE Notes: Check if necessary later and why.
        public void Dispose()
        {
            if (Texture != null) Texture.Dispose();
        }

        #region Animations

        public void SetAnimation(string name)
        {
            if (!Animations.ContainsKey(name))
                Debug.WarningLog("[Sprite]: No Animation with name '" + name + "'. Animation name invalid or not declared.");
            else
            {
                _CurrentAnimation = Animations[name];
                _CurrentAnimation.Start();
            }
        }

        public void SetFrame(int frame)
        {
            if (_CurrentAnimation != null)
                _CurrentAnimation.CurrentFrame = frame;
            else
                Debug.WarningLog("[Sprite]: No current Animation while trying to SetFrame.");
        }

        public bool IsPlaying(string name)
        {
            if (_CurrentAnimation == null)
                return false;

            if (_CurrentAnimation.Name == name)
                return true;

            return false;
        }

        public bool CompareFrame(int frame)
        {
            if (_CurrentAnimation == null)
                return false;

            if (_CurrentAnimation.CurrentFrame == frame)
                return true;
           
            return false;
        }

        public Sprite Add(Animation animation)
        {
            Animations.Add(animation.Name, animation);

            return this;
        }

        public void Pause()
        {
            if (_CurrentAnimation != null)
                _CurrentAnimation.IsActive = false;
        }

        public void UnPause()
        {
            if (_CurrentAnimation != null)
                _CurrentAnimation.IsActive = true;
        }

        #endregion

        #region Image Points

        public Sprite Add(string name, Vector2 image_point)
        {
            ImagePoint.Add(name, image_point);

            return this;
        }

        #endregion

        #region Utils

        public Sprite Clone()
        {
            return new Sprite(Texture, ClipRect)
            {
                Origin = Origin
            };
        }
        /*
        public Spritee GetSubtexture(int x, int y, int width, int height, Image applyTo = null)
        {
            if (applyTo == null)
                return new Image(Texture, x, y, width, height);
            else
            {
                applyTo.Texture = Texture;

                //applyTo.ClipRect = GetRelativeRect(x, y, width, height);
                applyTo.Width = width;
                applyTo.Height = height;
                //applyTo.Center = applyTo.Origin = new Vector2(Width, Height) * 0.5f;

                return applyTo;
            }
        }*/

        private void ClearAnimations()
        {
            Animations.Clear();
        }

        public Sprite GetSubtexture(Rectangle rect)
        {
            return new Sprite(Texture, rect);
        }

        #endregion
    }
}

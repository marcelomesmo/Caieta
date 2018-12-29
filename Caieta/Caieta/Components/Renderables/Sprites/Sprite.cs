using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Caieta
{
    public class Sprite : Renderable
    {
        public Dictionary<string, Animation> Animations;
        public Animation CurrentAnimation { get; private set; }

        public Action<string> OnFinish;
        public Action<string> OnFrameChange;

        public float AnimationTimer;
        public bool IgnoreTimeRate;

        public Sprite(Texture2D image, Rectangle sourceRect)
        {
            Animations = new Dictionary<string, Animation>(StringComparer.OrdinalIgnoreCase);

            if (image != null)
            {
                Animations.Add("default", new Animation("default", image));
                Animations["default"].AdjustRect(sourceRect);
                Animations["default"].CurrentState = Animation.LoopState.RemainOnFinalFrame;
                SetAnimation("default");
            }
            else
                CurrentAnimation = null;

            OnFinish = null;
            OnFrameChange = null;

            IgnoreTimeRate = false;
        }
        public Sprite(Texture2D texture, int x, int y, int width, int height) : this(texture, new Rectangle(x, y, width, height)) { }
        public Sprite(Texture2D texture) : this(texture, new Rectangle(0, 0, texture.Width, texture.Height)) { }
        public Sprite() : this(null, new Rectangle(0,0,0,0)) { }

        public override void Update()
        {
            base.Update();

            if (CurrentAnimation != null && CurrentAnimation.IsActive)
            {
                // Update Current Animation
                if (IgnoreTimeRate)
                    AnimationTimer += Engine.Instance.RawDeltaTime * 1000;
                else
                    AnimationTimer += Engine.Instance.DeltaTime * 1000;

                // Next Frame
                if (AnimationTimer >= CurrentAnimation.Duration)
                {
                    //OnFrameChange?.Invoke(CurrentAnimation.Name);
                    if (OnFrameChange != null)
                        OnFrameChange(CurrentAnimation.Name);

                    CurrentAnimation.CurrentFrame += CurrentAnimation.FrameDirection;

                    AnimationTimer = 0;

                    // End of Animation
                    if(CurrentAnimation.HasEnded())
                    {
                        CurrentAnimation.TimesPlayed++;

                        //OnFinish?.Invoke(_CurrentAnimation.Name);
                        if (OnFinish != null)
                            OnFinish(CurrentAnimation.Name);

                        CurrentAnimation.CalculateNextFrame();
                    }
                }
            }
        }

        public override void Render()
        {
            base.Render();

            if (IsVisible && CurrentAnimation != null)
                Graphics.Draw(CurrentAnimation.Sheet, Entity.Transform.Position, CurrentAnimation.Frame, Color, Entity.Transform.Rotation, CurrentAnimation.Origin, Entity.Transform.Scale, Effects, 0);
        }

        public override void Unload()
        {
            ClearAnimations();
            //if(Texture != null) Texture.Dispose();
            //Texture = null;
        }

        // NOTE Notes: Check if necessary later and why.
        public void Dispose()
        {
            //if (Texture != null) Texture.Dispose();
        }

        #region Animations

        public Animation Get(string name)
        {
            if (!Animations.ContainsKey(name))
            {
                Debug.WarningLog("[Sprite]: No Animation with name '" + name + "'. Animation name invalid or not declared.");
                return CurrentAnimation;
            }

            return Animations[name];
        }

        public void SetAnimation(string name)
        {
            if (!Animations.ContainsKey(name))
                Debug.WarningLog("[Sprite]: No Animation with name '" + name + "'. Animation name invalid or not declared.");
            else
            {
                CurrentAnimation = Animations[name];
                CurrentAnimation.Start();
            }
        }

        public void SetFrame(int frame)
        {
            if (CurrentAnimation != null)
                CurrentAnimation.CurrentFrame = frame;
            else
                Debug.WarningLog("[Sprite]: No current Animation while trying to SetFrame.");
        }

        public bool IsPlaying(string name)
        {
            if (CurrentAnimation == null)
                return false;

            if (CurrentAnimation.Name == name)
                return true;

            return false;
        }

        public bool CompareFrame(int frame)
        {
            if (CurrentAnimation == null)
                return false;

            if (CurrentAnimation.CurrentFrame == frame)
                return true;
           
            return false;
        }

        public Sprite Add(Animation animation)
        {
            if (Animations.ContainsKey(animation.Name))
                Debug.ErrorLog("[Sprite]: Animation with name '" + animation.Name + "' already added.");
            else
                Animations.Add(animation.Name, animation);

            return this;
        }

        public void Pause()
        {
            if (CurrentAnimation != null)
                CurrentAnimation.IsActive = false;
        }

        public void UnPause()
        {
            if (CurrentAnimation != null)
                CurrentAnimation.IsActive = true;
        }

        private void ClearAnimations()
        {
            Animations.Clear();
        }

        #endregion

        #region Utils

        /*public Sprite Clone()
        {
            if(Animations.Contains("default")
            {
                return new Sprite(CurrentAnimation.Sheet, CurrentAnimation.Frame)
                {
                    Origin = Origin
                };
            }
            else
            {
                return new Sprite()
                {
                    Animations = Animations
                };
            }
        }*/

        public void SetOrigin(Animation.Anchor anchor, Animation.AnchorPolicy policy = Animation.AnchorPolicy.CurrentAnimation)
        {
            switch(policy)
            {
                case Animation.AnchorPolicy.CurrentAnimation:
                    CurrentAnimation.SetOrigin(anchor);
                    break;

                case Animation.AnchorPolicy.AllAnimations:
                    foreach (Animation anim in Animations.Values)
                        anim.SetOrigin(anchor);
                    break;
            }
        }

        public override string ToString()
        {
            var r = string.Format("[Sprite]: Visibility: {0} Current Animation: {1}\n  Animation List:", IsVisible, CurrentAnimation.Name);
            foreach (Animation anim in Animations.Values)
                r += "\n" + anim.ToString();

            return r;
        }

        // Shortcuts
        public Vector2 Origin => CurrentAnimation.Origin;

        public int Width => CurrentAnimation.FrameWidth;

        public int Height => CurrentAnimation.FrameHeight;

        #endregion
    }
}

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Caieta
{
    public class Animation
    {
        public bool IsPlaying;

        public enum Anchor { BOTTOM_LEFT, BOTTOM, BOTTOM_RIGHT, LEFT, CENTER, RIGHT, TOP_LEFT, TOP, TOP_RIGHT }
        public enum AnchorPolicy { CurrentAnimation, AllAnimations }

        public enum LoopState
        {
            RemainOnFinalFrame,     // No Loop
            RevertToFirstFrame,     // Loop
            PingPong,               // PingPong Loop
            RepeatCount             // Loop for *count*
        }

        public LoopState CurrentState = LoopState.RevertToFirstFrame;
        public string Name { get; private set; }

        /* 
         *  Image
         */
        public Texture2D Sheet;
        public int Rows { get; private set; }
        public int Columns { get; private set; }
        // Individual Sprite sizes
        public int FrameWidth { get; private set; }
        public int FrameHeight { get; private set; }
        // Sprite position in the SpriteSheet, this is the current frame cliprect
        private readonly Rectangle[] Frames;
        public Vector2 Center { get; private set; }
        public Vector2 Origin;

        /*
         *  Frames & Time
         */
        public int CurrentFrame;
        public int TotalFrames { get; private set; }
        public int FrameDirection { get; private set; }
        private float[] FrameDuration;
        private int FirstFrame;
        private int LastFrame;

        public int TimesPlayed;
        private int _countTo = 1;   // How many times to repeat

        // Create animation using all frames on a sheet.
        public Animation(string name, Texture2D sheet, int rows, int columns, float duration, params float[] dur) : 
        this(name, sheet, sheet.Width / columns, sheet.Height / rows, 1, rows * columns, duration, dur) { }
        // Create single sprite animation
        public Animation(string name, Texture2D sheet) : this(name, sheet, 1, 1, 0) { }
        // Create single sprite animation from a sheet given start position and quantity of frames
        public Animation(string name, Texture2D sheet, int spriteWidth, int spriteHeight, int frameStart, int frameEnd, float duration, params float[] dur)
        { 
            Name = name;

            Sheet = sheet;
                       
            FrameWidth = spriteWidth;  
            FrameHeight = spriteHeight;
                       
            Rows = Sheet.Height / FrameHeight;
            Columns = Sheet.Width / FrameWidth;

            Origin = Center = new Vector2(FrameWidth * 0.5f, FrameHeight * 0.5f);

            CurrentFrame = 0;
            FirstFrame = frameStart;
            LastFrame = frameEnd;
            if(frameEnd < frameStart)
            {
                Debug.ErrorLog("[Animation]: Invalid frame position for animation '" + name + "'. Starting at: " + FirstFrame + " and trying to finish at: " + LastFrame + ". Setting Frame End to Frame Start.");
                frameEnd = frameStart;
            }
            TotalFrames = frameEnd - frameStart + 1;
            FrameDirection = 1;

            Frames = new Rectangle[TotalFrames];
            int _FrameRow, _FrameColumn;
            FrameDuration = new float[TotalFrames];
            // Fills Duration list
            for (int i = 0; i < TotalFrames; i++)
            {
                // Set & Calculate Frame position in Sprite Sheet
                _FrameRow = (i + FirstFrame - 1) / Columns;
                _FrameColumn = (i + FirstFrame - 1) % Columns;

                Frames[i] = new Rectangle(FrameWidth * _FrameColumn, FrameHeight * _FrameRow, FrameWidth, FrameHeight);

                // Set Frame duration
                // Notes: This could be shortcutted to: i < dur.Length ? Duration[i] = dur[i] : Duration[i] = duration;
                //          But I rather leave it like that for the sake of readability.
                if (dur.Length == 0)
                    FrameDuration[i] = duration;     // All frames have the same duration
                else if (i == 0)
                    FrameDuration[i] = duration;     // First-frame has "duration"
                else if (i < dur.Length)
                    FrameDuration[i] = dur[i];       // Get duration from duration list
                else
                    FrameDuration[i] = duration;     // Extra frames has default duration
                /*
                if (dur.Length != 0 && i > 0 && i < dur.Length)
                 FrameDuration[i] = dur[i];
                else
                 FrameDuration[i] = duration;
                */
            }
        }
        public Animation(string name, string sheet_path, int rows, int columns, float duration, params float[] dur) 
        : this(name, Resources.Get<Texture2D>(sheet_path), rows, columns, duration, dur) { }
        public Animation(string name, string sheet_path) : this(name, Resources.Get<Texture2D>(sheet_path), 1, 1, 0) { }
        public Animation(string name, string sheet_path, int spriteWidth, int spriteHeight, int frameStart, int frameEnd, float duration, params float[] dur)
        : this(name, Resources.Get<Texture2D>(sheet_path), spriteWidth, spriteHeight, frameStart, frameEnd, duration, dur) { }

        public void Start()
        {
            IsPlaying = true;
            CurrentFrame = 0;
            TimesPlayed = 0;
        }

        public void Stop()
        {
            IsPlaying = false;
            CurrentFrame = 0;
            TimesPlayed = 0;
        }

        #region Fluent Setters

        public Animation SetLoop(LoopState loop)
        {
            CurrentState = loop;

            return this;
        }

        public Animation SetCount(int count)
        {
            if (CurrentState != LoopState.RepeatCount)
            {
                Debug.WarningLog("[Animation]: '" + Name + "' trying to set count but loop state not set to RepeatCount.");
                return this;
            }

            _countTo = count;

            return this;
        }

        public Animation ChangeSheet(Texture2D new_sheet)
        {
            if (new_sheet.Width != Sheet.Width || new_sheet.Height != Sheet.Height)
            {
                Debug.ErrorLog("[Animation]: '" + Name + "' cant change sheets. Invalid or different sheet sizes.");
                return this;
            }

            Sheet = new_sheet;

            return this;
        }

        #endregion

        #region Utils

        /*public Rectangle CurrentFrameRect()
        {
            return Frames[CurrentFrame];
        }*/

        public Rectangle Frame => Frames[CurrentFrame];

        /*public float CurrentFrameDuration()
        {
            return Duration[CurrentFrame];
        }*/

        public float Duration => FrameDuration[CurrentFrame];

        public bool HasEnded()
        {
            return (CurrentFrame < 0 || CurrentFrame >= TotalFrames);
        }

        public void CalculateNextFrame()
        {
            switch(CurrentState)
            {
                // Stop on last frame
                case LoopState.RemainOnFinalFrame:
                case LoopState.RepeatCount:
                    if(TimesPlayed >= _countTo)
                    {
                        Stop();
                        CurrentFrame = TotalFrames - 1;
                    }
                    break;

                case LoopState.PingPong:
                    // Notes: We need to double skip last frame to avoid duplicanting.
                    // Pong
                    if (CurrentFrame < 0)
                    {
                        CurrentFrame = 1; // 1 instead of 0 to avoid duplicate
                        FrameDirection = 1;
                    }
                    // Ping
                    else
                    {
                        CurrentFrame = TotalFrames - 2; // -2 instead of -1 to avoid duplicate
                        FrameDirection = -1;
                    }

                    break;

                // Loop
                case LoopState.RevertToFirstFrame:
                    CurrentFrame = 0;
                    break;
            }
        }

        // Notes: Used for single-sprite animation
        //          Check if we can do it better later.
        public void AdjustRect(Rectangle sourceRect)
        {
            Frames[0] = sourceRect;

            FrameWidth = sourceRect.Width;
            FrameHeight = sourceRect.Height;

            Origin = Center = new Vector2(sourceRect.Width * 0.5f, sourceRect.Height * 0.5f);
        }

        public override string ToString()
        {
            return string.Format("[Animation]: '{0}' Total Frames: {1} Width: {2} Height: {3} Origin (Local Offset): {4} Center: {5} Playing: {6} using Sheet: {7}", Name, TotalFrames, FrameWidth, FrameHeight, Origin, Center, IsPlaying, Sheet);
        }

        #endregion

        #region Duration

        // Change duration of a specific frame
        public void ChangeDuration(int frame, float duration)
        {
            if (frame < 0 || frame >= TotalFrames)
                Debug.WarningLog("[Animation]: '" + Name + "' trying to set duration but frame '" + frame + "' doesnt exist or is invalid.");
            else
                FrameDuration[frame] = duration;
        }
        public void ChangeDuration(float duration, params float[] dur)
        {
            for (int i = 0; i < FrameDuration.Length; i++)
            {
                if (dur.Length == 0)
                    FrameDuration[i] = duration;     // All frames have the same duration
                else if (i < dur.Length)
                    FrameDuration[i] = dur[i];       // Get duration from duration list
                else
                    FrameDuration[i] = duration;     // Extra frames has default duration
            }
        }

        #endregion

        #region Origin

        public void SetOrigin(Anchor anchor)
        {
            // Notes: Origin is relative to local position, hence X: 0 and Y: 0.
            Rectangle ClipRect = new Rectangle(0, 0, FrameWidth, FrameHeight);

            switch (anchor)
            {
                case Anchor.BOTTOM_LEFT:
                    Origin.X = ClipRect.Left;
                    Origin.Y = ClipRect.Bottom;
                    break;
                case Anchor.BOTTOM:
                    Origin.X = ClipRect.Center.X;
                    Origin.Y = ClipRect.Bottom;
                    break;
                case Anchor.BOTTOM_RIGHT:
                    Origin.X = ClipRect.Right;
                    Origin.Y = ClipRect.Bottom;
                    break;
                case Anchor.TOP_LEFT:
                    Origin.X = ClipRect.Left;
                    Origin.Y = ClipRect.Top;
                    break;
                case Anchor.TOP:
                    Origin.X = ClipRect.Center.X;
                    Origin.Y = ClipRect.Top;
                    break;
                case Anchor.TOP_RIGHT:
                    Origin.X = ClipRect.Right;
                    Origin.Y = ClipRect.Top;
                    break;
                case Anchor.LEFT:
                    Origin.X = ClipRect.Left;
                    Origin.Y = ClipRect.Center.Y;
                    break;
                case Anchor.CENTER:
                    Origin.X = ClipRect.Center.X;
                    Origin.Y = ClipRect.Center.Y;
                    break;
                case Anchor.RIGHT:
                    Origin.X = ClipRect.Right;
                    Origin.Y = ClipRect.Center.Y;
                    break;
            }
        }

        public void SetOrigin(float x, float y)
        {
            Origin.X = x;
            Origin.Y = y;
        }

        public void CenterOrigin()
        {
            Origin.X = FrameWidth / 2f;
            Origin.Y = FrameHeight / 2f;
        }

        public void JustifyOrigin(Vector2 at)
        {
            Origin.X = FrameWidth * at.X;
            Origin.Y = FrameHeight * at.Y;
        }

        public void JustifyOrigin(float x, float y)
        {
            Origin.X = FrameWidth * x;
            Origin.Y = FrameHeight * y;
        }

        #endregion
    }
}

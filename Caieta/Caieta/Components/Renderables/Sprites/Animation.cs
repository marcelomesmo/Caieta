using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Caieta
{
    public class Animation
    {
        public bool IsActive;

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
        private int SpriteWidth, SpriteHeight;
        // Sprite position in the SpriteSheet
        public Rectangle SpriteInSheet;
        private int _FrameRow, _FrameColumn;

        /*
         *  Frames & Time
         */
        public int CurrentFrame;
        public int TotalFrames { get; private set; }
        public int FrameDirection { get; private set; }
        public float[] Duration { get; private set; }

        public int TimesPlayed;
        private int _countTo = 1;   // How many times to repeat

        public Animation(string name, Texture2D sheet, int rows, int columns, float duration, params float[] dur)
        {
            Name = name;

            Sheet = sheet;
            Rows = rows;
            Columns = columns;

            SpriteWidth = Sheet.Width / Columns;    // Width - Single Sprite
            SpriteHeight = Sheet.Height / Rows;     // Height - Single Sprite

            CurrentFrame = 0;
            TotalFrames = Rows * Columns;
            FrameDirection = 1;

            Duration = new float[TotalFrames];
            // Fills Duration list
            for(int i = 0; i < TotalFrames; i++)
            {
                // Notes: This could be shortcutted to: i < dur.Length ? Duration[i] = dur[i] : Duration[i] = duration;
                //          But I rather leave it like that for the sake of readability.
                if (dur.Length == 0)
                    Duration[i] = duration;     // All frames have the same duration
                else if (i < dur.Length)
                    Duration[i] = dur[i];       // Get duration from duration list
                else
                    Duration[i] = duration;     // Extra frames has default duration
            }
        }
        //public Animation(string name, int rows, int columns, float duration, params float[] dur) : base(name, null, rows, columns, duration, dur) { }

        public void Start()
        {
            IsActive = true;
            CurrentFrame = 0;
        }

        public void Stop()
        {
            IsActive = false;
            CurrentFrame = 0;
        }

        public void Update()
        {
            _FrameRow = CurrentFrame / Columns;
            _FrameColumn = CurrentFrame % Columns;

            SpriteInSheet = new Rectangle(SpriteWidth * _FrameColumn, SpriteHeight * _FrameRow, SpriteWidth, SpriteHeight);
        }

        public void Render()
        { 

        }

        #region Fluent Setters

        public Animation SetLoop(LoopState loop)
        {
            CurrentState = loop;

            return this;
        }

        public Animation SetCount(int count)
        {
            if(CurrentState != LoopState.RepeatCount)
            {
                Debug.WarningLog("[Animation]: '" + Name + "' trying to set count but loop state not set to RepeatCount.");
                return this;
            }

            _countTo = count;

            return this;
        }

        public Animation ChangeSheet(Texture2D new_sheet)
        {
            if(new_sheet.Width != Sheet.Width || new_sheet.Height != Sheet.Height)
            {
                Debug.ErrorLog("[Animation]: '" + Name + "' cant change sheets. Invalid or different sheet sizes.");
                return this;
            }

            Sheet = new_sheet;

            return this;
        }

        #endregion

        #region Utils

        public float CurrentFrameDuration()
        {
            return Duration[CurrentFrame];
        }

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
                        CurrentFrame = TotalFrames - 1;
                        Stop();
                    }
                    break;

                case LoopState.PingPong:
                    // Pong
                    if (CurrentFrame < 0)
                    {
                        CurrentFrame = 1; //0;
                        FrameDirection = 1;
                    }
                    else
                    {
                        CurrentFrame = TotalFrames - 2; //1;
                        FrameDirection = -1;
                    }
                    // Notes: We need to double skip last frame to avoid duplicanting.

                    break;

                // Loop
                case LoopState.RevertToFirstFrame:
                    CurrentFrame = 0;
                    break;
            }
        }

        #endregion
    }
}

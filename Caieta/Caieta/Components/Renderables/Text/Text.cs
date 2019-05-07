using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Caieta.Components.Renderables.Text
{
    public class Text : Renderable
    {
        /*
         *  Text Content
         */
        public string Content;

        /*
         *  Character
         */
        public SpriteFont Font;
        public Vector2 Size;
        public Vector2 Origin;

        /*
         *  Scrolling Text
         */
        public bool IsScrolling;
        public int ScrollDelay = 50;
        public bool HasFinishedScrolling;
        private string scrolledText;
        private float scrolledTextLength;
        // public Action OnScrollFinished;

        /*
         *  Paragraph
         */
        public enum HorizontalAlign { Left, Center, Right };
        public enum VerticalAlign { Top, Center, Bottom };
        //public enum HorizontalOverflow { Wrap, Overflow };
        //public enum VerticalOverflow { Truncate, Overflow };

        public HorizontalAlign H_Align;
        public VerticalAlign V_Align;
        //public HorizontalOverflow H_Over;
        //public VerticalOverflow V_Over;
        //public bool FitRect;

        public Text(string text, SpriteFont font)//, int size = 10)
        {
            Content = text;

            Font = font;
            //Font_Size = size;

            H_Align = HorizontalAlign.Center;
            V_Align = VerticalAlign.Center;
        }

        public override void Initialize()
        {
            base.Initialize();

            UpdatePosition();
        }

        public void UpdatePosition()
        {
            Size = Font.MeasureString(Content);
            //Position = Entity.Transform.Position;

            // Update Centering
            if (H_Align == HorizontalAlign.Left)
                Origin.X = 0;
            else if (H_Align == HorizontalAlign.Center)
                Origin.X = Size.X / 2;
            else
                Origin.X = Size.X;

            if (V_Align == VerticalAlign.Top)
                Origin.Y = 0;
            else if (V_Align == VerticalAlign.Center)
                Origin.Y = Size.Y / 2;
            else
                Origin.Y = Size.Y;

            Origin = Origin.Floor();
        }

        public void Align(Collider collider)
        {
            Align(collider.Bounds);
        }

        public void Align(Rectangle bounds)
        {
            Size = Font.MeasureString(Content);
            Vector2 pos = new Vector2(bounds.Center.X, bounds.Center.Y);

            Origin = pos;

            // Update Centering
            if (H_Align == HorizontalAlign.Left)
                Origin.X -= bounds.Width / 2;
            else if (H_Align == HorizontalAlign.Center)
                Origin.X -= Size.X / 2;
            else
                Origin.X -= -bounds.Width / 2 + Size.X; // Origin.X += bounds.Width/2 - Size.X;

            if (V_Align == VerticalAlign.Top)
                Origin.Y -= bounds.Height / 2;
            else if (V_Align == VerticalAlign.Center)
                Origin.Y -= Size.Y / 2;
            else
                Origin.Y -= -bounds.Height / 2 + Size.Y; // Origin.Y += bounds.Height/2 - Size.Y;

            Origin = Origin.Floor();

        }

        public void Justify(Vector2 justify)
        {
            Origin.X *= justify.X;
            Origin.Y *= justify.Y;
        }

        public override void Update()
        {
            base.Update();

            if (IsScrolling && !HasFinishedScrolling)
            {
                if (ScrollDelay == 0)
                {
                    scrolledText = Content;
                    HasFinishedScrolling = true;
                }
                else if (scrolledTextLength < Content.Length)
                {
                    scrolledTextLength = scrolledTextLength + Engine.Instance.DeltaTime / ScrollDelay;

                    if (scrolledTextLength >= Content.Length)
                    {
                        scrolledTextLength = Content.Length;
                        HasFinishedScrolling = true;
                    }

                    scrolledText = Content.Substring(0, (int)scrolledTextLength);
                }
            }
        }

        public override void Render()
        {
            base.Render();

            Graphics.DrawText(Font, Content, Entity.Transform.Position, Color * (Opacity/100f), Origin, Entity.Transform.Scale, Entity.Transform.Rotation);
        }

        public string FitText(BoxCollider collider)
        {
            string line = string.Empty;
            string returnString = string.Empty;
            string[] wordArray = Content.Split(' ');

            foreach (string word in wordArray)
            {
                if (Font.MeasureString(line + word).Length() > collider.Width)
                {
                    returnString = returnString + line + '\n';
                    line = string.Empty;
                }

                line = line + word + ' ';
            }

            return returnString + line;
        }
    }
}

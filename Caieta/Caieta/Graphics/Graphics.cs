using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Caieta
{
    public enum FillType { FILL, HOLLOW };
    public enum DrawStyle { NORMAL, DASHED, DOTTED };
    public enum FontSize { VERYSMALL, SMALL, MEDIUM, LARGE, EXTRALARGE };

    public static class Graphics
    {
        public static SpriteBatch SpriteBatch;

        /* Default Assets */
        public static Dictionary<FontSize, SpriteFont> DefaultFont;
        /*public static SpriteFont DefaultFontVERYSMALL { get; private set; }
        public static SpriteFont DefaultFontSMALL { get; private set; }
        public static SpriteFont DefaultFontMEDIUM { get; private set; }
        public static SpriteFont DefaultFontLARGE { get; private set; }
        public static SpriteFont DefaultFontEXTRALARGE { get; private set; }*/

        public static Texture2D BaseAtlas;
        private static Texture2D Particle;
        private static Texture2D Pixel;

        /* Default Values */
        public static Color ClearColor;
        public static Color LetterBoxColor;

        public static int Width { get; private set; }
        public static int Height { get; private set; }
        public static int ViewWidth { get; private set; }
        public static int ViewHeight { get; private set; }


        internal static void Initialize(int width, int height)
        {
            // Init Default Values
            LetterBoxColor = Color.Black;
            ClearColor = Color.CornflowerBlue;

            Width = ViewWidth = width;
            Height = ViewHeight = height;

            // Debug.Log("[Graphics]: Screen size " + ScreenWidth + " x " + ScreenHeight);

            Debug.Log("[Graphics]: Sucessfully init graphics.");
        }

        internal static void LoadContent(GraphicsDevice graphicsDevice)
        {
            SpriteBatch = new SpriteBatch(graphicsDevice);

            DefaultFont = new Dictionary<FontSize, SpriteFont>
            {
                { FontSize.VERYSMALL, Engine.Instance.Content.Load<SpriteFont>("Fonts/DefaultFontVERYSMALL") },
                { FontSize.SMALL, Engine.Instance.Content.Load<SpriteFont>("Fonts/DefaultFontSMALL") },
                { FontSize.MEDIUM, Engine.Instance.Content.Load<SpriteFont>("Fonts/DefaultFontMEDIUM") },
                { FontSize.LARGE, Engine.Instance.Content.Load<SpriteFont>("Fonts/DefaultFontLARGE") },
                { FontSize.EXTRALARGE, Engine.Instance.Content.Load<SpriteFont>("Fonts/DefaultFontEXTRALARGE") }
            };

            BaseAtlas = Engine.Instance.Content.Load<Texture2D>("Textures/Caieta-BaseAtlas");

            // Load Particles
            Color[] data;
            Pixel = new Texture2D(graphicsDevice, 1, 1);
            data = new Color[1 * 1];
            BaseAtlas.GetData(0, new Rectangle(0, 0, 1, 1), data, 0, data.Length);
            Pixel.SetData(data);
            Particle = new Texture2D(graphicsDevice, 2, 2);
            data = new Color[2 * 2];
            BaseAtlas.GetData(0, new Rectangle(0, 0, 2, 2), data, 0, data.Length);
            Particle.SetData(data);
            /*
                MTexture texture = new MTexture(2, 2, Color.White);
                Pixel = new MTexture(texture, 0, 0, 1, 1);
                Particle = new MTexture(texture, 0, 0, 2, 2);
            */

            Debug.Log("[Graphics]: Sucessfully load content.");
        }

        /*
        internal static void Begin()
        {
            // Notes: Change SamplerState to PointClamp on pixel perfect
            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Effect, Camera.Matrix * Engine.ScreenMatrix);
        }

        internal static void End()
        {
            SpriteBatch.End();
        }
        */

        internal static void Resize(int display_width, int display_height)
        {
            ViewWidth = display_width;
            ViewHeight = display_height;

            //Debug.Log("[Graphics]: Resize w " + ViewWidth + " h " + ViewHeight);
        }

        #region Text

        // Draw Text with DefaultFont
        public static void DrawText(string text, Vector2 position, Color color, FontSize size = FontSize.SMALL)
        {
            if(!DefaultFont.ContainsKey(size))
                throw new ArgumentException("[Graphics]: Default font size '" + size + "'. Name invalid or not declared.");
            else
                SpriteBatch.DrawString(DefaultFont[size], text, Calc.Floor(position), color);
        }

        /*public static void DrawTextJustified(SpriteFont font, string text, Vector2 position, Color color, Vector2 justify)
        {
            Vector2 origin = font.MeasureString(text);

            origin.X *= justify.X;
            origin.Y *= justify.Y;

            SpriteBatch.DrawString(font, text, Calc.Floor(position), color, 0, origin, 1, SpriteEffects.None, 0);
        }

        public static void DrawText(SpriteFont font, string text, Rectangle bounds, HorizontalAlign h_align, VerticalAlign v_align, Color color)
        {
            Vector2 size = font.MeasureString(text);
            Vector2 pos = bounds.GetCenter;
            Vector2 origin = size * 0.5f;
            
            // Update Centering
            if (H_Align == HorizontalAlign.Left)
                Origin.X = bounds.Width / 2;
            else if (H_Align == HorizontalAlign.Center)
                Origin.X = Size.X / 2;
            else
                Origin.X = -bounds.Width / 2 + Size.X;

            if (V_Align == VerticalAlign.Top)
                Origin.Y = bounds.Height / 2;
            else if (V_Align == VerticalAlign.Center)
                Origin.Y = Size.Y / 2;
            else
                Origin.Y = -bounds.Height / 2 + Size.Y;
                               
            if (align.HasFlag(Alignment.Left))
                origin.X += bounds.Width / 2 - size.X / 2;

            if (align.HasFlag(Alignment.Right))
                origin.X -= bounds.Width / 2 - size.X / 2;

            if (align.HasFlag(Alignment.Top))
                origin.Y += bounds.Height / 2 - size.Y / 2;

            if (align.HasFlag(Alignment.Bottom))
                origin.Y -= bounds.Height / 2 - size.Y / 2;

            DrawString(font, text, pos, color, 0, origin, 1, SpriteEffects.None, 0);
        }*/

        // Draw SpriteFont Text
        public static void DrawText(SpriteFont font, string text, Vector2 position, Color color)
        {
            SpriteBatch.DrawString(font, text, Calc.Floor(position), color);
        }

        // Draw SpriteFont Text with image point (origin), scale effect and/or rotation
        public static void DrawText(SpriteFont font, string text, Vector2 position, Color color, Vector2 origin, Vector2 scale, float rotation)
        {
            SpriteBatch.DrawString(font, text, Calc.Floor(position), color, rotation, origin, scale, SpriteEffects.None, 0);
        }
        /*
         * If you want align centered your string, you have to set origin as the half size of the string,
         * and the string will be centered in the position.
         * If you want align right your string, your origin has to be set to the size of the string.
         * */

        /*
         @ https://stackoverflow.com/questions/9108135/difference-between-origin-and-position-arguments-in-spritebatch-drawstring

                The origin is an offset related to the position.

                Position is related with the upper left corner of the string.

                If you want to rotate your string, it will rotate about that corner, but if you want to rotate about the center of the string in the given position, you have to set the origin as the half size of the string.
                Also it lets you rotate about an arbitrary point, doing the right math with origin.
                If you want align centered your string, you have to set origin as the half size of the string, and the string will be centered in the position.
                If you want align right your string, your origin has to be set to the size of the string.

         */
        #endregion

        #region Image

        public static void Draw(Texture2D texture)
        {
            SpriteBatch.Draw(texture, Vector2.Zero, Color.White);
        }

        public static void Draw(Texture2D Texture, Vector2 Position, Rectangle ClipRect, Color Color, float Rotation, Vector2 Origin, Vector2 Scale, SpriteEffects Effects, float depth)
        {
            SpriteBatch.Draw(Texture, Position, ClipRect, Color, Rotation, Origin, Scale, Effects, depth);
        }

        public static void Draw(Texture2D Texture, Rectangle Destination, Rectangle ClipRect, Color Color, float Rotation, Vector2 Origin, SpriteEffects Effects, float depth)
        {
            SpriteBatch.Draw(Texture, Destination, ClipRect, Color, Rotation, Origin, Effects, depth);
        }

        public static void Draw(Texture2D Texture, Vector2 Position, Rectangle ClipRect, Color Color)
        {
            SpriteBatch.Draw(Texture, Position, ClipRect, Color);
        }

        public static void Draw(Texture2D Texture, Rectangle Destination, Rectangle ClipRect, Color Color)
        {
            SpriteBatch.Draw(Texture, Destination, ClipRect, Color);
        }

        #endregion

        #region Shapes

        private static Rectangle _rect;

        // Draw Pixel
        public static void DrawPoint(Vector2 position, Color color, int opacity = 100)
        {
            // Fix opacity
            if (opacity < 0) opacity = 0;
            else if (opacity > 100) opacity = 100;

            position.X = (int)Math.Round(position.X);
            position.Y = (int)Math.Round(position.Y);

            SpriteBatch.Draw(Pixel, position, color * (opacity / 100f));
        }

        // Draw Line
        public static void DrawLine(Vector2 start, Vector2 end, Color color, int opacity = 100)
        {
            DrawLineAgle(start, Calc.Angle(start, end), Vector2.Distance(start, end), color, opacity);
        }

        public static void DrawLine(Vector2 start, Vector2 end, Color color, float thickness, int opacity = 100)
        {
            DrawLineAgle(start, Calc.Angle(start, end), Vector2.Distance(start, end), color, thickness, opacity);
        }

        public static void DrawLine(float x1, float y1, float x2, float y2, Color color, int opacity = 100)
        {
            DrawLine(new Vector2(x1, y1), new Vector2(x2, y2), color, opacity);
        }

        public static void DrawLineAgle(Vector2 start, float angle, float length, Color color, int opacity = 100)
        {
            // Fix opacity
            if (opacity < 0) opacity = 0;
            else if (opacity > 100) opacity = 100;

            start.X = (int)Math.Round(start.X);
            start.Y = (int)Math.Round(start.Y);

            //length = (int)Math.Round(length;

            SpriteBatch.Draw(Pixel, start, null, color * (opacity / 100f), angle, Vector2.Zero, new Vector2(length, 1), SpriteEffects.None, 0);
        }

        public static void DrawLineAgle(Vector2 start, float angle, float length, Color color, float thickness, int opacity = 100)
        {
            // Fix opacity
            if (opacity < 0) opacity = 0;
            else if (opacity > 100) opacity = 100;

            start.X = (int)Math.Round(start.X);
            start.Y = (int)Math.Round(start.Y);

            //length = (int)Math.Round(length;

            SpriteBatch.Draw(Pixel, start, null, color * (opacity / 100f), angle, new Vector2(0, .5f), new Vector2(length, thickness), SpriteEffects.None, 0);
        }

        public static void DrawLineAgle(float startX, float startY, float angle, float length, Color color, int opacity = 100)
        {
            DrawLineAgle(new Vector2(startX, startY), angle, length, color, opacity);
        }

        public static void DrawLineDashed(Vector2 start, Vector2 end, int lineSize, int gapSize, Color color, int opacity = 100)
        {
            Vector2 position = start;

            // We calc the lenght of the line
            Vector2 lineVec = new Vector2(end.X - start.X, end.Y - start.Y);
            float maxLength = lineVec.Length();
            lineVec.Normalize();
            // We then normalize the vector to know the current direction

            // We scale the gap and dash in that direction
            Vector2 dashVec = lineVec * lineSize;
            Vector2 gapVec = lineVec * gapSize;

            //Debug.Log("line (norm) " + lineVec + " lenght " + maxLength + " dash " + dashVec + " gap " + gapVec); 

            // We then intercalate draws of dashed lines and skip the gaps.
            float length = 0;
            while (length < maxLength)
            {
                Vector2 currEnd = position + dashVec;
                if (currEnd.X > end.X) currEnd.X = end.X;
                if (currEnd.Y > end.Y) currEnd.Y = end.Y;

                DrawLine(position, currEnd, color, opacity);

                position += dashVec + gapVec;

                length += dashVec.Length() + gapVec.Length();
            }
        }

        /*public static void DrawLineAngleDashed(float startX, float startY, float angle, float length, Color color, int opacity = 100)
        {

        }*/

        // Draw Circle

        // Draw Rect
        public static void DrawRectDashed(float x, float y, float width, float height, Color color, int lineSize, int gapSize, int opacity = 100)
        {
            // Draw upper line
            DrawLineDashed(new Vector2(x, y), new Vector2(x + width, y), lineSize, gapSize, color, opacity);
            // Draw bottom line
            DrawLineDashed(new Vector2(x, y + height), new Vector2(x + width, y + height), lineSize, gapSize, color, opacity);
            // Draw left line
            DrawLineDashed(new Vector2(x, y), new Vector2(x, y + height), lineSize, gapSize, color, opacity);
            // Draw right line
            DrawLineDashed(new Vector2(x + width, y), new Vector2(x + width, y + height), lineSize, gapSize, color, opacity);
        }

        public static void DrawRect(float x, float y, float width, float height, Color color, int opacity = 100, FillType fill = FillType.HOLLOW)
        {
            // Fix opacity
            if (opacity < 0) opacity = 0;
            else if(opacity > 100) opacity = 100;

            switch(fill)
            {
                // Draw Filled Rect
                case FillType.FILL:

                    _rect.X = (int)Math.Round(x);
                    _rect.Y = (int)Math.Round(y);
                    _rect.Width = (int)Math.Round(width);
                    _rect.Height = (int)Math.Round(height);

                    SpriteBatch.Draw(Pixel, _rect, color * (opacity/100f));
                    //(Pixel.Texture, rect, Pixel.ClipRect, color);

                    break;

                // Draw Hollow Rect
                case FillType.HOLLOW:

                    _rect.X = (int)Math.Round(x);
                    _rect.Y = (int)Math.Round(y);
                    _rect.Width = (int)Math.Round(width);
                    _rect.Height = 1;

                    SpriteBatch.Draw(Pixel, _rect, color);

                    _rect.Y += (int)Math.Round(height - 1);

                    SpriteBatch.Draw(Pixel, _rect, color);

                    _rect.Y -= (int)Math.Round(height - 1);
                    _rect.Width = 1;
                    _rect.Height = (int)Math.Round(height);

                    SpriteBatch.Draw(Pixel, _rect, color);

                    _rect.X += (int)Math.Round(width - 1);

                    SpriteBatch.Draw(Pixel, _rect, color);

                    break;

                default:

                    break;
            }
        }

        public static void DrawRect(Vector2 position, float width, float height, Color color, int opacity = 100, FillType fill = FillType.HOLLOW)
        {
            DrawRect(position.X, position.Y, width, height, color, opacity, fill);
        }

        public static void DrawRect(Rectangle rect, Color color, int opacity = 100, FillType fill = FillType.HOLLOW)
        {
            DrawRect(rect.X, rect.Y, rect.Width, rect.Height, color, opacity, fill);
        }

        #endregion

        #region Utils

        public static void ResetColor()
        {
            LetterBoxColor = Color.Black;
            ClearColor = Color.CornflowerBlue;
        }

        #endregion
    }
}

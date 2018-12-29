using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Caieta
{
    public enum FillType { FILL, HOLLOW };
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

        internal static void Begin()
        {
            // Notes: Change SamplerState to PointClamp on pixel perfect
            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, /*SamplerState.LinearClamp*/SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null,/*Effect, Camera.Matrix * */Engine.ScreenMatrix);
        }

        internal static void End()
        {
            SpriteBatch.End();
        }

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

        #endregion


        #region Shapes

        private static Rectangle _rect;

        public static void DrawPoint(Vector2 position, Color color, int opacity = 100)
        {
            SpriteBatch.Draw(Pixel, position, color * (opacity / 100f));
        }
    
        // Draw Line

        // Draw Circle

        public static void DrawRect(float x, float y, float width, float height, Color color, int opacity = 100, FillType fill = FillType.HOLLOW)
        {
            // Fix opacity
            if (opacity < 0) opacity = 0;
            else if(opacity > 100) opacity = 100;

            switch(fill)
            {
                // Draw Filled Rect
                case FillType.FILL:

                    _rect.X = (int)x;
                    _rect.Y = (int)y;
                    _rect.Width = (int)width;
                    _rect.Height = (int)height;

                    SpriteBatch.Draw(Pixel, _rect, color * (opacity/100f));
                    //(Pixel.Texture, rect, Pixel.ClipRect, color);

                    break;

                // Draw Hollow Rect
                case FillType.HOLLOW:

                    _rect.X = (int)x;
                    _rect.Y = (int)y;
                    _rect.Width = (int)width;
                    _rect.Height = 1;

                    SpriteBatch.Draw(Pixel, _rect, color);

                    _rect.Y += (int)height - 1;

                    SpriteBatch.Draw(Pixel, _rect, color);

                    _rect.Y -= (int)height - 1;
                    _rect.Width = 1;
                    _rect.Height = (int)height;

                    SpriteBatch.Draw(Pixel, _rect, color);

                    _rect.X += (int)width - 1;

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

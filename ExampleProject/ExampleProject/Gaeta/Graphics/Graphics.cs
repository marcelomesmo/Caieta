using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gaeta
{
    public static class Graphics
    {
        private static SpriteBatch spriteBatch;

        public static SpriteFont DefaultFont { get; private set; }

        public static Color ClearColor;

        internal static void Initialize()
        {
            ClearColor = Color.Black;

#if DEBUG
            Console.WriteLine("[Graphics]: Sucessfully init graphics.");
#endif
        }

        internal static void LoadContent(GraphicsDevice graphicsDevice)
        {
            spriteBatch = new SpriteBatch(graphicsDevice);

            DefaultFont = Engine.Instance.Content.Load<SpriteFont>("Fonts/DefaultFont");

            // Load Particles
            /*
                MTexture texture = new MTexture(2, 2, Color.White);
                Pixel = new MTexture(texture, 0, 0, 1, 1);
                Particle = new MTexture(texture, 0, 0, 2, 2);
            */

#if DEBUG
            Console.WriteLine("[Graphics]: Sucessfully load content.");
#endif
        }
    }
}

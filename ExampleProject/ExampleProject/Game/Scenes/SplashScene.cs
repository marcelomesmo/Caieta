using System;
using Caieta;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ExampleProject
{
    public class SplashScene : Scene
    {
        public SplashScene() : base()
        {
        }

        public override void Begin()
        {
            base.Begin(); 

            // Create Layers
            Add(new Layer("Default"));

            // Add entities to Layers
            Layers["Default"].Add(new Logo("Splash"));

            // Draw a Black background
            Graphics.ClearColor = Color.Black;
        }

        public override void End()
        {
            base.End();

            Graphics.ResetColor();
        }

        public override void Update()
        {
            base.Update();

            if (Input.Keyboard.Pressed(Keys.A))
                Engine.SceneManager.LoadScene("Menu");
        }

        public override void Render()
        {
            Graphics.DrawText("SPLASH SCREEN", new Vector2(280, 220), Color.White);

            base.Render();
        }
    }
}

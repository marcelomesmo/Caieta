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

        Logo logo = new Logo("Splash");

        public override void Awake()
        {
            base.Awake(); 

            // Create Layers
            Add(new Layer("Default"));

            // Add entities to Layers
            Layers["Default"].Add(logo);

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


            Input.Touch.OnDoubleTap = () =>
            {
                Debug.Log("Double Tap 1");
            };

            Input.Touch.OnTouchStart = () =>
            {
                Debug.Log("Touch Start");
            };

            Input.Touch.OnTouchEnd = () =>
            {
                Debug.Log("Touch End");
            };

        }

        /*
        public override void Render()
        {
            base.Render();

            Graphics.DrawText("SPLASH SCREEN", new Vector2(280, 220), Color.White);

            var text = "LOREM IPSULUM LOREM IPSULUM LOREM IPSULUM LOREM IPSULUM LOREM IPSULUM LOREM IPSULUM " +
                "LOREM IPSULUM LOREM IPSULUM LOREM IPSULUM LOREM IPSULUM LOREM IPSULUM LOREM IPSULUM LOREM IPSULUM " +
                "LOREM IPSULUM LOREM IPSULUM LOREM IPSULUM LOREM IPSULUM LOREM IPSULUM LOREM IPSULUM LOREM IPSULUM " +
                "LOREM IPSULUM LOREM IPSULUM LOREM IPSULUM LOREM IPSULUM LOREM IPSULUM ";

            //Graphics.DrawTextJustified(text, new Vector2(100, 100), Color.White, new Vector2(0, 0));
        }
        */
    }
}

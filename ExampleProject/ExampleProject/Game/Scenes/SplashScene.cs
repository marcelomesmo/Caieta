using System;
using Caieta;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ExampleProject.Game.Scenes
{
    public class SplashScene : Scene
    {
        public SplashScene() : base()
        {
        }

        public override void Begin()
        {
            base.Begin();

        }

        public override void End()
        {
            base.End();

        }

        public override void Update()
        {
            base.Update();

            if (Input.Keyboard.Pressed(Keys.A))
            {
                Engine.SceneManager.LoadScene("Menu");
            }
        }

        public override void Render()
        {
            base.Render();

            Graphics.DrawText("SPLASH SCREEN", new Vector2(100, 100), Color.White);
        }
    }
}

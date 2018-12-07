using System;
using Caieta;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ExampleProject.Game.Scenes
{
    public class MenuScene : Scene
    {
        public MenuScene() : base()
        {
            OnSceneStart += LoadTextStart;
            OnSceneEnd += LoadTextEnd;
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

            // TODO: Add your update logic here
            if (Input.Keyboard.Pressed(Keys.S))
            {
                Engine.SceneManager.LoadScene("Fase1");
            }
        }

        public override void Render()
        {
            base.Render();

            Graphics.DrawText("MENU SCREEN", new Vector2(Graphics.Width/2, Graphics.Height/2), Color.White);

        }

        public void LoadTextStart()
        {
            Debug.Log("[MenuScene]: OnSceneStart test.");
        }

        public void LoadTextEnd()
        {
            Debug.Log("[MenuScene]: OnSceneEnd test.");
        }
    }
}

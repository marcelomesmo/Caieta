using System;
using Caieta;
using Caieta.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ExampleProject.Game.Scenes
{
    public class MenuScene : Scene
    {
        Player Player1;

        Background Bg;

        public MenuScene() : base()
        {
            OnSceneStart += LoadTextStart;
            OnSceneEnd += LoadTextEnd;

            Player1 = new Player("Player1");

            Bg = new Background("Background1");
        }

        public override void Begin()
        {
            base.Begin();

            Add(new Layer("Background"));
            Add(new Layer("HUD"));
            Layers["HUD"].SetGlobal();
            Add(new Layer("Objects"));

            Bg.Add(new Solid(100, 200, 0, 0));

            Layers["Background"].Add(Bg);

            Layers["HUD"].Add(Player1);
        }

        public override void End()
        {
            base.End();

        }

        public override void Update()
        {
            base.Update();

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

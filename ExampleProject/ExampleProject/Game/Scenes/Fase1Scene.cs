using System;
using Caieta;
using Microsoft.Xna.Framework;

namespace ExampleProject.Game.Scenes
{
    public class Fase1Scene : Scene
    {
        public Fase1Scene() : base()
        {
            OnSceneStart += LoadTextStart;
            OnSceneEnd += LoadTextEnd;

        }

        public override void Begin()
        {
            base.Begin();

        }

        public override void Render()
        {
            base.Render();

            Graphics.DrawText("FASE1 SCREEN", new Vector2(Graphics.Width / 2, Graphics.Height / 2), Color.White /*, Text::Centered*/);

        }

        public void LoadTextStart()
        {
            Debug.Log("[Fase1]: OnSceneStart test.");
        }

        public void LoadTextEnd()
        {
            Debug.Log("[Fase1]: OnSceneEnd test.");
        }
    }
}

using System;
using Caieta;
using Caieta.Audio;
using Caieta.Scenes.Transitions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TiledSharp;

namespace ExampleProject.Game.Scenes
{
    public class Fase1Scene : Scene
    {
        public Fase1Scene() : base()
        {
            OnSceneStart += LoadTextStart;
            OnSceneEnd += LoadTextEnd;

            Layout = new Rectangle(0, 0, 1200, 1200);
        }

        FadeIn FadeIn;
        FadeOut FadeOut;

        public override void Begin()
        {
            base.Begin();

            //Add(new Layer("HUD").SetGlobal(true));
            Add(new Layer("HUD"));
            Add(new Layer("TOP"));

            /*
             *  FADE
             */
             // Fade In Test
            FadeIn = new FadeIn(2000);
            Layers["TOP"].Add(FadeIn);

            FadeIn.Start();
            Pause();
            FadeIn.OnFinish = Resume;

            // Fade Out Test
            FadeOut = new FadeOut(50);
            Layers["TOP"].Add(FadeOut);

            AudioManager.SFX.Load("Coleta");
            AudioManager.SFX.Load("Correndo_2_v2");
        }

        public override void Update()
        {
            base.Update();

            if (Input.Keyboard.Pressed(Keys.M))
                FadeIn.Start();

            if (Input.Keyboard.Pressed(Keys.O))
                FadeOut.Start();

            FadeOut.OnFinish = () =>
            {
                Engine.SceneManager.LoadScene("Splash");
            };

        }

        /*
        public override void Render()
        {
            base.Render();

            Graphics.DrawText("FASE1 SCREEN", new Vector2(Graphics.Width / 2, Graphics.Height / 2), Color.White, Text::Centered);

        }
        */

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

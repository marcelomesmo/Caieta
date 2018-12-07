using System;
using System.Collections.Generic;
using Caieta;
using ExampleProject.Game.Scenes;

namespace ExampleProject.Desktop
{
    /// <summary>
    /// The main class.
    /// </summary>
    public static class TestProgram
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var game = new Engine("Teste", 480, 270, 960, 540, 
                new Dictionary<string, Scene>
                {
                    ["Splash"] = new SplashScene(),
                    ["Menu"] = new MenuScene(),
                    ["Fase1"] = new Fase1Scene()
                }
            , "Splash", false))
                game.Run();
        }
    }
}

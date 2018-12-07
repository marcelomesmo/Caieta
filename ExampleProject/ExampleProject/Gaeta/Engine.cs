using System;
using System.IO;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gaeta
{
    public class Engine : Game
    {

        public static Engine Instance { get; private set; }

        public string Title;
        public Version Version;

        /*
         *      GRAPHICS & INPUT
         */
        private static GraphicsDeviceManager graphicsManager;
        // (static) Input

        /*
         *      GAME WINDOW
         */
        public static int Width { get; set; }
        public static int Height { get; set; }
        //public static int ViewWidth { get; private set; }
        //public static int ViewHeight { get; private set; }
        public static bool IsFullScreen { get; set; }
        public static bool ExitOnEscapeKeypress;

        /*
         *      UPDATE TIMER
         */
        public float DeltaTime;         // RawDelta * TimeRate
        public float RawDeltaTime;
        public float TimeRate = 1f;
        // How many ticks we want to freeze (skip) until next update (1000 / targetFPS).
        // If you find that your app is very performance-intensive you may want to set it to 1000 / 30 for instance.
        public float TimeStep;
        //float timestamp;        // Current time stamp.
        //float lastFrameTime;    // Time since last frame update.

        /*
         *      Frame Rate per Second
         */
        public float TargetFPS;                             // Our target (max) Frames Per Second
        public float FPS;                                   // Current FPS
        private TimeSpan lastFpsUpdateTime = TimeSpan.Zero; // Last time we updated FPS
        private float fpsFrameCount = 0;                    // Count how many frames has been draw in the last FPS_UPDATE_TIME

        /*
         *      CONTENT DIRECTORY
         */
#if !CONSOLE
        private string AssemblyDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
#endif

        public string ContentDirectory
        {
#if PS4
            get { return Path.Combine("/app0/", Content.RootDirectory); }
#elif NSWITCH
            get { return Path.Combine("rom:/", Content.RootDirectory); }
#elif XBOXONE
            get { return Content.RootDirectory; }
#else
            get { return Path.Combine(AssemblyDirectory, Content.RootDirectory); }
#endif
        }

        /*
         *      START ENGINE
         */
        public Engine(string gameTitle, int width, int height, bool fullscreen = false)
        {
            Title = Window.Title = gameTitle;

            Width = width;
            Height = height;
            IsFullScreen = fullscreen;

            graphicsManager = new GraphicsDeviceManager(this)
            {
                //graphicsManager.DeviceReset += OnGraphicsReset;
                //graphicsManager.DeviceCreated += OnGraphicsCreate;
                SynchronizeWithVerticalRetrace = true,
                PreferMultiSampling = false,
                GraphicsProfile = GraphicsProfile.HiDef,
                PreferredBackBufferFormat = SurfaceFormat.Color,
                PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8
            };

            graphicsManager.ApplyChanges();

        #if PS4 || XBOXONE
            graphicsManager.PreferredBackBufferWidth = 1920;
            graphicsManager.PreferredBackBufferHeight = 1080;
        #elif NSWITCH
            graphicsManager.PreferredBackBufferWidth = 1280;
            graphicsManager.PreferredBackBufferHeight = 720;
        #else
            Window.AllowUserResizing = true;
            //game.Window.ClientSizeChanged += OnClientSizeChanged;

            if (IsFullScreen)
            {
                graphicsManager.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                graphicsManager.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                graphicsManager.IsFullScreen = true;
            }
            else
            {
                graphicsManager.PreferredBackBufferWidth = Engine.Width;
                graphicsManager.PreferredBackBufferHeight = Engine.Height;
                graphicsManager.IsFullScreen = false;
            }
        #endif

            Content.RootDirectory = @"Content";

        #if DEBUG
            Console.WriteLine("[Engine]: Content Directory " + Content.RootDirectory + " at" );
            Console.WriteLine("   folder " + ContentDirectory);
#endif

            IsMouseVisible = false;
            IsFixedTimeStep = false;
            ExitOnEscapeKeypress = true;
        }

        protected override void Initialize()
        {
            base.Initialize();

            Graphics.Initialize();
            //Input.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            Graphics.LoadContent(GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            RawDeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            DeltaTime = RawDeltaTime * TimeRate;

            //Update input
            //Input.Update();
            /*
        #if !CONSOLE
            if (ExitOnEscapeKeypress && Input.Keyboard.Pressed(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                Exit();
                return;
            }
        #endif
            */

            // Calculate FPS
            fpsFrameCount++;
            lastFpsUpdateTime += gameTime.ElapsedGameTime;      // ElapsedGameTime gives the time since last update
            if (lastFpsUpdateTime >= TimeSpan.FromSeconds(1))
            {
#if DEBUG
                Window.Title = Title + " " + fpsFrameCount.ToString() + " fps - " + (GC.GetTotalMemory(false) / 1048576f).ToString("F") + " MB";
#endif

                FPS = fpsFrameCount;
                fpsFrameCount = 0;
                lastFpsUpdateTime -= TimeSpan.FromSeconds(1);
            }

            // Update current scene
            if (TimeStep > 0)
                TimeStep = Math.Max(TimeStep - RawDeltaTime, 0);
            /*else if (scene != null)
            {
                scene.BeforeUpdate();
                scene.Update();
                scene.AfterUpdate();
            }*/

            //Update transitions - scenes
            /*if (scene != nextScene)
            {
                var lastScene = scene;
                if (scene != null)
                    scene.End();
                scene = nextScene;
                OnSceneTransition(lastScene, nextScene);
                if (scene != null)
                    scene.Begin();
            }*/

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //RenderCore();
            /*if (scene != null)
                scene.BeforeRender();
            */
            GraphicsDevice.SetRenderTarget(null);       // Render null
            //GraphicsDevice.Viewport = Viewport;       
            GraphicsDevice.Clear(Graphics.ClearColor);  // Clear screen

            /*if (scene != null)
            {
                scene.Render();
                scene.AfterRender();
            }*/

            base.Draw(gameTime);
        }
    }
}

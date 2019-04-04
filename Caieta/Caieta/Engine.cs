using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Caieta.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Caieta
{
    public class Engine : Game
    {
        // Notes: unimplemented
        /*public enum ResolutionPolicy
        {
            PixelPerfect,
            BestFit
        }*/

        public static Engine Instance { get; private set; }

        public string Title;
        public Version Version;

        /*
         *      DEBUGGER
         */
        public Debugger _Debugger;

        /*
         *      SCENES
         */
        // Notes: Turning this into a interface would be more "correct" but less convenient.
        /*
         * private SceneManager _SceneManager;
         * public ISceneManager SceneManager() { return (ISceneManager)_SceneManager; }
         * 
         * This will allow for methods to be encapsulated.
         * 
         * public interface ISceneManager { void LoadScene(); }
         * 
         * public class SceneManager : ISceneManager { 
         *      public void Update() { } 
         *      public void LoadScene() { }
         * } 
         * 
         * On a side note: internal should be enough when separating assemblies.
         * 
         */
        public static SceneManager SceneManager;

        /*
         *      GRAPHICS
         */
        private static GraphicsDeviceManager GraphicsDeviceManager;
        public static Viewport Viewport { get; private set; }
        public static Matrix ScreenMatrix { get; private set; }
        /*public static int ViewPadding
        {
            get { return viewPadding; }
            set
            {
                viewPadding = value;
                Instance.UpdateView();
            }
        }
        private static int viewPadding = 0;*/
        private RenderTarget2D _destinationRender;

        /*
         *      GAME WINDOW
         */
        private static int Width;
        private static int Height;
        private static int ViewWidth;
        private static int ViewHeight;
        public static bool IsFullScreen;
        private static bool _resizing;
        public static bool IsPixelPerfect;
        public static bool ExitOnEscapeKeypress;

        /*
         *      UPDATE TIMER
         */
        public float DeltaTime;         // RawDelta * TimeRate
        public float RawDeltaTime;
        public float TimeRate = 1f;
        // How many ticks we want to freeze (skip) until next update (1000 / targetFPS).
        // If you find that your app is very performance-intensive you may want to set it to 1000 / 30 for instance.
        private float _TimeStep;

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
        public Engine(string gameTitle, int width, int height, int display_w, int display_h, Dictionary<string, Scene> scenes, string firstScene, bool fullscreen = false, bool pixelperfect = true, bool fixedtimestep = false)
        {
            Instance = this;

            AudioManager.Initialize();

            // Notes: Send this to Initialize?
            _Debugger = new Debugger();

            SceneManager = new SceneManager(scenes, firstScene);

            Title = Window.Title = gameTitle;

            Width = width;
            Height = height;
            IsFullScreen = fullscreen;
            // Notes: Replace by ResolutionPolicy enum
            IsPixelPerfect = pixelperfect;

            IsFixedTimeStep = fixedtimestep;
            // Notes: Update to make it more dynamic (?)
            TargetFPS = 60;
            _TimeStep = 1000 / TargetFPS;

            GraphicsDeviceManager = new GraphicsDeviceManager(this)
            {
                SynchronizeWithVerticalRetrace = true,
                PreferMultiSampling = false,
                GraphicsProfile = GraphicsProfile.HiDef,
                PreferredBackBufferFormat = SurfaceFormat.Color,
                PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8
            };

            GraphicsDeviceManager.DeviceReset += OnGraphicsReset;
            GraphicsDeviceManager.DeviceCreated += OnGraphicsCreate;

            // Will ignore the input Display Width and Height 
            // in case we are on PS4, XBOXONE, SWITCH or FullScreen
#if PS4 || XBOXONE
            TargetWidth = 1920;
            TargetHeight = 1080;
#elif NSWITCH
            TargetWidth = 1280;
            TargetHeight = 720;
#else
            Window.ClientSizeChanged += OnClientSizeChanged;

            if (IsFullScreen)
            {
                /*if(IsPixelPerfect)
                {
                    TargetWidth = 1920;
                    TargetHeight = 1080;
                }
                else
                {*/
                ViewWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                ViewHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                //}


                Window.AllowUserResizing = false;
                GraphicsDeviceManager.IsFullScreen = true;
                //GraphicsDeviceManager.HardwareModeSwitch = false;
                //Window.IsBorderless = true;
            }
            else
            {
                ViewWidth = display_w;
                ViewHeight = display_h;

                Window.AllowUserResizing = true;
                GraphicsDeviceManager.IsFullScreen = false;
            }
#endif

            GraphicsDeviceManager.PreferredBackBufferWidth = ViewWidth;
            GraphicsDeviceManager.PreferredBackBufferHeight = ViewHeight;

            GraphicsDeviceManager.ApplyChanges();

            Content.RootDirectory = @"Content";

            Debug.Log("[Engine]: Content Directory " + Content.RootDirectory + " at");
            Debug.Log("   folder " + ContentDirectory);
            Debug.LogBreak();

#if WIN || MAC || LINUX
            IsMouseVisible = true;
#else
            IsMouseVisible = false;
#endif
            IsFixedTimeStep = false;
            ExitOnEscapeKeypress = true;

            _destinationRender = new RenderTarget2D(GraphicsDevice, 1920, 1080, false, SurfaceFormat.Color, DepthFormat.None, GraphicsDeviceManager.GraphicsDevice.PresentationParameters.MultiSampleCount, RenderTargetUsage.DiscardContents);
        }


            #region WindowEvents

#if !CONSOLE
        protected virtual void OnClientSizeChanged(object sender, EventArgs e)
        {

            Debug.Log("[Engine]: On Client Size Changed");

            if (Window.ClientBounds.Width > 0 && Window.ClientBounds.Height > 0 && !_resizing)
            {
                _resizing = true;

                ViewWidth = Window.ClientBounds.Width;
                ViewHeight = Window.ClientBounds.Height;
                //TargetWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                //TargetHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

                GraphicsDeviceManager.PreferredBackBufferWidth = ViewWidth;
                GraphicsDeviceManager.PreferredBackBufferHeight = ViewHeight;

                UpdateView();

                _resizing = false;
            }
        }
#endif

        protected virtual void OnGraphicsReset(object sender, EventArgs e)
        {

            Debug.Log("[Engine]: On Graphics Reset");

            UpdateView();

            /*if (scene != null)
                scene.HandleGraphicsReset();
            if (nextScene != null && nextScene != scene)
                nextScene.HandleGraphicsReset();*/
        }

        protected virtual void OnGraphicsCreate(object sender, EventArgs e)
        {

            Debug.Log("[Engine]: On Graphics Create");

            UpdateView();

            /*if (scene != null)
                scene.HandleGraphicsCreate();
            if (nextScene != null && nextScene != scene)
                nextScene.HandleGraphicsCreate();*/
        }

        private void UpdateView()
        {
            // Device Width and Height
            float screenWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            float screenHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;

            // Aspect Ratio
            float aspect;
            // Gets the Width/Height (float) proportion

            // Pixel perfect
            // Scale proportion
            var higherScale = (int)Math.Floor(Math.Min(screenWidth / Width, screenHeight / Height));
            // Gets the minimum Width/Height (integer) proportion between native and device

            if (IsPixelPerfect)
            {
                // Get new Target View size 
                ViewWidth = Width * higherScale;
                ViewHeight = Height * higherScale;

                aspect = Height / (float)Width;

                ScreenMatrix = Matrix.CreateScale(higherScale);
            }
            else
            {
                // Get new Target View size
                // Portrait
                if (screenWidth / Width > screenHeight / Height)
                {
                    ViewWidth = (int)(screenHeight / Height * Width);
                    ViewHeight = (int)screenHeight;
                }
                // Landscape
                else
                {
                    ViewWidth = (int)screenWidth;
                    ViewHeight = (int)(screenWidth / Width * Height);
                }

                aspect = ViewHeight / (float)ViewWidth;

                ScreenMatrix = Matrix.CreateScale(ViewWidth / (float)Width);
            }

            /* Using Padding
            // Set ViewPadding when necessary, i.e. XBOX 360 safe-area
            TargetWidth -= ViewPadding * 2;
            TargetHeight -= (int)(aspect * ViewPadding * 2);

            // Update Screen Matrix
            if (IsPixelPerfect) ScreenMatrix = Matrix.CreateScale((int)Math.Floor(TargetWidth / (float)Width));
            else ScreenMatrix = Matrix.CreateScale(TargetWidth / (float)Width);
            */

            Debug.Log("[Engine]: (Real) Width: " + Width + " (Real) Height: " + Height);
            Debug.Log("[Engine]: (Real) Aspect: " + (Height / (float)Width) + " Pixel Scale: " + higherScale);
            Debug.Log("[Engine]: View Width: " + ViewWidth + " View Height: " + ViewHeight);
            Debug.Log("[Engine]: View Aspect: " + (ViewHeight / (float)ViewWidth) + " Matrix Scale: " + (ViewWidth / (float)Width));

            //Console.WriteLine("[Engine]: Window Width: " + Window.ClientBounds.Width + " Window Height: " + Window.ClientBounds.Height);
            Debug.Log("[Engine]: Buffer Width: " + GraphicsDeviceManager.PreferredBackBufferWidth + " Buffer Height: " + GraphicsDeviceManager.PreferredBackBufferHeight);
            Debug.Log("[Engine]: Screen Width: " + screenWidth + " Screen Height: " + screenHeight);
            Debug.LogLine();

            //GraphicsDeviceManager.PreferredBackBufferWidth = TargetWidth;
            //GraphicsDeviceManager.PreferredBackBufferHeight = TargetHeight;

            // If we are using full screen mode, we should check to make sure that the display
            // adapter can handle the video mode we are trying to set.  To do this, we will
            // iterate through the display modes supported by the adapter and check them against
            // the mode we want to set.
            /*foreach (DisplayMode displayMode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
            {
                // Check the width and height of each mode against the passed values
                if (displayMode.Width == _screenWidth && displayMode.Height == _screenHeight)
                {
                    // The mode is supported, so set the buffer formats, apply changes and return
                    _graphicsDevice.PreferredBackBufferWidth = _screenWidth;
                    _graphicsDevice.PreferredBackBufferHeight = _screenHeight;
                    _graphicsDevice.IsFullScreen = _fullScreen;
                    _graphicsDevice.ApplyChanges();
                    break;
                }
            }*/

            // Update Viewport
            Viewport = new Viewport
            {
                X = (int)(screenWidth / 2 - ViewWidth / 2),
                Y = (int)(screenHeight / 2 - ViewHeight / 2),
                Width = ViewWidth,
                Height = ViewHeight,
                MinDepth = 0,
                MaxDepth = 1
            };

            Graphics.Resize(ViewWidth, ViewHeight);
            SceneManager.Camera.Resize(ViewWidth, ViewHeight);
        }

        public static void SetWindowed(int width, int height)
        {
#if !CONSOLE
            if (width > 0 && height > 0)
            {
                _resizing = true;

                ViewWidth = width;
                ViewHeight = height;

                GraphicsDeviceManager.PreferredBackBufferWidth = ViewWidth;
                GraphicsDeviceManager.PreferredBackBufferHeight = ViewHeight;

                GraphicsDeviceManager.IsFullScreen = false;

                GraphicsDeviceManager.ApplyChanges();

                _resizing = false;
            }
#endif
        }

        public static void SetFullscreen()
        {
#if !CONSOLE
            _resizing = true;

            ViewWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            ViewHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            GraphicsDeviceManager.PreferredBackBufferWidth = ViewWidth;
            GraphicsDeviceManager.PreferredBackBufferHeight = ViewHeight;

            GraphicsDeviceManager.IsFullScreen = true;
            //Window.IsBorderless = true;
            //GraphicsDeviceManager.ToggleFullScreen();

            GraphicsDeviceManager.ApplyChanges();

            _resizing = false;
#endif
        }

            #endregion

        protected override void Initialize()
        {
            base.Initialize();

            AudioManager.Initialize();

            Graphics.Initialize(Width, Height);
            Graphics.Resize(ViewWidth, ViewHeight);

            Input.Initialize();

            _Debugger.Initialize();

            Debug.Log("[Engine]: Sucessfully Initialize.");
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            Graphics.LoadContent(GraphicsDevice);

            Debug.Log("[Engine]: Sucessfully Load Content.");
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();

            AudioManager.Unload();

            if (Content != null)
            {
                Content.Unload();
                //Content.Dispose();
                //Content = null;
            }
        }

        // Run after LoadContent and before first Update
        protected override void BeginRun()
        {
            base.BeginRun();

            SceneManager.Begin();

            Input.Enable();

            Debug.Log("[Engine]: Sucessfully BeginRun.");
        }

        protected override void Update(GameTime gameTime)
        {
            RawDeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            DeltaTime = RawDeltaTime * TimeRate;

            //Update input
            Input.Update();
#if !CONSOLE
            if (ExitOnEscapeKeypress && Input.Keyboard.Pressed(Keys.Escape))
            {
                Exit();
                return;
            }
#endif

            // Calculate FPS
            fpsFrameCount++;
            lastFpsUpdateTime += gameTime.ElapsedGameTime;      // ElapsedGameTime gives the time since last update
            if (lastFpsUpdateTime >= TimeSpan.FromSeconds(1))
            {
                Window.Title = Title;

                /*#if DEBUG
                     Window.Title = Title + " " + fpsFrameCount.ToString() + " fps - " + (GC.GetTotalMemory(false) / 1048576f).ToString("F") + " MB";
#endif*/

                FPS = fpsFrameCount;
                fpsFrameCount = 0;
                lastFpsUpdateTime -= TimeSpan.FromSeconds(1);
            }

            // Skip Update
            if (IsFixedTimeStep && _TimeStep > 0)
                _TimeStep = Math.Max(_TimeStep - RawDeltaTime, 0);
            // Update current scene
            else
                SceneManager.Update();

            // Debug Console & Inspector
            _Debugger.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SetRenderTarget(_destinationRender);       // Render null
            GraphicsDevice.Clear(Graphics.ClearColor);  // Clear screen

            // Draw on RenderTarget
            // Notes: Removed because now every Render creates its on SpriteBatch for each Layer.
            //if(IsPixelPerfect)
            //    Graphics.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null,/*Effect,*/ SceneManager.Camera.Matrix * Engine.ScreenMatrix);
            //else
            //    Graphics.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null,/*Effect,*/ SceneManager.Camera.Matrix * Engine.ScreenMatrix);
            SceneManager.Render();      // Render Scene
            //Graphics.SpriteBatch.End();

            // Draw on ViewPort
            GraphicsDevice.SetRenderTarget(null);       // Render null
            GraphicsDevice.Viewport = Viewport;
            //GraphicsDevice.Clear(Graphics.ClearColor);  // Clear screen
            GraphicsDevice.Clear(Graphics.LetterBoxColor);  // Clear screen

            // Draw RenderTarget to screen
            Graphics.SpriteBatch.Begin();
            Graphics.Draw(_destinationRender);
            Graphics.SpriteBatch.End();

            // Debug Console & Inspector
            _Debugger.Render();

            base.Draw(gameTime);
        }

        #region Events

        protected override void OnExiting(object sender, EventArgs args)
        {
            base.OnExiting(sender, args);

            Input.Disable();
        }

        #endregion
    }
}

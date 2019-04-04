using System;
using Caieta.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Caieta
{
    public class DebugInspector
    {
        // Notes: Set this to false to force disable.
        public bool IsEnabled = true;
        public bool IsOpen;
        public bool ShowGrid = true;

        public enum InspectorState { NONE, SCENE, ENTITIES, INPUTS, AUDIO, EXIT };
        private InspectorState _State;

        public DebugInspector()
        {
            IsOpen = false;

            Debug.Log("[DebugInspector]: Inspector complete initialized.");
            Debug.LogLine();
        }

        internal void Initialize()
        {

        }

        internal void Update()
        {
            // Update Inspector
            if (IsOpen)
                UpdateOpen();
            else if (IsEnabled)
                UpdateClosed();
        }

        private void UpdateOpen()
        {
            if (Input.Keyboard.Pressed(Keys.F1))
                _State = InspectorState.SCENE;
            else if (Input.Keyboard.Pressed(Keys.F2))
                _State = InspectorState.ENTITIES;
            else if (Input.Keyboard.Pressed(Keys.F3))
                _State = InspectorState.INPUTS;
            else if (Input.Keyboard.Pressed(Keys.F4))
                _State = InspectorState.AUDIO;
            else if (Input.Keyboard.Pressed(Keys.F5))
                _State = InspectorState.EXIT;

            if (Input.Keyboard.Pressed(Keys.Tab) || _State == InspectorState.EXIT)
            {
                IsOpen = false;
                _State = InspectorState.NONE;
            }
        }

        private void UpdateClosed()
        {
            if (Input.Keyboard.Pressed(Keys.Tab))
            {
                IsOpen = true;
                _State = InspectorState.NONE;
            }
        }

        private int draw_space = 0;

        internal void Render()
        {
            /*
             *      DRAW SCENE
             */
            DrawScene();
            // Draw Layout, Colliders & Sprites

            /*
             *      DRAW MENU
             */
            DrawMenu();
            // Draw Menu with Infos
        }

        private void DrawScene()
        {
            // Start graphics with rendertarget
            if (Engine.IsPixelPerfect)
                Graphics.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Engine.SceneManager.Camera.Matrix * Engine.ScreenMatrix);
            else
                Graphics.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Engine.SceneManager.Camera.Matrix * Engine.ScreenMatrix);

            /*
             *  Grid Box
             */
            if (ShowGrid)
            {
                // Draw Grid
                // Draw Horizontal lines
                for (int x = Engine.SceneManager.CurrScene.TileWidth; x < Engine.SceneManager.CurrScene.Layout.Width; x = x + Engine.SceneManager.CurrScene.TileWidth)
                    Graphics.DrawLine(x, 0, x, Engine.SceneManager.CurrScene.Layout.Height, Color.Gray, 30);

                // Draw Vertical lines
                for (int y = Engine.SceneManager.CurrScene.TileHeight; y < Engine.SceneManager.CurrScene.Layout.Height; y = y + Engine.SceneManager.CurrScene.TileHeight)
                    Graphics.DrawLine(0, y, Engine.SceneManager.CurrScene.Layout.Width, y, Color.Gray, 30);
            }

            /*
             *  Layout Box
             */
            Graphics.DrawRect(Engine.SceneManager.CurrScene.Layout.X, Engine.SceneManager.CurrScene.Layout.Y, Engine.SceneManager.CurrScene.Layout.Width, Engine.SceneManager.CurrScene.Layout.Height, Color.Black, 100, FillType.HOLLOW);

            // End graphics
            Graphics.SpriteBatch.End();


            // Game Objects Debug
            foreach (var layer in Engine.SceneManager.SceneLayers())
            {
                if (layer.IsVisible)
                {
                    // Make Camera obey Layer Parallax
                    Engine.SceneManager.Camera.Parallax = new Vector2(layer.Parallax.X / 100f, layer.Parallax.Y / 100f);
                    // Notes: Uncomment this when position is fixed. Now it displays current position.

                    // Notes: We dont need to do this if current parallax is equal to last layer parallax. Could be a future optimization, but I rather believe It wont change much.
                    // Open Batch for Current Layer
                    if (Engine.IsPixelPerfect)
                        Graphics.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Engine.SceneManager.Camera.Matrix * Engine.ScreenMatrix);
                    else
                        Graphics.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Engine.SceneManager.Camera.Matrix * Engine.ScreenMatrix);

                    // Draw layer Entities
                    foreach (var ent in layer.Entities)
                    {
                        /*
                         * Draw Entity Colliders
                         */
                        foreach (var col in ent.GetAll<Collider>())
                        {
                            // Collider Box
                            Graphics.DrawRect(col.AbsolutePosition.X, col.AbsolutePosition.Y, col.Width, col.Height, Color.Red, 100, FillType.HOLLOW);
                            // Collider Origin
                            Graphics.DrawPoint(col.AbsolutePosition, Color.OrangeRed);
                        }

                        /*
                         *  Draw Sprite
                         */
                        foreach (var sprite in ent.GetAll<Sprite>())
                        {
                            // Sprite Box
                            Graphics.DrawRect(ent.Transform.Position.X - sprite.Origin.X, ent.Transform.Position.Y - sprite.Origin.Y, sprite.Width, sprite.Height, Color.LimeGreen, 20, FillType.FILL);

                            // Sprite Origin
                            Graphics.DrawRect(ent.Transform.Position.X - sprite.Origin.X - 2, ent.Transform.Position.Y - sprite.Origin.Y - 2, 5, 5, Color.LimeGreen, 50);
                            Graphics.DrawPoint(ent.Transform.Position - sprite.Origin, Color.LimeGreen, 50);
                            Graphics.DrawRect(ent.Transform.Position.X - sprite.Origin.X - 2, ent.Transform.Position.Y - sprite.Origin.Y - 2, 5, 5, Color.LimeGreen, 50);
                        }

                        /*
                         *  Draw Entity Transform Position
                         */
                        Graphics.DrawPoint(ent.Transform.Position, Color.Orange);
                        // Draw Angle
                        //Graphics.DrawLineAgle(ent.Transform.Position, ent.Transform.Rotation, 5, Color.Orange);
                        // Draw Name
                        Graphics.DrawText(ent.Name, new Vector2((int)Math.Round(ent.Transform.Position.X), (int)Math.Round(ent.Transform.Position.Y)) + new Vector2(-10, -10), Color.Bisque, FontSize.VERYSMALL);
                    }

                    // Set Camera back to Default
                    Engine.SceneManager.Camera.Parallax = Vector2.One;

                    // Close Batch for Current Layer
                    Graphics.SpriteBatch.End();
                }
            }
        }

        private void DrawMenu()
        {
            int screenWidth = Graphics.ViewWidth;
            int screenHeight = Graphics.ViewHeight;

            // Start a new batch to draw relative to screensize
            Graphics.SpriteBatch.Begin();

            // Game Data
            Graphics.DrawText("FPS: " + Engine.Instance.FPS.ToString(), new Vector2(20, 20), Color.White, FontSize.VERYSMALL);
            Graphics.DrawText("Memory Usage: " + (GC.GetTotalMemory(false) / 1048576f).ToString("F") + " MB", new Vector2(120, 20), Color.White, FontSize.VERYSMALL);
            Graphics.DrawText("==== Inspector ====", new Vector2(20, 40), Color.White, FontSize.MEDIUM);

            // DISPLAY MENU
            Graphics.DrawText("SCENE     [F1]", new Vector2(20, 80), Color.White, FontSize.MEDIUM);
            Graphics.DrawText("ENTITIES  [F2]", new Vector2(20, 100), Color.White, FontSize.MEDIUM);
            Graphics.DrawText("INPUTS    [F3]", new Vector2(20, 120), Color.White, FontSize.MEDIUM);
            Graphics.DrawText("AUDIO     [F4]", new Vector2(20, 140), Color.White, FontSize.MEDIUM);

            Graphics.DrawText("EXIT      [F5 or Tab]", new Vector2(20, 180), Color.White, FontSize.MEDIUM);

            /*
                     * Inspector
                     * 
                     * Game Data
                     * 
                     * Scene      [F1]
                     * Entities   [F2]
                     * 
                     * Inputs     [F3]
                     * 
                     * Exit       [F5 or Tab]
                     */
                        // Display Right Side detailed info
                        switch (_State)
            {
                case InspectorState.SCENE:
                    // DISPLAY SCENE            (RIGHT SIDE)
                    Graphics.DrawText(">", new Vector2(10, 80), Color.White, FontSize.MEDIUM);
                    // Scene
                    Graphics.DrawText("SCENE", new Vector2(2 * screenWidth / 3, 80), Color.White, FontSize.MEDIUM);
                    Graphics.DrawText("  " + Engine.SceneManager.SceneName(), new Vector2(2 * screenWidth / 3, 100), Color.White);

                    // Layers
                    Graphics.DrawText("LAYERS ", new Vector2(2 * screenWidth / 3, 140), Color.White, FontSize.MEDIUM);

                    draw_space = 0;
                    foreach (var layer in Engine.SceneManager.SceneLayers())
                    {
                        if (layer.IsGlobal)
                            Graphics.DrawText("  " + layer.Name + " [GLOBAL]", new Vector2(2 * screenWidth / 3, 160 + (20 * draw_space)), Color.White);
                        else
                            Graphics.DrawText("  " + layer.Name, new Vector2(2 * screenWidth / 3, 160 + (20 * draw_space)), Color.White);

                        draw_space++;
                    }

                    break;

                case InspectorState.ENTITIES:
                    // DISPLAY ENTITIES         (RIGHT SIDE)
                    Graphics.DrawText(">", new Vector2(10, 100), Color.White, FontSize.MEDIUM);
                    // Entities
                    Graphics.DrawText("ENTITIES ", new Vector2(2 * screenWidth / 3, 80), Color.White, FontSize.MEDIUM);
                    Graphics.DrawText("(" + Engine.SceneManager.ScenePopulation() + ")", new Vector2((2 * screenWidth / 3) + 150, 80), Color.White, FontSize.MEDIUM);

                    // Open Entities
                    draw_space = 0;
                    foreach (var layer in Engine.SceneManager.SceneLayers())
                    {
                        foreach (var ent in layer.Entities)
                        {
                            // Draw Entity Name List
                            Graphics.DrawText("  " + ent.Name, new Vector2(2 * screenWidth / 3, 100 + (20 * draw_space)), Color.White);

                            // Draw Movement info if any
                            if (ent.Has<Platform>())
                            {
                                Graphics.DrawText("Vel: " + ent.Get<Platform>().Velocity.X + " (X) " + ent.Get<Platform>().Velocity.Y + " (Y) "
                                , new Vector2((2 * screenWidth / 3) - 90, 100 + (20 * draw_space)), Color.White, FontSize.VERYSMALL);

                                Graphics.DrawText("IsMoving   " + ent.Get<Platform>().IsMoving, new Vector2((2 * screenWidth / 3) - 90, 100 + (20 * draw_space) + 20), Color.White, FontSize.VERYSMALL);
                                Graphics.DrawText("IsJumping  " + ent.Get<Platform>().IsJumping, new Vector2((2 * screenWidth / 3) - 90, 100 + (20 * draw_space) + 35), Color.White, FontSize.VERYSMALL);
                                Graphics.DrawText("IsFalling  " + ent.Get<Platform>().IsFalling, new Vector2((2 * screenWidth / 3) - 90, 100 + (20 * draw_space) + 50), Color.White, FontSize.VERYSMALL);
                                Graphics.DrawText("IsOnFloor  " + ent.Get<Platform>().IsOnFloor, new Vector2((2 * screenWidth / 3) - 90, 100 + (20 * draw_space) + 65), Color.White, FontSize.VERYSMALL);
                                Graphics.DrawText("Wall Left  " + ent.Get<Platform>().IsByWall["Left"], new Vector2((2 * screenWidth / 3) - 90, 100 + (20 * draw_space) + 80), Color.White, FontSize.VERYSMALL);
                                Graphics.DrawText("Wall Right " + ent.Get<Platform>().IsByWall["Right"], new Vector2((2 * screenWidth / 3) - 90, 100 + (20 * draw_space) + 95), Color.White, FontSize.VERYSMALL);

                                Graphics.DrawText("(World) Pos: " + ent.Transform.Position.X + " (X) " + ent.Transform.Position.Y + " (Y) "
                                , new Vector2((2 * screenWidth / 3) - 90, 100 + (20 * draw_space) + 120), Color.White, FontSize.VERYSMALL);
                                Graphics.DrawText("(Screen) Pos: " + ent.Transform.ScreenPosition.X + " (X) " + ent.Transform.ScreenPosition.Y + " (Y) "
                                , new Vector2((2 * screenWidth / 3) - 90, 100 + (20 * draw_space) + 135), Color.White, FontSize.VERYSMALL);
                            }
                        }

                        draw_space++;
                    }
                    break;

                case InspectorState.INPUTS:
                    // DISPLAY INPUTS           (RIGHT SIDE)
                    Graphics.DrawText(">", new Vector2(10, 120), Color.White, FontSize.MEDIUM);
                    // Inputs
                    Graphics.DrawText("INPUTS ", new Vector2(2 * screenWidth / 3, 80), Color.White, FontSize.MEDIUM);
                    Graphics.DrawText("Key Hold      " + Input.Keyboard.GetHoldKey(), new Vector2(screenWidth / 2, 120), Color.White);
                    Graphics.DrawText("Key Pressed   " + Input.Keyboard.GetPressedKey(), new Vector2(screenWidth / 2, 140), Color.White);
                    Graphics.DrawText("Key Released  " + Input.Keyboard.GetReleasedKey(), new Vector2(screenWidth / 2, 160), Color.White);
                    Graphics.DrawText("Direction     " + Input.Keyboard.GetDirection(), new Vector2(3 * screenWidth / 4, 140), Color.White);
                    Graphics.DrawText("Key Modifier  " + Input.Keyboard.GetModifierKey(), new Vector2(3 * screenWidth / 4, 160), Color.White);

                    Graphics.DrawText("Mouse Hold      " + Input.Touch.IsMouseHold(MouseButton.Left), new Vector2(screenWidth / 2, 200), Color.White);
                    Graphics.DrawText("Mouse Pressed   " + Input.Touch.IsMousePressed(MouseButton.Left), new Vector2(screenWidth / 2, 220), Color.White);
                    Graphics.DrawText("Mouse Released  " + Input.Touch.IsMouseReleased(MouseButton.Left), new Vector2(screenWidth / 2, 240), Color.White);
                    Graphics.DrawText("Mouse Pos  " + Input.Touch.Position, new Vector2(3 * screenWidth / 4, 200), Color.White);
                    Graphics.DrawText("IsMoving   " + Input.Touch.IsMoving, new Vector2(3 * screenWidth / 4, 220), Color.White);

                    draw_space = 0;
                    foreach (var gamepad in Input.GamePads)
                    {
                        if (gamepad.IsAttached)
                            draw_space++;
                    }

                    Graphics.DrawText("GamePads Connected   " + draw_space, new Vector2(screenWidth / 2, 280), Color.White);

                    draw_space = 0;
                    foreach (var gamepad in Input.GamePads)
                    {
                        if (gamepad.IsAttached)
                        {
                            int startX = 20 + ((screenWidth / 4) * draw_space);
                            int startY = 240;
                            Graphics.DrawText("GamePad " + draw_space, new Vector2(startX, startY - 20), Color.White);

                            Graphics.DrawText("GamePad Hold  " + gamepad.GetHoldButton(), new Vector2(startX, startY), Color.White);
                            Graphics.DrawText("GamePad Pressed   " + gamepad.GetPressedButton(), new Vector2(startX, startY + 20), Color.White);
                            Graphics.DrawText("GamePad Released  " + gamepad.GetReleasedButton(), new Vector2(startX, startY + 40), Color.White);
                            Graphics.DrawText("DPad Direction        ", new Vector2(startX, startY + 60), Color.White);
                            Graphics.DrawText("Left Axis Direction  ", new Vector2(startX, startY + 80), Color.White);
                            Graphics.DrawText("Right Axis Direction  ", new Vector2(startX, startY + 100), Color.White);
                            Graphics.DrawText("LT Pressure  ", new Vector2(startX, startY + 120), Color.White);
                            Graphics.DrawText("RT Pressure  ", new Vector2(startX, startY + 140), Color.White);
                        }
                        draw_space++;
                    }

                    break;

                case InspectorState.AUDIO:
                    // DISPLAY AUDIO           (RIGHT SIDE)
                    Graphics.DrawText(">", new Vector2(10, 140), Color.White, FontSize.MEDIUM);

                    // Audio
                    Graphics.DrawText("AUDIO", new Vector2(2 * screenWidth / 3, 80), Color.White, FontSize.MEDIUM);
                    Graphics.DrawText("Master Volume: " + AudioManager.MasterVolume, new Vector2(2 * screenWidth / 3, 100), Color.White);

                    // Music
                    Graphics.DrawText("MUSIC", new Vector2(2 * screenWidth / 3, 140), Color.White, FontSize.MEDIUM);
                    Graphics.DrawText("Volume: " + AudioManager.Music.Volume, new Vector2(2 * screenWidth / 3, 160), Color.White);

                    if (AudioManager.Music.CurrMusic != null)
                    {
                        Graphics.DrawText(" " + AudioManager.Music.CurrMusic.Name, new Vector2(2 * screenWidth / 3, 180), Color.White);
                        Graphics.DrawText("Duration: " + Calc.GetHumanReadableTime(AudioManager.Music.CurrMusic.Position) + " / " + Calc.GetHumanReadableTime(AudioManager.Music.CurrMusic.Duration), new Vector2(2 * screenWidth / 3, 200), Color.White);
                    }
                    else
                    {
                        Graphics.DrawText(" Not Playing ", new Vector2(2 * screenWidth / 3, 180), Color.White);
                        Graphics.DrawText("Duration: 00:00 / 00:00", new Vector2(2 * screenWidth / 3, 200), Color.White);
                    }

                    // Sound Effects
                    Graphics.DrawText("SFX", new Vector2(2 * screenWidth / 3, 240), Color.White, FontSize.MEDIUM);
                    Graphics.DrawText("Volume: " + AudioManager.SFX.Volume, new Vector2(2 * screenWidth / 3, 260), Color.White);

                    draw_space = 0;
                    foreach (var sounds in AudioManager.SFX.SFXs)
                    {
                        Graphics.DrawText("  " + sounds.Key, new Vector2(2 * screenWidth / 3, 280 + (20 * draw_space)), Color.White);
                        //Graphics.DrawText("  " + sounds.Value, new Vector2(2 * screenWidth / 3, 280 + (40 * draw_space)), Color.White);

                        draw_space++;
                    }

                    break;
            }

            Graphics.SpriteBatch.End();
        }
    }
}

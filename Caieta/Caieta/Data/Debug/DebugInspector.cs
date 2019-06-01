using System;
using Caieta.Audio;
using Caieta.Components.Renderables.Sprites;
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

        [Flags] enum InspectorState { NONE = 0, SCENE = 1, ENTITIES = 2, INPUTS = 4, AUDIO = 8 };
        private InspectorState _State;

        public DebugInspector()
        {
            IsOpen = false;

            //Debug.Log("[DebugInspector]: Inspector complete initialized.");
            //Debug.LogLine();
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
                _State ^= InspectorState.SCENE;            // XOR Toggle Values
            else if (Input.Keyboard.Pressed(Keys.F2))
                _State ^= InspectorState.ENTITIES;
            else if (Input.Keyboard.Pressed(Keys.F3))
                _State ^= InspectorState.INPUTS;
            else if (Input.Keyboard.Pressed(Keys.F4))
                _State ^= InspectorState.AUDIO;
            else if (Input.Keyboard.Pressed(Keys.F6))
                _State = InspectorState.NONE;

            if (Input.Keyboard.Pressed(Keys.Tab) || Input.Keyboard.Pressed(Keys.F5))
                IsOpen = false;
        }

        private void UpdateClosed()
        {
            if (Input.Keyboard.Pressed(Keys.Tab))
            {
                IsOpen = true;
                _State = _State | InspectorState.SCENE | InspectorState.ENTITIES | InspectorState.INPUTS | InspectorState.AUDIO;
            }
        }

        private int draw_space = 0;

        internal void Render()
        {
            // Draw Layout, Colliders & Sprites.    Render to Viewport
            DrawScene();

            // Draw Menu with Infos.                Render to Screen
            DrawMenu();
        }

        private void DrawScene()
        {
            // Start graphics with rendertarget
            if (Engine.IsPixelPerfect)
                Graphics.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Engine.SceneManager.Camera.Matrix * Engine.ScreenMatrix);
            else
                Graphics.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Engine.SceneManager.Camera.Matrix * Engine.ScreenMatrix);

            /*
             *  Draw Grid Box
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
             *  Draw Layout Box
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
                    if (layer.IsGlobal && Engine.SceneManager.CheckGlobal(layer.Name))
                        Engine.SceneManager.Camera.Parallax = new Vector2(Engine.SceneManager.GetGlobal(layer.Name).Parallax.X / 100f, Engine.SceneManager.GetGlobal(layer.Name).Parallax.Y / 100f);
                    else
                        Engine.SceneManager.Camera.Parallax = new Vector2(layer.Parallax.X / 100f, layer.Parallax.Y / 100f);
                    // Notes: Uncomment this when position is fixed. Now it displays current position.

                    // Notes: We dont need to do this if current parallax is equal to last layer parallax. Could be a future optimization, but I rather believe It wont change much.
                    // Open Batch for Current Layer
                    if (Engine.IsPixelPerfect)
                        Graphics.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Engine.SceneManager.Camera.Matrix * Engine.ScreenMatrix);
                    else
                        Graphics.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Engine.SceneManager.Camera.Matrix * Engine.ScreenMatrix);

                    // Draw layer Entities
                    foreach (var ent in (layer.IsGlobal && Engine.SceneManager.CheckGlobal(layer.Name)) ? Engine.SceneManager.GetGlobal(layer.Name).Entities : layer.Entities)
                    {
                        /*
                         * Draw Platform Raycast
                         */
                        Platform platform = ent.Get<Platform>();
                        if (platform != null)
                        {
                            Graphics.DrawLine(ent.Transform.Position, platform.LeftRay, Color.White);
                            Graphics.DrawLine(ent.Transform.Position, platform.RightRay, Color.White);
                        }

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
                            Graphics.DrawRect(ent.Transform.Position.X - sprite.Origin.X, ent.Transform.Position.Y - sprite.Origin.Y, sprite.Width * ent.Transform.Scale.X, sprite.Height * ent.Transform.Scale.Y, Color.LimeGreen, 20, FillType.FILL);

                            // Sprite Origin
                            Graphics.DrawRect(ent.Transform.Position.X - sprite.Origin.X - 2, ent.Transform.Position.Y - sprite.Origin.Y - 2, 5, 5, Color.LimeGreen, 50);
                            Graphics.DrawPoint(ent.Transform.Position - sprite.Origin, Color.LimeGreen, 50);
                            Graphics.DrawRect(ent.Transform.Position.X - sprite.Origin.X - 2, ent.Transform.Position.Y - sprite.Origin.Y - 2, 5, 5, Color.LimeGreen, 50);
                        }

                        /*
                         *  Draw TiledSprite
                         */
                        foreach (var sprite in ent.GetAll<TiledSprite>())
                        {
                            // Sprite Box
                            Graphics.DrawRect(ent.Transform.Position.X - sprite.Origin.X, ent.Transform.Position.Y - sprite.Origin.Y, sprite.RegionWidth * ent.Transform.Scale.X, sprite.RegionHeight * ent.Transform.Scale.Y, Color.LimeGreen, 20, FillType.FILL);

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
            Graphics.DrawText("FPS: " + Engine.Instance.FPS.ToString(), new Vector2(20, 20), Color.White, FontSize.SMALL);
            Graphics.DrawText("Memory Usage: " + (GC.GetTotalMemory(false) / 1048576f).ToString("F") + " MB", new Vector2(90, 20), Color.White, FontSize.SMALL);
            Graphics.DrawText("[Press F5 or TAB to Close]", new Vector2(screenWidth - 180, 20), Color.White, FontSize.SMALL);

            // Draw Audio
            if ((_State & InspectorState.AUDIO) == InspectorState.AUDIO)
            {
                Graphics.DrawText("Volume (Master: " + AudioManager.MasterVolume + " Music: " + AudioManager.Music.Volume +
                                  " SFX: " + AudioManager.SFX.Volume + ")"
                                   , new Vector2(20, 50), Color.White, FontSize.SMALL);
                // Music
                if (AudioManager.Music.CurrMusic != null)
                {
                    Graphics.DrawText(" " + AudioManager.Music.CurrMusic.Name, new Vector2(20, 60), Color.White, FontSize.SMALL);
                    Graphics.DrawText("Duration: " + Calc.GetHumanReadableTime(AudioManager.Music.CurrMusic.Position) + " / " + Calc.GetHumanReadableTime(AudioManager.Music.CurrMusic.Duration), new Vector2(20, 70), Color.White, FontSize.SMALL);
                }
                else
                {
                    Graphics.DrawText("Not Playing ", new Vector2(20, 60), Color.White, FontSize.SMALL);
                    Graphics.DrawText("Duration: 00:00 / 00:00", new Vector2(20, 70), Color.White, FontSize.SMALL);
                }
            }

            /*
             *      Display detailed info
             */
            // Draw Scene
            if ((_State & InspectorState.SCENE) == InspectorState.SCENE)
            {
                Graphics.DrawText("SCENE", new Vector2(3 * screenWidth / 4, 40), Color.White, FontSize.MEDIUM);
                Graphics.DrawText(" " + Engine.SceneManager.SceneName(), new Vector2(3 * screenWidth / 4, 60), Color.White, FontSize.MEDIUM);

                // Draw Layers
                Graphics.DrawText("LAYERS ", new Vector2(3 * screenWidth / 4, 80), Color.White, FontSize.MEDIUM);

                draw_space = 0;
                foreach (var layer in Engine.SceneManager.SceneLayers())
                {
                    if (layer.IsGlobal)
                        Graphics.DrawText(" " + layer.Name + "(" + Engine.SceneManager.GetGlobal(layer.Name).Population() + ")" + " [GLOBAL]", new Vector2(3 * screenWidth / 4, 100 + (20 * draw_space)), Color.White, FontSize.MEDIUM);
                    else
                        Graphics.DrawText(" " + layer.Name + "(" + layer.Population() + ")", new Vector2(3 * screenWidth / 4, 100 + (20 * draw_space)), Color.White, FontSize.MEDIUM);

                    // Draw Entities
                    //if ((_State & InspectorState.ENTITIES) == InspectorState.ENTITIES)
                    //{
                    //    foreach (var ent in (layer.IsGlobal && Engine.SceneManager.CheckGlobal(layer.Name)) ? Engine.SceneManager.GetGlobal(layer.Name).Entities : layer.Entities)
                    //    {
                    //        // Draw Entity Name List
                    //        Graphics.DrawText("  " + ent.Name, new Vector2(3 * screenWidth / 4, 120 + (20 * draw_space)), Color.White, FontSize.SMALL);
                           
                    //        // Draw Entity Position
                    //        Graphics.DrawText("(World)  " + (int)ent.Transform.Position.X + " X " + (int)ent.Transform.Position.Y + " Y "
                    //            , new Vector2(screenWidth - 130, 120 + (20 * draw_space)), Color.White, FontSize.VERYSMALL);
                    //        Graphics.DrawText("(Screen) " + (int)ent.Transform.ScreenPosition.X + " X " + (int)ent.Transform.ScreenPosition.Y + " Y "
                    //            , new Vector2(screenWidth - 130, 130 + (20 * draw_space)), Color.White, FontSize.VERYSMALL);

                    //        draw_space++;

                    //        Platform platform = ent.Get<Platform>();
                    //        if (platform != null)
                    //        {
                    //            Graphics.DrawText("Vel   X: " + platform.Velocity.X + " Y: " + platform.Velocity.Y, new Vector2(250, 70), Color.White, FontSize.VERYSMALL);
                    //            Graphics.DrawText("Dir   X: " + platform.MoveDirection.X + " Y: " + platform.MoveDirection.Y, new Vector2(250, 80), Color.White, FontSize.VERYSMALL);
                    //            Graphics.DrawText("Force X: " + platform.Force.X + " Y: " + platform.Force.Y, new Vector2(250, 90), Color.White, FontSize.VERYSMALL);
                    //            Graphics.DrawText("Moving: " + platform.IsMoving + " On Floor: " + platform.IsOnFloor, new Vector2(250, 100), Color.White, FontSize.VERYSMALL);
                    //            Graphics.DrawText("Jumping: " + platform.IsJumping + " Falling: " + platform.IsFalling + " Jump Count: " + platform.JumpCount, new Vector2(250, 110), Color.White, FontSize.VERYSMALL);
                    //            Graphics.DrawText("Sliding: " + platform.IsSliding + " Left Wall: " + platform.IsByWallLeft + " Right Wall: " + platform.IsByWallRight, new Vector2(250, 120), Color.White, FontSize.VERYSMALL);
                    //        }
                    //    }
                    //}
                    draw_space++;
                }
            }

            // Draw Inputs
            if ((_State & InspectorState.INPUTS) == InspectorState.INPUTS)
            {
                Graphics.DrawText("INPUTS ", new Vector2(20, 110), Color.White);
                Graphics.DrawText("Mouse Pos    " + (int)Input.Touch.Position.X + " X " + (int)Input.Touch.Position.Y + " Y", new Vector2(20, 120), Color.White);
                Graphics.DrawText("Key Direction " + Input.Keyboard.GetDirection(), new Vector2(20, 130), Color.White);
                Graphics.DrawText("Key Modifier  " + Input.Keyboard.GetModifierKey(), new Vector2(20, 140), Color.White);

                draw_space = 0;
                foreach (var gamepad in Input.GamePads)
                {
                    if (gamepad.IsAttached)
                        draw_space++;
                }
                Graphics.DrawText("GamePads Connected   " + draw_space, new Vector2(20, 170), Color.White);

                draw_space = 0;
                foreach (var gamepad in Input.GamePads)
                {
                    if (gamepad.IsAttached)
                    {
                        int startX = 260 + ((screenWidth / 5) * draw_space);//20 + ((screenWidth / 4) * draw_space);
                        int startY = screenHeight - 75;
                        Graphics.DrawText("C"+ draw_space + ".", new Vector2(startX, startY + 15), Color.White);
                        Graphics.DrawText("   Button Hold " + gamepad.GetHoldButton(), new Vector2(startX, startY+15), Color.White);
                        Graphics.DrawText("   DPad   " + Input.GamePads[draw_space].DPadDirection(), new Vector2(startX, startY + 30), Color.White);
                        Graphics.DrawText("   L Axis " + Input.GamePads[draw_space].LeftStickDirection(), new Vector2(startX, startY + 45), Color.White);
                        Graphics.DrawText("   R Axis " + Input.GamePads[draw_space].RightStickDirection(), new Vector2(startX, startY + 60), Color.White);
                        Graphics.DrawText("   LT " + Input.GamePads[draw_space].LeftTrigger().ToString("F"), new Vector2(startX + 110, startY + 45), Color.White);
                        Graphics.DrawText("   RT " + Input.GamePads[draw_space].RightTrigger().ToString("F"), new Vector2(startX + 110, startY + 60), Color.White);
                    }
                    draw_space++;
                }
            }

            Graphics.SpriteBatch.End();
        }
    }
}

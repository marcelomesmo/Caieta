using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Caieta
{
    public class DebugInspector
    {
        public bool IsEnabled = true;
        public bool IsOpen;

        private Keyboard Keyboard;
        private Touch Touch;
        private GamePad[] GamePads;

        public enum InspectorState { NONE, SCENE, ENTITIES, INPUTS, EXIT };
        private InspectorState _State;

        public DebugInspector()
        {
            IsOpen = false;

            Debug.Log("[DebugInspector]: Inspector complete initialized.");
            Debug.LogLine();
        }

        internal void Initialize()
        {
            Keyboard = new Keyboard();
            Touch = new Touch();

            GamePads = new GamePad[Input.MAX_SUPPORTED_GAMEPADS];
            for (int i = 0; i < Input.MAX_SUPPORTED_GAMEPADS; i++)
                if(Input.GamePads[i] != null) 
                    GamePads[i] = Input.GamePads[i];
        }

        internal void Update()
        {
            // Update Inputs
            Keyboard.Update();
            Touch.Update();
            for (int i = 0; i < Input.MAX_SUPPORTED_GAMEPADS; i++)
                GamePads[i].Update();

            // Update Inspector
            if (IsOpen)
                UpdateOpen();
            else if (IsEnabled)
                UpdateClosed();
        }

        private void UpdateOpen()
        {
            if (Keyboard.Pressed(Keys.F1))
                _State = InspectorState.SCENE;
            else if (Keyboard.Pressed(Keys.F2))
                _State = InspectorState.ENTITIES;
            else if (Keyboard.Pressed(Keys.F3))
                _State = InspectorState.INPUTS;
            /*else if (Keyboard.Pressed(Keys.F4))   // Reserved Command for later use
                _State = InspectorState.RESERVED;*/
            else if (Keyboard.Pressed(Keys.F5))
                _State = InspectorState.EXIT;

            if (Keyboard.Pressed(Keys.Tab) || _State == InspectorState.EXIT)
            {
                IsOpen = false;
                _State = InspectorState.NONE;
                Input.Enable();
            }
        }

        private void UpdateClosed()
        {
            if (Keyboard.Pressed(Keys.Tab))
            {
                IsOpen = true;
                _State = InspectorState.NONE;
                Input.Disable();
            }
        }

        private int draw_space = 0;

        internal void Render()
        {
            int screenWidth = Graphics.ViewWidth;
            int screenHeight = Graphics.ViewHeight;

            Graphics.SpriteBatch.Begin();

            // Game Data
            Graphics.DrawText("FPS: " + Engine.Instance.FPS.ToString(), new Vector2(20, 20), Color.White, FontSize.VERYSMALL);
            Graphics.DrawText("Memory Usage: " + (GC.GetTotalMemory(false) / 1048576f).ToString("F") + " MB", new Vector2(120, 20), Color.White, FontSize.VERYSMALL);
            Graphics.DrawText("==== Inspector ====", new Vector2(20, 40), Color.White, FontSize.MEDIUM);

            // DISPLAY MENU
            Graphics.DrawText("SCENE     [F1]", new Vector2(20, 80), Color.White, FontSize.MEDIUM);
            Graphics.DrawText("ENTITIES  [F2]", new Vector2(20, 100), Color.White, FontSize.MEDIUM);

            Graphics.DrawText("INPUTS    [F3]", new Vector2(20, 140), Color.White, FontSize.MEDIUM);

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
                            Graphics.DrawText("  " + layer.Name + " [GLOBAL]", new Vector2(screenWidth - 20, 160 + (20 * draw_space)), Color.White);
                        else
                            Graphics.DrawText("  " + layer.Name, new Vector2(screenWidth - 20, 160 + (20 * draw_space)), Color.White);

                        draw_space++;
                    }

                    break;

                case InspectorState.ENTITIES:
                    // DISPLAY ENTITIES         (RIGHT SIDE)
                    Graphics.DrawText(">", new Vector2(10, 100), Color.White, FontSize.MEDIUM);
                    // Entities
                    Graphics.DrawText("ENTITIES ", new Vector2(2 * screenWidth / 3, 80), Color.White, FontSize.MEDIUM);
                    Graphics.DrawText("(" + Engine.SceneManager.ScenePopulation() + ")", new Vector2((2 * screenWidth / 3) +150, 80), Color.White, FontSize.MEDIUM);

                    // Open Entities
                    /*draw_space = 0;
                    foreach (var ent in Engine.SceneManager.CurrScene().Entities())
                    {
                        Graphics.DrawText("  " + ent.Name, new Vector2(screenWidth - 20, 80 + (20*draw_space)), Color.White);
                    }*/
                    break;

                case InspectorState.INPUTS:
                    // DISPLAY INPUTS           (RIGHT SIDE)
                    Graphics.DrawText(">", new Vector2(10, 140), Color.White, FontSize.MEDIUM);
                    // Inputs
                    Graphics.DrawText("INPUTS ", new Vector2(2 * screenWidth / 3, 80), Color.White, FontSize.MEDIUM);
                    Graphics.DrawText("Key Hold      " + Keyboard.GetHoldKey(), new Vector2(screenWidth / 2, 120), Color.White);
                    Graphics.DrawText("Key Pressed   " + Keyboard.GetPressedKey(), new Vector2(screenWidth / 2, 140), Color.White);
                    Graphics.DrawText("Key Released  " + Keyboard.GetReleasedKey(), new Vector2(screenWidth / 2, 160), Color.White);
                    Graphics.DrawText("Direction     " + Keyboard.GetDirection(), new Vector2(3 * screenWidth / 4, 140), Color.White);
                    Graphics.DrawText("Key Modifier  " + Keyboard.GetModifierKey(), new Vector2(3 * screenWidth / 4, 160), Color.White);

                    draw_space = 0;
                    foreach (var gamepad in GamePads)
                    {
                        if (gamepad.IsAttached)
                            draw_space++;
                    }

                    Graphics.DrawText("GamePads Connected   " + draw_space, new Vector2(screenWidth / 2, 200), Color.White);

                    draw_space = 0; 
                    foreach (var gamepad in GamePads)
                    {
                        if(gamepad.IsAttached)
                        {
                            Graphics.DrawText("GamePad " + draw_space, new Vector2(screenWidth / 2, 220 + (200 * draw_space)), Color.White);

                            Graphics.DrawText("GamePad Hold  " + gamepad.GetHoldButton(), new Vector2(screenWidth / 2, 240 + (200 * draw_space)), Color.White);
                            Graphics.DrawText("GamePad Pressed   " + gamepad.GetPressedButton(), new Vector2(screenWidth / 2, 260 + (200 * draw_space)), Color.White);
                            Graphics.DrawText("GamePad Released  " + gamepad.GetReleasedButton(), new Vector2(screenWidth / 2, 280 + (20 * draw_space)), Color.White);
                            Graphics.DrawText("DPad Direction        ", new Vector2(screenWidth / 2, 300 + (200 * draw_space)), Color.White);
                            Graphics.DrawText("Left Axis Direction  ", new Vector2(2 * screenWidth / 3, 240 + (200 * draw_space)), Color.White);
                            Graphics.DrawText("Right Axis Direction  ", new Vector2(2 * screenWidth / 3, 260 + (200 * draw_space)), Color.White);
                            Graphics.DrawText("Left Trigger Pressure  ", new Vector2(2 * screenWidth / 3, 280 + (200 * draw_space)), Color.White);
                            Graphics.DrawText("Right Trigger Pressure  ", new Vector2(2 * screenWidth / 3, 300 + (200 * draw_space)), Color.White);
                        }
                        draw_space++;
                    }

                    break;
            }

            Graphics.End();
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using Caieta.Audio;
using Caieta.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Caieta
{
    public class SceneManager
    {
        public Camera Camera;

        private Dictionary<string, Scene> _SceneList;

        public Scene CurrScene { get; private set; }

        private Scene _NextScene;
        private Dictionary<string, Layer> _GlobalLayers;

        public string FirstScene { get; private set; }

        public SceneManager(Dictionary<string, Scene> scenes, string firstScene)
        {
            // Init Scene List
            _SceneList = scenes;
            // Init Scenes
            foreach (var _pair in _SceneList)
            {
                var _scene = _pair.Value;
                _scene.Name = _pair.Key;
            }

            _GlobalLayers = new Dictionary<string, Layer>();

            FirstScene = firstScene;

            // Check First Scene
            if (!_SceneList.ContainsKey(FirstScene))
                throw new ArgumentException("[SceneManager]: First scene '" + FirstScene + "'. Name invalid or not declared.");
            else CurrScene = _NextScene = _SceneList[FirstScene];

            // Game Camera
            Camera = new Camera();

            Debug.LogLine();

            // Debug initial entities
            // Notes: Make this into Console Debug later.
            // Debug.Log("[SceneManager]: Scene '" + _CurrScene.Name + "' has total: " + _CurrScene.Population() + " entities created.");
        }

        internal void Begin()
        {
            // Start First Scene
            if (CurrScene != null)
            {
                CurrScene.Awake();
                CurrScene.Start();
            }
        }

        internal void Update()
        {
            // Update Scene
            if (CurrScene != null)
            {
                CurrScene.Update();

                // Update Layers
                foreach (var _CurrLayer in CurrScene.Layers.Values)
                {
                    // Layer movement obey Camera Parallax
                    Engine.SceneManager.Camera.Parallax = new Vector2(_CurrLayer.Parallax.X / 100f, _CurrLayer.Parallax.Y / 100f);

                    if (_CurrLayer.IsGlobal)
                    {
                        if (!_GlobalLayers.ContainsKey(_CurrLayer.Name))
                            Debug.ErrorLog("[SceneManager]: Global layer '" + _CurrLayer.Name + "' not found.");
                        else
                        {
                            // Update Global Layer
                            _GlobalLayers[_CurrLayer.Name].AwakeEntities();
                            _GlobalLayers[_CurrLayer.Name].Update();
                        }
                    }
                    else
                    {
                        // Update Local Layer
                        _CurrLayer.AwakeEntities();
                        _CurrLayer.Update();
                    }

                    // Set Camera back to Default
                    Engine.SceneManager.Camera.Parallax = Vector2.One;
                }

                CurrScene.LateUpdate();
            }

            // Change Scenes
            if (CurrScene != _NextScene)
            {
                if (CurrScene != null)
                    CurrScene.End();

                CurrScene = _NextScene;

                OnSceneTransition();

                if (CurrScene != null)
                {
                    CurrScene.Awake();
                    CurrScene.Start();
                }
            }
        }

        internal void Render()
        {
            // Render Scene
            if (CurrScene != null)
            {
                // Render Layers
                foreach (var _CurrLayer in CurrScene.Layers.Values)
                {
                    if (!_CurrLayer.IsVisible)
                        continue;

                    if (_CurrLayer.IsGlobal && _GlobalLayers.ContainsKey(_CurrLayer.Name))
                        if (!_GlobalLayers[_CurrLayer.Name].IsVisible)
                            continue;

                    // Layer rendering obey Camera Parallax
                    if (_CurrLayer.IsGlobal)
                    {
                        if (!_GlobalLayers.ContainsKey(_CurrLayer.Name))
                            Debug.ErrorLog("[SceneManager]: Trying to get Parallax from invalid Global layer '" + _CurrLayer.Name + "' from within Scene '" + CurrScene.Name + "'.");
                        else
                            Engine.SceneManager.Camera.Parallax = new Vector2(_GlobalLayers[_CurrLayer.Name].Parallax.X / 100f, _GlobalLayers[_CurrLayer.Name].Parallax.Y / 100f);
                    }
                    else
                        Engine.SceneManager.Camera.Parallax = new Vector2(_CurrLayer.Parallax.X / 100f, _CurrLayer.Parallax.Y / 100f);

                    // Notes: We dont need to do this if current parallax is equal to last layer parallax. Could be a future optimization, but I rather believe It wont change much.
                    // Open Batch for Current Layer
                    if (Engine.IsPixelPerfect)
                        Graphics.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null,/*Effect,*/ Engine.SceneManager.Camera.Matrix * Engine.ScreenMatrix);
                    else
                        Graphics.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null,/*Effect,*/ Engine.SceneManager.Camera.Matrix * Engine.ScreenMatrix);

                    // Render Global Layer
                    if (_CurrLayer.IsGlobal)
                    {
                        if (!_GlobalLayers.ContainsKey(_CurrLayer.Name))
                            Debug.ErrorLog("[SceneManager]: Trying to Render invalid Global layer '" + _CurrLayer.Name + "' from within Scene '" + CurrScene.Name + "'.");
                        else
                            _GlobalLayers[_CurrLayer.Name].Render();
                    }
                    // Render Local Layer
                    else
                        _CurrLayer.Render();

                    // Set Camera back to Default
                    Engine.SceneManager.Camera.Parallax = Vector2.One;

                    // Close Batch for Current Layer
                    Graphics.SpriteBatch.End();
                }

                // Render Scene on top of everything (?)
                // Notes: Removed because every Render is now managed by the Layer.
                //Graphics.SpriteBatch.Begin();
                //CurrScene.Render();
                //Graphics.SpriteBatch.End();
            }
        }

        // Called after a Scene ends, before the next Scene begins
        protected virtual void OnSceneTransition()
        {
            GC.Collect();                       // Unload Garbage Collector on Scene transition
            GC.WaitForPendingFinalizers();

            Input.Touch.CleanActions();         // Unload Input events on Scene transition
            AudioManager.Unload();              // Unload SFX and Music on Scene transition
            Camera.Reset();                     // Unload Camera and reset on Scene transition

            Engine.Instance.TimeRate = 1f;
        }

        #region Utils

        public void LoadScene(string scenename)
        {
            if (!_SceneList.ContainsKey(scenename))
                Debug.ErrorLog("[SceneManager]: Couldn't load scene '" + scenename + "' name invalid or not declared.");
            else
                _NextScene = _SceneList[scenename];
        }

        public void RestartScene()
        {
            if (CurrScene != null)
            {
                CurrScene.End();

                OnSceneTransition();

                CurrScene.Awake();
                CurrScene.Start();
            }
        }

        /// <summary>
        /// Return current scene name.
        /// </summary>
        /// <returns>The name.</returns>
        public string SceneName()
        {
            if (CurrScene != null)
                return CurrScene.Name;
            else
                Debug.ErrorLog("[SceneManager]: First scene not loaded.");

            return "";
        }

        public List<Layer> SceneLayers()
        {
            return CurrScene.Layers.Values.ToList();
        }

        public int ScenePopulation()
        {
            return CurrScene.Population();
        }

        public List<Entity> SceneEntities()
        {
            var entities = new List<Entity>();

            foreach(var l in CurrScene.Layers.Values)
                entities.AddRange(l.Entities);

            return entities;
        }

        public List<Entity> LayerEntities(string layerName)
        {
            var entities = new List<Entity>();

            if (CheckGlobal(layerName))
                entities.AddRange(_GlobalLayers[layerName].Entities);
            else if (CurrScene.Layers.ContainsKey(layerName))
                entities.AddRange(CurrScene.Layers[layerName].Entities);
            else
                Debug.ErrorLog("[SceneManager]: Layer '" + layerName + "'not found while trying to get Entities.");

            return entities;
        }

        public void ForceAwakeGlobalEntities(string layerName)
        {
            if (!_GlobalLayers.ContainsKey(layerName))
                Debug.ErrorLog("[SceneManager]: Global layer '" + layerName + "' not found.");
            else
                // Update Global Layer
                _GlobalLayers[layerName].AwakeEntities();
        }

        public void AddGlobal(string name, Layer layer)
        {
            // Check Global Layers
            if (!_GlobalLayers.ContainsKey(name))
            {
                _GlobalLayers.Add(name, layer);
                Debug.Log("[SceneManager]: '" + name + "' added to Global layers stack.");
            }
            // Already on Global
            else
                Debug.Log("[SceneManager]: '" + name + "' Global layer already on stack.");
        }

        public Layer GetGlobal(string layerName)
        {
            if (!_GlobalLayers.ContainsKey(layerName))
                Debug.ErrorLog("[SceneManager]: Global layer '" + layerName + "' not found.");

            return _GlobalLayers[layerName];
        }

        public bool CheckGlobal(string layerName)
        {
            return _GlobalLayers.ContainsKey(layerName);
        }

        public void Pause()
        {
            CurrScene.Pause();
        }

        public void UnPause()
        {
            CurrScene.Resume();
        }

        #endregion
    }
}
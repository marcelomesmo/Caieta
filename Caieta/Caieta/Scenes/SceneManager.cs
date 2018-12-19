using System;
using System.Collections.Generic;
using System.Linq;

namespace Caieta
{
    public class SceneManager
    {
        private Dictionary<string, Scene> _SceneList;
        private Scene _CurrScene;
        private Scene _NextScene;
        private Dictionary<string, Layer> _GlobalLayers;

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

            // Check First Scene
            if (!_SceneList.ContainsKey(firstScene))
                throw new ArgumentException("[SceneManager]: First scene '" + firstScene + "'. Name invalid or not declared.");
            else _CurrScene = _NextScene = _SceneList[firstScene];


            Debug.LogLine();

            // Debug initial entities
            // Notes: Make this into Console Debug later.
            // Debug.Log("[SceneManager]: Scene '" + _CurrScene.Name + "' has total: " + _CurrScene.Population() + " entities created.");
        }

        internal void Begin()
        {
            // Start First Scene
            _CurrScene.Begin();
        }

        internal void Update()
        {
            // Update Scene
            if (_CurrScene != null)
            {
                _CurrScene.Update();

                // Update Layers
                if (!_CurrScene.Paused)
                {
                    foreach (var _CurrLayer in _CurrScene.Layers.Values)
                    {
                        if (_CurrLayer.IsGlobal)
                        {
                            if (!_GlobalLayers.ContainsKey(_CurrLayer.Name))
                                Debug.ErrorLog("[SceneManager]: Global layer '" + _CurrLayer.Name + "' not found.");
                            else
                            {
                                // Update Global Layer
                                _GlobalLayers[_CurrLayer.Name].UpdateLists();
                                _GlobalLayers[_CurrLayer.Name].Update();
                            }
                        }
                        else
                        {
                            // Update Local Layer
                            _CurrLayer.UpdateLists();
                            _CurrLayer.Update();
                        }
                    }
                }

                _CurrScene.LateUpdate();
            }

            // Change Scenes
            if (_CurrScene != _NextScene)
            {
                if (_CurrScene != null)
                    _CurrScene.End();

                _CurrScene = _NextScene;

                OnSceneTransition();

                if (_CurrScene != null)
                    _CurrScene.Begin();
            }
        }

        internal void Render()
        {
            if (_CurrScene != null)
            {
                _CurrScene.Render();
            }
        }

        // Called after a Scene ends, before the next Scene begins
        protected virtual void OnSceneTransition()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();

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

        /// <summary>
        /// Return current scene name.
        /// </summary>
        /// <returns>The name.</returns>
        public string SceneName()
        {
            if (_CurrScene != null)
                return _CurrScene.Name;
            else
                Debug.ErrorLog("[SceneManager]: First scene not loaded.");

            return "";
        }

        public List<Layer> SceneLayers()
        {
            return _CurrScene.Layers.Values.ToList();
        }

        public int ScenePopulation()
        {
            return _CurrScene.Population();
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

        public bool CheckGlobal(string name)
        {
            return _GlobalLayers.ContainsKey(name);
        }

        #endregion
    }
}
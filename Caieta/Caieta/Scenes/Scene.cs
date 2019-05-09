using System;
using System.Collections;
using System.Collections.Generic;
using Caieta.Entities;
using Microsoft.Xna.Framework;

namespace Caieta
{
    public abstract class Scene
    {
        /*
         *      RUNTIME
         */
        public bool Paused { get; private set; }
        public float TimeActive { get; private set; }
        public float RawTimeActive { get; private set; }

        public Rectangle Layout;
        public int TileWidth = 16;
        public int TileHeight = 16;

        /*
         *      LAYOUT SIZE
         */
        // public Vector2 SceneBounds;

        /*
         *      ID
         */
        public string Name;

        /*
         *      LAYERS
         */
        public Dictionary<string, Layer> Layers { get; private set; }

        public event Action OnSceneStart;
        public event Action OnSceneEnd;

        protected Scene()
        {
            Layers = new Dictionary<string, Layer>();

            Layout = new Rectangle(0, 0, Graphics.Width, Graphics.Height);
        }

        /*
         * Trigger Actions for OnSceneStart and OnSceneEnd
         */
        public virtual void Awake()
        {
            Debug.Log("[Scene]: Awake scene '" + Name + "'.");
        }

        public virtual void Start()
        {
            Debug.Log("[Scene]: Start scene '" + Name + "'.");

            // First Update Entities
            foreach (var layer in Layers.Values)
            {
                if (!layer.IsGlobal)
                    layer.UpdateLists();
                else
                    Engine.SceneManager.ForceUpdateGlobal(layer.Name);
            }

            // Trigger OnSceneStart after everything is loaded and every entity is added.
            if (OnSceneStart != null)
            {
                Debug.Log("[Scene]: On Scene Start trigger.");

                OnSceneStart();
                OnSceneStart = null;
            }
        }

        public virtual void End()
        {
            Debug.Log("[Scene]: Closing scene '" + Name + "'.");

            if (OnSceneEnd != null)
            {
                Debug.Log("[Scene]: On Scene End trigger.");

                OnSceneEnd();
                OnSceneEnd = null;
            }

            foreach (var layer in Layers.Values)
            {
                if (!layer.IsGlobal)
                {
                    Debug.Log("     Unloading layer '" + layer.Name + "'.");

                    foreach (var ent in layer.Entities)
                    {
                        Debug.Log("          Unloading entity '" + ent.Name + "'.");
                        ent.Unload();
                    }

                    layer.Unload();
                }
            }

            Layers.Clear();
        }

        /*
         * Update sequence: Update -> LateUpdate
         */
        public virtual void Update()
        {
            // Scene active time
            if (!Paused) TimeActive += Engine.Instance.DeltaTime;

            // Scene total life time (independent of timerate & pause)
            RawTimeActive += Engine.Instance.RawDeltaTime;
        }

        public virtual void LateUpdate()
        {

        }
        /*
         * Notes: Removed because not necessary. Every render is made inside the layers.     
            public virtual void Render()
            {

            }
        */
        public virtual void Pause()
        {
            Paused = true;
            Engine.Instance.TimeRate = 0f;

            Debug.Log("[Scene]: Paused scene '" + Name + "'.");
        }

        public virtual void Resume()
        {
            Paused = false;
            Engine.Instance.TimeRate = 1f;

            Debug.Log("[Scene]: Resumed scene '" + Name + "'.");
        }

        #region Layers

        public void Add(Layer layer)
        {
            // Auto make Layer global if global is set in previous layer
            if (Engine.SceneManager.CheckGlobal(layer.Name))
                layer.SetGlobal();

            if (Layers.ContainsKey(layer.Name))
                Debug.ErrorLog("[Scene]: Layer '" + layer.Name + "' already added to Scene '" + Name + "'.");
            else
                Layers.Add(layer.Name, layer);
        }

        public void SetVisible(string layername, bool visibility)
        {
            if (!Layers.ContainsKey(layername))
                Debug.ErrorLog("[Scene]: Couldn't access layer '" + layername + "'. Name invalid or not declared.");
            else 
                Layers[layername].IsVisible = visibility;
        }

        // SetParallax
        // SetAngle
        // SetScale

        // Count how many entities we have in the scene
        public int Population()
        {
            var pop = 0;

            // Count how many entities we have in all layers belonging to this scene
            foreach (var _layer in Layers.Values)
                pop += _layer.Population();

            return pop;
        }

        #endregion

        #region Entities

        /*
        public void CreateObject(Entity type, string layer, int x, int y)
        {
            Entity ent = type.Clone();

            ent.Transform.Position = new Microsoft.Xna.Framework.Vector2(x, y);

            if (!Layers.ContainsKey(layer))
                Debug.ErrorLog("[Scene]: Layer '" + layer + "' invalid or not declared.");
            else
                Layers[layer].Add(ent);
        }

        public void CreateObject(Entity type, string layer, string image_point)
        {
            if (!type.ImagePoint.ContainsKey(image_point))
                Debug.ErrorLog("[Scene]: Image Point '" + image_point + "' invalid or not declared.");
            else
            {
                Microsoft.Xna.Framework.Vector2 pos = type.ImagePoint[image_point];

                CreateObject(type, layer, pos.X, pos.Y);
            }
        }
        */

        #endregion

        #region Implementation of IEnumerable

        /*public IEnumerator<Layer> GetEnumerator()
        {
            return Layers.GetEnumerator();
        }

        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }*/

        #endregion


        #region Interval

        // Trigger every "interval" seconds. Ex: given 2.0f, this will return true once every 2 seconds.
        public bool EverySeconds(float interval)
        {
            return (int)((TimeActive - Engine.Instance.DeltaTime) / interval) < (int)(TimeActive / interval);
        }

        #endregion
    }
}

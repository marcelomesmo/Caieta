using System;
using System.Collections;
using System.Collections.Generic;

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
        }

        /*
         * Trigger Actions for OnSceneStart and OnSceneEnd
         */
        public virtual void Begin()
        {
            Debug.Log("[Scene]: Starting scene '" + Name + "'.");

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

            foreach(var layer in Layers.Values)
            {
                if(!layer.IsGlobal)
                {
                    foreach (var ent in layer.Entities)
                        ent.Unload();
                }
            }

            if (OnSceneEnd != null)
            {
                Debug.Log("[Scene]: On Scene End trigger.");

                OnSceneEnd();
                OnSceneEnd = null;
            }
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

        public virtual void Render()
        {
            foreach (var _CurrLayer in Layers.Values)
            {
                if(_CurrLayer.IsVisible)
                    _CurrLayer.Render();
            }
        }

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

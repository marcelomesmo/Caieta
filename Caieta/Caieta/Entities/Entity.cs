using System;
using System.Collections.Generic;
using Caieta.Components;

namespace Caieta.Entities
{
    public class Entity
    {
        /*
         *      PROPERTIES
         */
        public string Name { get; private set; }
        public bool IsVisible;

        /*
         *      POSITION, ANGLE & SCALE
         */
        //public Transform transform;

        /*
         *      ACTIONS
         */
        public event Action OnCreate;
        public event Action OnDestroy;

        /*
         *      COMPONENTS
         */
        public List<Component> Components;

        // Notes: Think about taking initial_visibility out, pattern Constructors and init
        public Entity(string entityname, bool initial_visibility = true)
        {
            Name = entityname;
            IsVisible = initial_visibility;

            Components = new List<Component>();
        }

        public virtual void Create()
        {
            Debug.Log("[Entity]: Entity '" + Name + "' added to Scene.");

            if (OnCreate != null)
            {
                Debug.Log("[Entity]: On Create entity trigger.");

                OnCreate();
                OnCreate = null;
            }
        }

        public virtual void Destroy()
        {
            Debug.Log("[Entity]: Entity '" + Name + "' removed from Scene.");

            if (OnDestroy != null)
            {
                Debug.Log("[Entity]: On Destroy entity trigger.");

                OnDestroy();
                OnDestroy = null;
            }
        }

        public virtual void Update()
        {
            foreach (var component in Components)
                component.Update();
        }

        public virtual void Render()
        {
            foreach (var component in Components)
                component.Render();
        }

        public virtual void Unload()
        {
            foreach (var component in Components)
                component.Unload();
        }

        #region Components

        public void Add(Component component)
        {
            Components.Add(component);
        }

        public T Get<T>() where T : Component
        {
            foreach (var component in Components)
                if (component is T)
                    return component as T;
            return null;
        }

        #endregion
    }
}

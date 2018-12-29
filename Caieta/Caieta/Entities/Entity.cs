using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

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
        public Transform Transform;

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

            Transform = new Transform();

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
            // Check if a component is Unique and have already been added.
            if (component is IUnique)
            {
                var typeOfcomponent = component.GetType();

                Debug.Log("type of component : " + component.GetType());

                foreach (var c in Components)
                    if (c.GetType() == typeOfcomponent)
                    {
                        Debug.WarningLog("[Entity]: '" + Name + "' already have a '"+ typeOfcomponent + "' unique component.");
                        return;
                    }
            }

            // Add Component to this Entity
            component.Entity = this;
            Components.Add(component);
            component.Initialize();
            // Notes: for future reference, instead of having a straight Initialize() we can add component to a _toAdd list and post-initialize all at the same time or in a specific order.
        }

        // Return first
        public T Get<T>() where T : Component
        {
            foreach (var component in Components)
                if (component is T)
                    return component as T;
            return null;
        }

        // Return all
        public List<T> GetAll<T>() where T : Component
        {
            var list = new List<T>();

            foreach (var component in Components)
                if (component is T)
                    list.Add( component as T );

            return list;
        }

        // Check if a Component exists
        public bool Has<T>() where T : Component
        {
            var comp = Get<T>();
            if (comp == null)
                return false;

            return true;
        }

        #endregion

        #region Tags & Collision

        /*
        public bool IsCollidable; IsTrigger;
         
        public string TagName;
        private int _Flag;

        public void SetCollisionFilter(string tag, params string[] tags)
        {

        }
        */

        #endregion
    }
}

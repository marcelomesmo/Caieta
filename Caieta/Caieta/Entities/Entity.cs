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
        public Layer Layer { get; private set; }

        /*
         *      POSITION, ANGLE & SCALE
         */
        public Transform Transform;

        /*
         *      ACTIONS
         */
        public event Action OnCreate;
        public event Action OnStart;
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
            //Debug.Log("[Entity]: Entity '" + Name + "' added to Scene.");

            if (OnCreate != null)
            {
                Debug.Log("[Entity]: On Create entity trigger.");

                OnCreate();
                OnCreate = null;
            }
        }

        public virtual void Start()
        {
            //Debug.Log("[Entity]: Entity '" + Name + "' started. All components initialized.");

            if (OnStart != null)
            {
                Debug.Log("[Entity]: On Start entity trigger.");

                OnStart();
                OnStart = null;
            }
        }

        public virtual void Destroy()
        {
            if (OnDestroy != null)
            {
                Debug.Log("[Entity]: On Destroy entity trigger.");

                OnDestroy();
                OnDestroy = null;
            }
            /*
            Transform.Parent = null;

            // Destroy any children we have
            for (var i = Transform.Children.Count - 1; i >= 0; i--)
            {
                Transform.Children[i] = null;
            }
            Transform.Children.Clear();

            Components.Clear();
            // Notes: Add Components.Remove in order to call Component.OnRemoved. But I dont believe its necessary.
            */
            // Remove itself from the scene
            Layer.Remove(this);
            /*foreach (Layer l in Engine.SceneManager.SceneLayers())
                if(l.Remove(this)) 
                    break;*/
        }

        public void Removed()
        {
            Transform.Parent = null;

            // Destroy any children we have
            for (var i = Transform.Children.Count - 1; i >= 0; i--)
            {
                Transform.Children[i] = null;
            }
            Transform.Children.Clear();

            Components.Clear();
            // Notes: Add Components.Remove in order to call Component.OnRemoved. But I dont believe its necessary.

            Debug.Log("[Entity]: Entity '" + Name + "' removed from Scene.");
        }

        public virtual void Update()
        {
            foreach (var component in Components)
                if(component.IsActive)
                    component.Update();
        }

        public virtual void Render()
        {
            foreach (var component in Components)
                if(component is Renderable r && r.IsVisible)
                    component.Render();
        }

        public virtual void Unload()
        {
            foreach (var component in Components)
                component.Unload();
        }

        public virtual void Added(Layer layer)
        {
            Layer = layer;
        }

        #region Components

        public void Add(Component component)
        {
            if(component != null)
            {
                // Check if a component is Unique and have already been added.
                if (component is IUnique)
                {
                    var typeOfcomponent = component.GetType();

                    Debug.Log("type of component : " + component.GetType());

                    foreach (var c in Components)
                        if (c.GetType() == typeOfcomponent)
                        {
                            Debug.WarningLog("[Entity]: '" + Name + "' already have a '" + typeOfcomponent + "' unique component.");
                            return;
                        }
                }

                // Add Component to this Entity
                component.Entity = this;
                Components.Add(component);
                component.Initialize();
                // Notes: for future reference, instead of having a straight Initialize() we can add component to a _toAdd list and post-initialize all at the same time or in a specific order.

            }
            else {
                Debug.ErrorLog("[Entity]: Trying to add component to '" + Name + "'. Component cant be 'null'.");
            }
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

        public void MirrorColliders(bool mirror)
        {
            var collider_list = GetAll<Collider>();

            foreach (Collider c in collider_list)
                c.IsMirrored = mirror;
        }

        public void FlipColliders(bool flip)
        {
            var collider_list = GetAll<Collider>();

            foreach (Collider c in collider_list)
                c.IsFlipped = flip;
        }

        /*
        public bool IsCollidable; IsTrigger;
         
        public string TagName;
        private int _Flag;

        public void SetCollisionFilter(string tag, params string[] tags)
        {

        }
        */

        #endregion

        #region Utils
            /*
        public virtual Entity Clone()
        {
            var entity = Activator.CreateInstance(GetType()) as Entity;

            entity.Name = Name;

            entity.IsVisible = IsVisible;

            // clone Components
            for (var i = 0; i < Components.Count; i++)
                entity.Add(Components[i].Clone());

            entity.Transform = Transform;

            // clone any children of the Entity.transform
            for (var i = 0; i < entity.Transform.childCount; i++)
            {
                var child = entity.transform.getChild(i).entity;

                var childClone = child.clone();
                childClone.transform.copyFrom(child.transform);
                childClone.transform.parent = transform;
            }
        }
        */
        #endregion
    }
}

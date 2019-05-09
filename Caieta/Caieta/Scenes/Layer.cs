using System;
using System.Collections;
using System.Collections.Generic;
using Caieta.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Caieta
{
    public class Layer : IEnumerable<Entity>, IEnumerable
    {
        public string Name { get; private set; }

        public bool IsVisible;
        public bool IsGlobal { get; private set; }

        public Vector2 Parallax
        {
            get { return parallax; }
            set
            {
                parallax.X = MathHelper.Clamp(value.X, 0, 100);

                parallax.Y = MathHelper.Clamp(value.Y, 0, 100);

                /*Debug.Log("[Layer]: '" + Name + "' parallax set to " + parallax.X + ", " + parallax.Y +
                          " from within values " + value.X + ", " + value.Y + ".");
                */
            }
        }
        private Vector2 parallax;//Vector2.Zero;

        public List<Entity> Entities { get; private set; }
        private List<Entity> _toAdd { get; set; }
        //private List<Entity> _toStart { get; set; }
        private List<Entity> _toRemove { get; set; }
        // Notes: Auxiliar hashes useful for O(1) set.Contains operations.
        // Notes: There are multiple stack overflow threads on this subject. 
        private HashSet<Entity> _current { get; set; }
        private HashSet<Entity> _adding { get; set; }
        private HashSet<Entity> _removing { get; set; }

        public Layer(string layername)
        {
            Name = layername;

            // Notes: Default behavior is set inside constructor to force method calls when creating object
            IsVisible = true;
            IsGlobal = false;
            Parallax = new Vector2(100, 100);

            Entities = new List<Entity>();
            _toAdd = new List<Entity>();
            _toRemove = new List<Entity>();


            _current = new HashSet<Entity>();
            _adding = new HashSet<Entity>();
            _removing = new HashSet<Entity>();
        }

        #region Fluent Constructor

        public void SetGlobal()
        {
            IsGlobal = true;

            Engine.SceneManager.AddGlobal(Name, this);
        }

        #endregion

        public virtual void UpdateLists()
        {
            /*
             * Update Entity List
             */

            // Update adding new entities
            if (_toAdd.Count > 0)
            {
                for (int i = 0; i < _toAdd.Count; i++)
                {
                    var entity = _toAdd[i];
                    if (!_current.Contains(entity))     // Notes: O(1) operation.
                    {
                        _current.Add(entity);           // Notes: Using this cause Entities.Contains(entity) is O(n).
                        Entities.Add(entity);

                        entity.Added(this);
                        // Trigger Entity OnCreate
                        entity.Create();
                    }
                }

                foreach(var entity in _toAdd)
                    entity.Start();

                _toAdd.Clear();
                _adding.Clear();
            }

            if (_toRemove.Count > 0)
            {
                for (int i = 0; i < _toRemove.Count; i++)
                {
                    var entity = _toRemove[i];
                    if (_current.Contains(entity))
                    {
                        _current.Remove(entity);
                        Entities.Remove(entity);

                        // Trigger Entity OnDestroy
                        //entity.Destroy();
                        entity.Removed();
                    }
                }

                _toRemove.Clear();
                _removing.Clear();
            }

            // Notes: If necessary, sort Z Index here
        }

        public virtual void Update()
        {
            // Update Entities
            foreach (var _entity in Entities)
                //if(_entity.IsActive)
                _entity.Update();
        }

        public virtual void Render()
        {
            // Render Entities
            foreach (var _entity in Entities)
                if(_entity.IsVisible)
                    _entity.Render();
        }

        public virtual void Unload()
        {
            Entities.Clear();
            _toAdd.Clear();
            _toRemove.Clear();

            _current.Clear();
            _adding.Clear();
            _removing.Clear();
        }

        #region Entities

        public void Add(Entity entity)
        {
            // Check if we ain't adding an already toAdd    entity
            // Check if we ain't adding an already existing entity
            if (!_adding.Contains(entity) && !_current.Contains(entity))
            {
                _adding.Add(entity);
                _toAdd.Add(entity);
            }
        }

        public bool Remove(Entity entity)
        {
            // Check if we ain't removing an already toRemove entity
            // Check if we are   removing an         existing entity
            if (!_removing.Contains(entity) && _current.Contains(entity))
            {
                _removing.Add(entity);
                _toRemove.Add(entity);

                return true;
            }
            else {
                Debug.Log("[Layer]: Trying to remove entity '"+ entity.Name + "' from layer '" + Name + "'. Entity already removing or not found.");
                return false;
            }

        }

        // Notes: If necessary, create Add and Remove methods for 'IEnumerable<Entity>' and 'params Entity[]'

        // Count how many entities we have in this layer
        public int Population()
        {
            return Entities.Count;
        }

        #endregion

        #region Entity Search

        // Get Entity by name
        public Entity GetEntity(string entityname)
        {
            foreach (var e in Entities)
                if (e.Name == entityname)
                    return e;

            return null;
        }

        // Get Entity by type
        public T GetEntity<T>() where T : Entity
        {
            foreach (var e in Entities)
                if (e is T)
                    return e as T;

            return null;
        }

        // Get Entity list by type
        public List<T> GetAllEntities<T>() where T : Entity
        {
            List<T> list = new List<T>();

            foreach (var e in Entities)
                if (e is T)
                    list.Add(e as T);

            return list;
        }

        #endregion

        #region Implementation of IEnumerable

        public IEnumerator<Entity> GetEnumerator()
        {
            return Entities.GetEnumerator();
        }

        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}

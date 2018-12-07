using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Caieta
{
    public class Layer : IEnumerable<Entity>, IEnumerable
    {
        public string Name { get; private set; }

        public bool IsVisible { get; set; }
        public bool IsGlobal { get; set; }
        public Vector2 Parallax
        {
            get { return parallax; }
            set
            {
                // 0 if < 0. 100 if > 100. else value
                parallax.X = value.X < 0 ? 0 : value.X > 0 ? 100 : value.X;

                parallax.Y = value.Y < 0 ? 0 : value.Y > 0 ? 100 : value.Y;

                Debug.Log("[Layer]: '" + Name + "' parallax set to " + parallax.X + ", " + parallax.Y +
                          " from within values " + value.X + ", " + value.Y + ".");
            }
        }
        private Vector2 parallax = Vector2.Zero;

        public List<Entity> Entities { get; private set; }
        private List<Entity> _toAdd { get; set; }
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

                        // Trigger Entity OnCreate
                        entity.Create();
                    }
                }

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
                        entity.Destroy();
                    }
                }

                _toRemove.Clear();
                _removing.Clear();
            }

            // Notes: If necessary, sort Z Index here

            // Notes: If necessary, trigger Awake here
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

        public void Remove(Entity entity)
        {
            // Check if we ain't removing an already toRemove entity
            // Check if we are   removing an         existing entity
            if (!_removing.Contains(entity) && _current.Contains(entity))
            {
                _removing.Add(entity);
                _toRemove.Add(entity);
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
